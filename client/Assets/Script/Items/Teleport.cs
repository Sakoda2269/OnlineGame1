using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : ItemBase
{
    // Start is called before the first frame update
    public override void Use(GameObject user){
        GameObject eye = user.transform.GetChild(0).gameObject;
        Vector3 dir = user.transform.forward + eye.transform.forward;
        Ray ray = new Ray(eye.transform.position, dir);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            Rigidbody rb = user.GetComponent<Rigidbody>();
            rb.AddForce((hit.point - user.transform.position).normalized * 10.0f, ForceMode.Impulse);
        }
    }

    protected override void init(){
        maxCooldown = 200;
    }

    public override string GetName(){
        return "Teleport";
    }

}
