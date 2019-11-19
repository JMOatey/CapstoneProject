import boto3
import json
import pymysql

database_host       = "capstone.ce8exeo9ulmj.us-east-1.rds.amazonaws.com"
database_port       = 3306
database_username   = "admin"
database_password   = "capstone"
database_name       = "capstone"


client = boto3.client('cognito-idp', region_name = 'us-east-1')
dynamodb = boto3.resource('dynamodb')
table = dynamodb.Table('profile')
lookup_table = dynamodb.Table('save_lookup')

user_pool = "us-east-1_uyYfYsErr"
client_id = "6fpfb0p74fh9gb1g78j80h6ro"



# Sign into user account
# If this is the first sign in, respond to the password challenge
def sign_up(username, password, database=None):

    response = None

    try:
        create_user_response = client.admin_create_user(
            UserPoolId = user_pool,
            Username = username,
            TemporaryPassword = password,
            MessageAction='SUPPRESS'
        )

        # If the user was created create a profile and save entry lookup and a response message 
        if create_user_response.get("User"):
            response = "Account '{}' created".format(username)

            if database:
                with database.cursor() as cursor:
                    sql = "INSERT INTO profile (username, data) VALUES (%s, %s)"
                    cursor.execute(sql, (username, r"{}"))
                database.commit()

    except Exception as err:
        print(err)

    print(response)
    return response



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
    event_body = json.loads(event.get("body"))
    
    username = event_body.get('username')
    password = event_body.get('password')

    result = sign_up(username, password, database)
    code = True if result else False

    body = {
        "message": result, 
        "code": code
    }

    return {
        'statusCode': 200,
        'body': json.dumps(body)
    }



if __name__ == "__main__":
    result = lambda_handler({"username": "Nate1", "password": "password"}, None)

    print(result)