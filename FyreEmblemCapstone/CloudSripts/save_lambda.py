import boto3
import json
import pymysql

database_host       = "capstone.ce8exeo9ulmj.us-east-1.rds.amazonaws.com"
database_port       = 3306
database_username   = "admin"
database_password   = "capstone"
database_name       = "capstone"


dynamodb = boto3.resource('dynamodb')

def create_save(username, save_data, database):
    try:
        with database.cursor() as cursor:
            sql = "INSERT INTO save (username, data) VALUES (%s, %s)"
            cursor.execute(sql, (username, save_data))
        database.commit()

        return f"New save created for {username}"
    except Exception as e:
        print(e)
        return None



def get_save(username, save_id, database):
    try:
        with database.cursor() as cursor:
            if save_id:
                sql = "SELECT data FROM save WHERE username=%s AND id=%s"
                cursor.execute(sql, (username, save_id))
                result = cursor.fetchone()

                return result.get('data')
            else:
                sql = "SELECT data FROM save WHERE username=%s"
                cursor.execute(sql, (username, ))
                result = cursor.fetchall()
                result = [res.get('data') for res in result]

                return result

    except Exception as e:
        print(e)
        return None



def delete_save(username, save_id, database):
    try:
        with database.cursor() as cursor:
            sql = "DELETE FROM save WHERE username=%s AND id=%s"
            cursor.execute(sql, (username, save_id))
        database.commit()

        return f"Save deleted for {username}"
    except Exception as e:
        print(e)
        return None



def update_save(username, save_id, save_data, database):
    try:
        with database.cursor() as cursor:
            sql = "UPDATE save SET data=%s WHERE username=%s AND id=%s"
            cursor.execute(sql, (save_data, username, save_id))
        database.commit()

        return f"{username}'s save data has been updated."
    except Exception as e:
        print(e)
        return None


def lambda_handler(event, context):
    # Connect to database
    database = pymysql.connect(
        database_host, 
        user=database_username,
        passwd=database_password, 
        db=database_name, 
        connect_timeout=5,
        cursorclass=pymysql.cursors.DictCursor)

    print(event)

    # Handle event
    http_method = event.get("httpMethod")
    username = event.get("requestContext", {}).get("authorizer", {}).get("claims", {}).get("username")
    event_body = event.get("body", r"{}")
    event_body = json.loads(event_body if event_body else r"{}")
    event_path = event.get("path", "").split("/")

    response = None
    
    if http_method == "POST":
        save_data = event_body.get("save")
        response = create_save(username, save_data, database)

    elif http_method == "GET":
        # If a save ID is applied, fetch that one
        if len(event_path) == 3:
            save_id = event_path[2]
            response = get_save(username, save_id, database)

        # Otherwise, return all of them
        else:
            response = get_save(username, None, database)

    elif http_method == "PUT":
        if len(event_path) == 3:
            save_id = event_path[2]
            save_data = event_body.get("save")
            response = update_save(username, save_id, save_data, database)

    elif http_method == "DELETE":
        if len(event_path) == 3:
            save_id = event_path[2]
            response = delete_save(username, save_id, database)

    code = True if (response or type(response) == type([])) else False

    body = {
        "message": response, 
        "code": code
    }
    
    return {
        'statusCode': 200,
        'body': json.dumps(body)
    }


if __name__ == "__main__":
    lambda_handler(None, None)