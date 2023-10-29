using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leap : ItemBase
{
    public override void Use(GameObject user){
        Rigidbody rb = user.GetComponent<Rigidbody>();
        GameObject eye = user.transform.GetChild(0).gameObject;
        rb.AddForce((eye.transform.forward).normalized * 30f,
         ForceMode.Impulse);
    }
    protected override void init(){
        maxCooldown = 100;
    }
    public override string GetName(){
        return "Leap";
    }
}
