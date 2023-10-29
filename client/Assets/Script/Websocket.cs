using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using TMPro;

public class Websocket : MonoBehaviour
{
    WebSocket ws;
    public GameObject playerObject;
    public GameObject enemyObject;
    public GameObject chat;
    public GameObject hotBar;
    private GameObject myPlayer;
    private Dictionary<string, GameObject> enemies = new Dictionary<string, GameObject>();
    private Queue<Tuple<string, Vector3>> enemieIdPos = new Queue<Tuple<string, Vector3>>();
    private Player playerScript;
    // Start is called before the first frame update
    async void Start()
    {
        string adress = Join.adressIn;
        string myname = Join.nameIn;
        ws = new WebSocket("ws://"+ adress + "/ws/game/server1");
        ws.OnOpen += () => 
        {
            Debug.Log("connect");
            myPlayer = Instantiate(playerObject, new Vector3(-33, 4, -5), Quaternion.identity);
            playerScript = myPlayer.GetComponent<Player>();
            playerScript.name = myname;
            playerScript.SetHotBar(hotBar);
            playerScript.Join(ws);
        };

        ws.OnError += (e) =>
        {
            Debug.Log(e);
        };

        ws.OnClose += (e) => 
        {
            Debug.Log("connection close");
        };
        ws.OnMessage += (bytes) =>
        {
            var message_in = System.Text.Encoding.UTF8.GetString(bytes);
            JObject message = JObject.Parse(message_in);
            if(message["method"].ToString().Equals("join")){
                if(playerScript.myid.Equals("")){
                    playerScript.myid = message["id"].ToString();
                    playerScript.ws = ws;
                    JArray players = (JArray)message["data"]["joined"];
                    for(int i = 0; i < players.Count(); i++){
                        GameObject joinedEnemy;
                        joinedEnemy = Instantiate(enemyObject);
                        try{
                            JArray itemNames = (JArray)message["data"]["id_items"][players[i].ToString()];
                            List<string> itemNameList = new List<string>();
                            for(int j = 0; j < itemNames.Count(); j++){
                                itemNameList.Add(itemNames[i].ToString());
                            }
                            joinedEnemy.GetComponent<Enemy>().SetItems(itemNameList.ToArray());
                            enemies.Add(players[i].ToString(), joinedEnemy);
                            joinedEnemy.GetComponent<Enemy>().name = 
                                message["data"]["id_name"][players[i].ToString()].ToString();
                            GameObject nameTag = joinedEnemy.transform.GetChild(1).gameObject;
                            GameObject canvas = nameTag.transform.GetChild(0).gameObject;
                            GameObject text = canvas.transform.GetChild(0).gameObject;
                            text.GetComponent<TMP_Text>().text = joinedEnemy.GetComponent<Enemy>().name;
                        }
                        catch(NullReferenceException e){
                        }
                    }
                }else{
                    GameObject joinedEnemy = Instantiate(enemyObject);
                    enemies.Add(message["id"].ToString(), joinedEnemy);
                    Debug.Log(message["data"]["id_name"][message["id"].ToString()]);
                    JArray itemNames = (JArray)message["data"]["id_items"][message["id"].ToString()];
                    List<string> itemNameList = new List<string>();
                    for(int j = 0; j < itemNames.Count(); j++){
                        itemNameList.Add(itemNames[message["id"].ToString()].ToString());
                    }
                    joinedEnemy.GetComponent<Enemy>().SetItems(itemNameList.ToArray());
                    joinedEnemy.GetComponent<Enemy>().name = message["data"]["id_name"][message["id"].ToString()].ToString();
                    GameObject nameTag = joinedEnemy.transform.GetChild(1).gameObject;
                    GameObject canvas = nameTag.transform.GetChild(0).gameObject;
                    GameObject text = canvas.transform.GetChild(0).gameObject;
                    text.GetComponent<TMP_Text>().text = joinedEnemy.GetComponent<Enemy>().name;
                }
            }
            if(message["method"].ToString().Equals("update")){
                if(!message["id"].ToString().Equals(playerScript.myid)){
                    Vector3 pos = new Vector3(
                        float.Parse(message["data"]["pos"]["x"].ToString()),
                        float.Parse(message["data"]["pos"]["y"].ToString()),
                        float.Parse(message["data"]["pos"]["z"].ToString())
                    );
                    Vector3 rot = new Vector3(
                        float.Parse(message["data"]["rot"]["x"].ToString()),
                        float.Parse(message["data"]["rot"]["y"].ToString()),
                        float.Parse(message["data"]["rot"]["z"].ToString())
                    );
                    Dictionary<string, float> state = new Dictionary<string, float>{
                        {"speed", float.Parse(message["data"]["state"]["speed"].ToString())},
                        {"jump", float.Parse(message["data"]["state"]["jump"].ToString())}
                    };
                    try{
                        enemies[message["id"].ToString()].GetComponent<Enemy>().UpdatePosRot(pos, rot, state);
                        Vector3 tagDire = pos - myPlayer.transform.position;
                        GameObject nameTag = enemies[message["id"].ToString()].transform.GetChild(1).gameObject;
                        Quaternion q = Quaternion.LookRotation(tagDire);
                        Vector3 rotation = q.eulerAngles;
                        rotation.x = 0;
                        rotation.z = 0;
                        rotation.y -= 180.0f;
                        nameTag.transform.rotation = Quaternion.Euler(rotation);
                    }catch(KeyNotFoundException e){
                    }
                }
            }
            if(message["method"].ToString().Equals("leave")){
                Destroy(enemies[message["id"].ToString()]);
                enemies.Remove(message["id"].ToString());
            }
            if(message["method"].ToString().Equals("chat")){
                chat.GetComponent<ChatLog>().NewChat(message["data"]["chat"].ToString());
            }
        };
        // InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);
        Debug.Log(playerScript);
        await ws.Connect();

    }

    // Update is called once per frame
    void Update()
    {
        if (ws.State == WebSocketState.Open)
        {
            #if !UNITY_WEBGL || UNITY_EDITOR
                ws.DispatchMessageQueue();
            #endif
            try{
                playerScript.UpdatePos(ws);

            } catch(NullReferenceException e){

            }
            
        }
    }

    private void OnDestroy() {
        playerScript.Leave(ws);
        ws.Close();
        ws = null;
    }

}
