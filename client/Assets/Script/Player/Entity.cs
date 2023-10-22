using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public abstract class Entity : MonoBehaviour
{
    public string myid = "";
    public string name = "";
    
    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        ExtraUpdate();
    }

    protected abstract void init();
    protected abstract void ExtraUpdate();

    protected class EntityData{
        public string id {get; set;}
        public string method {get; set;}
        public Dictionary<string, float> pos;
        public Dictionary<string, float> rotate;
        public Dictionary<string, float> state;
    }
}
