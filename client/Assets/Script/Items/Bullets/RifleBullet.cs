using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleBullet : MonoBehaviour
{
    public float speed = 10f;
    private int deadTime;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        deadTime = 1000;
        rb = this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // this.gameObject.transform.position += speed * this.gameObject.transform.forward;
        rb.AddForce(speed * this.gameObject.transform.forward);
        deadTime--;
        if(deadTime <= 0){
            Destroy(this.gameObject);
        }
    }
}
