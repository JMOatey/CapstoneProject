import json


def multiplayer_game_create(game_definition):
    return game_definition


def multiplayer_game_move(game_move):
    return game_move


def lambda_handler(event, context):
    event_body = json.loads(event.get("body", "{}"))
    game_action = event_body.get("game_action")
    
    print(event_body)

    if game_action == "create":
        game_definition = event_body.get("game_definition")
        response = multiplayer_game_create(game_definition)

    elif game_action == "move":
        game_move = event_body.get("game_move")
        response = multiplayer_game_move(game_move)
    
    code = True if response else False

    body = {
        "message": response, 
        "code": code
    }

    return {
        'statusCode': 200,
        'body': json.dumps(body)
    }
