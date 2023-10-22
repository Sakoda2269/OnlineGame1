from django.urls import re_path, path
 
from . import consumers
 
websocket_urlpatterns = [
    path("ws/game/<str:room_id>", consumers.GameConsumer.as_asgi()),
]