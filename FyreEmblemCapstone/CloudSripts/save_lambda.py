import boto3
import json

dynamodb = boto3.resource('dynamodb')
table = dynamodb.Table('save')
lookup_table = dynamodb.Table('save_lookup')

def create_save(username, save_data):
    try:
        response = table.get_item(
            Key={
                'username': username
            }
        )

        print(response)

        return ""
        # return response.get('Item')
    except Exception as e:
        print(e)
        return None



def get_save(username, save_id):
    try:
        response = table.get_item(
            Key={
                'username': username,
                'save_id': save_id
            }
        )

        return response.get('Item')
    except Exception as e:
        print(e)
        return None



def delete_save(username, save_id):
    try:
        response = table.delete_item(
            Key={
                'username': username,
                'save_id': save_id
            }
        )

        return response.get('Item')
    except Exception as e:
        print(e)
        return None



def update_save(username, save_id, save_data):
    try:
        response = table.update_item(
            Key={
                'username': username,
                'save_id': save_id
            },
            UpdateExpression = "set profile = :sd",
            ExpressionAttributeValues = {
                ':sd': save_data
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
    event_body = json.loads(event.get("body", "{}"))

    print(event)

    if http_method == "POST":
        save_data = event_body.get("save")
        response = create_save(username, save_data)

    elif http_method == "GET":
        save_id = event.get("headers", {}).get("id")
        response = get_save(username, save_id)

    elif http_method == "PUT":
        save_id = event.get("headers", {}).get("id")
        save_data = event_body.get("save")
        response = update_save(username, save_id, save_data)

    elif http_method == "DELETE":
        save_id = event.get("headers", {}).get("id")
        response = delete_save(username, save_id)

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