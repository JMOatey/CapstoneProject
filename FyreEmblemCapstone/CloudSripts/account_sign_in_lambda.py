import boto3
import json
from urllib.parse import unquote

client = boto3.client('cognito-idp', region_name = 'us-east-1')

user_pool = "us-east-1_uyYfYsErr"
client_id = "6fpfb0p74fh9gb1g78j80h6ro"



# Sign into user account
# If this is the first sign in, respond to the password challenge
def sign_in(username, password):

    response = None

    try:
        auth_response = client.admin_initiate_auth(
            UserPoolId = user_pool,
            ClientId = client_id,
            AuthFlow = 'ADMIN_NO_SRP_AUTH',
            AuthParameters = {
                'USERNAME': username,
                'PASSWORD': password
            }
        )

        # Check if this is the first sign in
        if auth_response.get("ChallengeName") == "NEW_PASSWORD_REQUIRED":
            session_id = auth_response.get("Session")

            challenge_response = client.admin_respond_to_auth_challenge(
                UserPoolId = user_pool,
                ClientId = client_id,
                ChallengeName='NEW_PASSWORD_REQUIRED',
                ChallengeResponses={
                    'USERNAME': username,
                    'NEW_PASSWORD': password
                },
                Session=session_id
            )

            auth_result = challenge_response.get("AuthenticationResult")
        else:
            auth_result = auth_response.get("AuthenticationResult")


        if auth_result:
            access_token = auth_result.get("AccessToken")
            refresh_token = auth_result.get("RefreshToken")

            response = {
                "access_token": access_token,
                "refresh_token": refresh_token
            }
    except Exception as err:
        print(err)

    return response



def lambda_handler(event, context):
    event_body_encoded = event.get("body")
    event_body = unquote(event_body_encoded)

    event_body = json.loads(event_body)
    
    username = event_body.get('username').strip(u'\u200b')
    password = event_body.get('password').strip(u'\u200b')
    
    print(event_body)

    result = sign_in(username, password)
    code = True if result else False

    print(result)

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
