using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exproder : ItemBase
{
    public ParticleSystem exprodeParticle;
    public ParticleSystem bulletParticle;
    public GameObject magicBullet;
    // Start is called before the first frame update
    public override void Use(GameObject user){
        GameObject eye = user.transform.GetChild(1).gameObject;
        GameObject bullet = Instantiate(magicBullet, eye.transform.position, eye.transform.rotation);
        bullet.GetComponent<MagicBullet>().effect += Exprode;
        bullet.GetComponent<MagicBullet>().particle = bulletParticle;
    }

    protected override void init(){
        maxCooldown = 100;
    }

    public void Exprode(GameObject bullet){
        ParticleSystem newParticle = Instantiate(exprodeParticle);
        newParticle.transform.position = bullet.transform.position;
		newParticle.Play();
        Collider[] hitColliders = Physics.OverlapSphere(bullet.transform.position, 10);
        for(int i = 0; i < hitColliders.Length; i++){
            Rigidbody rb = hitColliders[i].GetComponent<Rigidbody>();
            if(rb){
                rb.AddExplosionForce(20, bullet.transform.position, 10, 0, ForceMode.Impulse);
            }
        }
    }

}