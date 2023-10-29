using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class Enemy : Entity
{
    private Vector3 myPos;
    private Vector3 myRot;
    private Animator anim;
    protected override void init(){
        myPos = new Vector3(0, 0, 0);
        myRot = new Vector3(0, 0, 0);
        anim = this.gameObject.GetComponent<Animator>();
    }
    protected override void ExtraUpdate(){
        this.transform.position = myPos;
        this.transform.localEulerAngles = myRot;
    }
    public void UpdatePosRot(Vector3 pos, Vector3 rot, Dictionary<string, float> state){
        myPos = pos;
        myRot = rot;
        bool jumping = true;
        if(state["jump"] < 0.5f){
            jumping = false;
        }
        anim.SetBool("Jump", jumping);
        anim.SetBool("Grounded", !jumping);
        anim.SetFloat("Speed", state["speed"]);
        
    }
    public void SetItems(string[] itemNames){
        for(int i = 0; i < itemNames.Length; i++){
            if(!itemNames[i].Equals("null")){
                items[i] = Instantiate((GameObject)Resources.Load("Items/" + itemNames[i]));
            }
        }
    }
}
