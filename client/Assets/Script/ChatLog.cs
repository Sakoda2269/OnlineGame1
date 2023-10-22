using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatLog : MonoBehaviour
{
    public GameObject chat;
    private Queue<GameObject> chatLogs;
    // Start is called before the first frame update
    void Start()
    {
        chatLogs = new Queue<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewChat(string detail){
        foreach (GameObject c in chatLogs){
            Vector3 pos = c.GetComponent<RectTransform>().position;
            pos.y -= 37;
            c.GetComponent<RectTransform>().position = pos;
        }
        GameObject newchat = Instantiate(chat);
        newchat.GetComponent<RectTransform>().position = new Vector3(615, 121, 0);
        newchat.transform.parent = this.gameObject.transform;
        newchat.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = detail;
        chatLogs.Enqueue(newchat);
        Invoke("RemoveTop", 5);
    }

    private void RemoveTop(){
        Destroy(chatLogs.Dequeue());
    }

}
