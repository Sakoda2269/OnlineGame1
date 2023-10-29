using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{

    protected int maxCooldown;
    protected int cooldown;
    protected GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        cooldown = 0;
        init();
    }

    // Update is called once per frame
    void Update()
    {
        if(cooldown > 0){
            Debug.Log(cooldown);
            cooldown--;
        }
    }

    public void SetParent(GameObject user){
        parent = user;
    }

    public void UseItem(GameObject user){
        if(cooldown <= 0){
            Use(user);
            cooldown = maxCooldown;
        }
    }
    public abstract void Use(GameObject user);
    protected abstract void init();

    public int GetCoolDown(){
        return cooldown;
    }
    public int GetMaxCoolDown(){
        return maxCooldown;
    }
}
