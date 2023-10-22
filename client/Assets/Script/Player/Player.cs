using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json; 

public class Player : Entity
{
    Rigidbody rb;
    public float _speed = 10f;
    public float sensitiveRotate = 5.0f;
    public float jumpPower = 10.0f;
    private bool isJumping = false;
    public float speedMagnification = 5; //調整必要　例10
    public Vector3 movingVelocity;
    Vector3 movingDirecion;
    private Animator anim;
    private int sprinting = 1;
    private int jumpCount = 0;
    private float animSpeed = 0;

    // Start is called before the first frame update
    protected override void init(){
        rb = this.GetComponent<Rigidbody>();
        anim = this.gameObject.GetComponent<Animator>();
    }
    protected override void ExtraUpdate(){
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
        if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift))
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
                animSpeed = 6.0f;
            }
            else
            {
                animSpeed = 3.0f;
            }
        }
        else
        {
            animSpeed = 0;
        }
        anim.SetFloat("Speed", animSpeed);
        

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

    public void UpdatePos(WebSocket ws){
        Transform trans = this.transform;
        float jumping = 0f;
        if(jumpCount >= 1){
            jumping = 1.0f;
        }
        EntityData sendData = new EntityData{
            id=myid,
            method="update",
            pos = new Dictionary<string, float>{
                {"x", trans.position.x},
                {"y", trans.position.y},
                {"z", trans.position.z}
            },
            rotate = new Dictionary<string, float>{
                {"x", trans.localEulerAngles.x},
                {"y", trans.localEulerAngles.y},
                {"z", trans.localEulerAngles.z}
            },
            state = new Dictionary<string, float>{
                {"speed", animSpeed},
                {"jump", jumping},
            }
        };
        ws.SendText(JsonConvert.SerializeObject(sendData));
    }

    public void Join(WebSocket ws){
        Dictionary<string, string> sendData = new Dictionary<string, string>{
            {"method", "join"},
            {"name", name}
        };
        ws.SendText(JsonConvert.SerializeObject(sendData));
    }

    public void Leave(WebSocket ws){
        Dictionary<string, string> sendData = new Dictionary<string, string>{
            {"method", "leave"},
            {"id", myid}
        };
        ws.SendText(JsonConvert.SerializeObject(sendData));
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
