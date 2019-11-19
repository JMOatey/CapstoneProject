#!/usr/bin/env python

# WS client example

import asyncio
import websockets

async def hello():
    uri = "wss://wkd5lvn0t0.execute-api.us-east-1.amazonaws.com/dev"
    async with websockets.connect(uri) as websocket:

        await websocket.send("test")

        response = await websocket.recv()
        print(response)

asyncio.get_event_loop().run_until_complete(hello())