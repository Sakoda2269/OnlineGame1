using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

    Rigidbody rb;
    public float _speed = 10f;
    public float sensitiveRotate = 5.0f;
    public float jumpPower = 20.0f;
    private bool isJumping = false;
    public float speedMagnification = 10; //調整必要　例10
    public Vector3 movingVelocity;
    Vector3 movingDirecion;
    private Animator anim;
    private int sprinting = 1;
    private int jumpCount = 0;
    
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        anim = this.gameObject.GetComponent<Animator>();
    }

   void Update()
    {
        
        Transform mytrans = this.transform;
        float rotateX = Input.GetAxis("Mouse X") * sensitiveRotate;
        mytrans.Rotate(0.0f, rotateX, 0.0f);
        float x = 0;
        float z = 0;

        if (Input.GetKey(KeyCode.W))
        {
            z = _speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            x = -_speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            z = -_speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            x = _speed;
        }
        if(Input.GetKey(KeyCode.LeftControl))
        {
            sprinting = 2;
        }
        else
        {
            sprinting = 1;
        }
        if(x != 0 || z != 0)
        {
            if(sprinting == 2)
            {
                anim.SetFloat("Speed", 6.0f);
            }
            else
            {
                anim.SetFloat("Speed", 3.0f);
            }
        }
        else
        {
            anim.SetFloat("Speed", 0f);
        }
        

        movingDirecion = transform.rotation *  new Vector3(x, 0, z);
        movingDirecion.Normalize();//斜めの距離が長くなるのを防ぎます
        movingVelocity = sprinting * movingDirecion * speedMagnification;
        
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping) {
            rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
            jumpCount++;
            if(jumpCount >= 2){
                isJumping = true;
                jumpCount = 0;
            }
            anim.SetBool("Jump", true);
            anim.SetBool("Grounded", false);
	    }
    }

    void FixedUpdate() {

	    rb.velocity = new Vector3(movingVelocity.x, rb.velocity.y, movingVelocity.z);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Floor"))
        {
            isJumping = false;
            jumpCount = 0;
            anim.SetBool("Jump", false);
            anim.SetBool("Grounded", true);
        }
    }
}
