using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : ItemBase
{
    public GameObject bullet;
    // Start is called before the first frame update
    protected override void init(){
        maxCooldown = 5;
    }
    public override void Use(GameObject user){
        Vector3 pos =  user.transform.GetChild(0).gameObject.transform.position 
        + 0.8f * user.transform.GetChild(0).gameObject.transform.forward;
        Instantiate(bullet, pos, user.transform.GetChild(0).gameObject.transform.rotation);
    }

}
