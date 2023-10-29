using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{

    protected int maxCooldown;
    protected int cooldown;
    protected GameObject parent;
    public string myname;

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

    public bool UseItem(GameObject user){
        if(cooldown <= 0){
            Use(user);
            cooldown = maxCooldown;
            return true;
        }
        return false;
    }
    public abstract void Use(GameObject user);
    protected abstract void init();
    public abstract string GetName();

    public int GetCoolDown(){
        return cooldown;
    }
    public int GetMaxCoolDown(){
        return maxCooldown;
    }
}
