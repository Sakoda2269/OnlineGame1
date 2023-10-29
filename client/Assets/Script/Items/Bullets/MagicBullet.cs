using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class MagicBullet : MonoBehaviour
{
    public Action<GameObject> effect;
    public float speed = 100.0f;
    Rigidbody rb;
    private int deadTime = 1000;
    public ParticleSystem particle;
    private ParticleSystem newParticle;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        if(particle != null){
            newParticle = Instantiate(particle);
		    newParticle.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = speed * this.transform.forward;
        deadTime--;
        if(deadTime <= 0){
            Destroy(this.gameObject);
            Destroy(newParticle);
        }
        if(newParticle != null){
            newParticle.transform.position = this.gameObject.transform.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Floor"))
        {
            effect.Invoke(this.gameObject);
            Destroy(newParticle);
            Destroy(this.gameObject);
        }
    }


}
