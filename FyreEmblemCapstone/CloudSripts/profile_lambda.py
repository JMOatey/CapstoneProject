import boto3
import json

dynamodb = boto3.resource('dynamodb')
table = dynamodb.Table('profile')


def get_profile(username):
    try:
        response = table.get_item(
            Key={
                'username': username
            }
        )

        return response.get('Item')
    except Exception as e:
        print(e)
        return None



def update_profile(username, profile_data):
    try:
        response = table.update_item(
            Key = {
                'username': username
            },
            UpdateExpression = "set profile = :pd",
            ExpressionAttributeValues = {
                ':pd': profile_data
            },
            ReturnValues = "ALL_NEW"
        )

        return response.get('Attributes')
    except Exception as e:
        print(e)
        return None


def lambda_handler(event, context):
    http_method = event.get("httpMethod")
    username = event.get("requestContext", {}).get("authorizer", {}).get("claims", {}).get("username")

    if http_method == "GET":
        response = get_profile(username)

    elif http_method == "PUT":
        event_body = json.loads(event.get("body", {}))

        profile_data = event_body.get("profile")
        response = update_profile(username, profile_data)

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