import boto3
import json
import pymysql

database_host       = "capstone.ce8exeo9ulmj.us-east-1.rds.amazonaws.com"
database_port       = 3306
database_username   = "admin"
database_password   = "capstone"
database_name       = "capstone"


dynamodb = boto3.resource('dynamodb')


def get_profile(username, database):
    try:
        with database.cursor() as cursor:
            sql = "SELECT data FROM profile WHERE username=%s"
            cursor.execute(sql, (username, ))
            result = cursor.fetchone()

        return result.get('data')
    except Exception as e:
        print(e)
        return None



def update_profile(username, profile_data, database):
    try:
        with database.cursor() as cursor:
            sql = "UPDATE profile SET data=%s WHERE username=%s"
            cursor.execute(sql, (profile_data, username))
        database.commit()

        return f"{username}'s profile has been updated."
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

    # Handle event
    http_method = event.get("httpMethod")
    username = event.get("requestContext", {}).get("authorizer", {}).get("claims", {}).get("username")

    if http_method == "GET":
        response = get_profile(username, database)

    elif http_method == "PUT":
        event_body = json.loads(event.get("body", {}))

        profile_data = event_body.get("profile")
        response = update_profile(username, profile_data, database)

    code = True if response else False

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