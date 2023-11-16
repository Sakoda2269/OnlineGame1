import json
import random
from collections import defaultdict
from channels.generic.websocket import AsyncWebsocketConsumer
import uuid

class GameConsumer(AsyncWebsocketConsumer):

    joined = set()
    id_name = dict()
    id_items = dict()
    myid = ""

    async def connect(self):
        print("connect")
        self.myid = str(uuid.uuid4())
        await self.accept()
        self.room_id = self.scope["url_route"]["kwargs"]["room_id"]
        await self.channel_layer.group_add(
            self.room_id,
            self.channel_name
        )
    
    async def disconnect(self, code):
        await self.channel_layer.group_send(
                self.room_id,
                {
                    "type": "sender",
                    "method": "leave",
                    "id": self.myid,
                    "data":{}
                }
            )
        await self.channel_layer.group_send(
                self.room_id,
                {
                    "type": "sender",
                    "method": "chat",
                    "id": self.myid,
                    "data":{
                        "chat":self.id_name[self.myid] + " left this game!"
                    }
                }
            )
        await self.channel_layer.group_discard(
            self.room_id,
            self.channel_name
        )
        self.joined.discard(self.myid)
        self.id_name.pop(self.myid)
        self.id_items.pop(self.myid)
        return await super().disconnect(code)
    
    async def receive(self, text_data=None, bytes_data=None):
        data = json.loads(text_data)
        print(data)
        if(data["method"] == "join"):
            new_id = self.myid
            self.joined.add(new_id)
            self.id_name[new_id] = data["name"]
            self.id_items[new_id] = data["items"]
            print(data)
            print(data["name"], "joined!")
            await self.channel_layer.group_send(
                self.room_id,
                {
                    "type": "sender",
                    "method": "join",
                    "id" : new_id,
                    "name" : data["name"],
                    "data":{
                        "joined":list(self.joined),
                        "id_items":self.id_items,
                        "id_name":self.id_name
                    }
                }
            )
            await self.channel_layer.group_send(
                self.room_id,
                {
                    "type": "sender",
                    "method": "chat",
                    "id" : new_id,
                    "name" : data["name"],
                    "data":{
                        "chat": data["name"] + " joined this game!"
                    }
                }
            )
        if(data["method"] == "leave"):
            leave_id = data["id"]
            if(leave_id != self.myid):
                print(leave_id, "left!")
                await self.channel_layer.group_send(
                    self.room_id,
                    {
                        "type": "sender",
                        "method": "leave",
                        "id": leave_id,
                        "data":{}
                    }
                )
                await self.channel_layer.group_send(
                    self.room_id,
                    {
                        "type": "sender",
                        "method": "chat",
                        "id": leave_id,
                        "data":{
                            "chat":self.id_name[leave_id] + " left this game!"
                        }
                    }
                )
                self.id_name.pop(leave_id)
        if(data["method"] == "update"):
            updata_id = data["id"]
            pos = data["pos"]
            rot = data["rotate"]
            state = data["state"]
            send_data = {
                        "pos":pos,
                        "rot":rot,
                        "state":state
                    }
            await self.channel_layer.group_send(
                self.room_id,
                {
                    "type": "sender",
                    "method": "update",
                    "id": updata_id,
                    "data":send_data
                }
            )
        if(data["method"] == "useItem"):
            print(data)

        if(data["method"] == "test"):
            print(data["data"])

            
    
    async def sender(self, event):
        method = event["method"]
        data = event["data"]
        send_id = event["id"]
        # Send message to WebSocket
        await self.send(text_data=json.dumps({
                "method": method, 
                "data":data, 
                "id":send_id
            }))
    
    