using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json; 

public class Player : Entity
{
    Rigidbody rb;
    public float _speed = 10;
    public float sensitiveRotate = 5.0f;
    public float jumpPower = 10.0f;
    public float speedMagnification = 10; //調整必要　例10
    public Vector3 movingVelocity;
    public WebSocket ws;
    public GameObject[] itemObjects = new GameObject[10];
    // public GameObject Item;
    // private ItemBase item;
    Vector3 movingDirecion;
    private bool isJumping = false;
    private Animator anim;
    private int sprinting = 1;
    private int jumpCount = 0;
    private float animSpeed = 0;
    private HotBar hotbar;
    private GameObject firePos;

    // Start is called before the first frame update
    protected override void init(){
        rb = this.GetComponent<Rigidbody>();
        anim = this.gameObject.GetComponent<Animator>();
        firePos = this.gameObject.transform.GetChild(1).gameObject;
        for(int i = 0; i < itemObjects.Length; i++){
            if(itemObjects[i]){
                items[i] = Instantiate(itemObjects[i]);
            }
        }
        // GameObject tmpItem = Instantiate(Item);
        // item = tmpItem.GetComponent<ItemBase>();
        
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
        movingVelocity = sprinting * movingDirecion * speedMagnification * 0.015f;
        
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

        if (Input.GetMouseButton(0)) {
            // item.UseItem(this.gameObject);
            if(items[hotbar.selectNum].GetComponent<ItemBase>().UseItem(this.gameObject)){
                Transform trans = firePos.transform;
                ItemUseData sendData = new ItemUseData{
                    id=myid,
                    method="useItem",
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
                    itemNum = hotbar.selectNum
                };
                ws.SendText(JsonConvert.SerializeObject(sendData));
            };
        }

    }

    void FixedUpdate() {

	    // rb.velocity = new Vector3(movingVelocity.x, rb.velocity.y, movingVelocity.z);
        Ray ray = new Ray(this.gameObject.transform.GetChild(0).gameObject.transform.position,
         new Vector3(movingVelocity.x, 0, movingVelocity.z));
        RaycastHit hit;
        if(!Physics.Raycast(ray, out hit, 0.5f))
        {
            rb.MovePosition(rb.position + new Vector3(movingVelocity.x, 0, movingVelocity.z));
        }

        for(int i = 0; i < items.Length; i++){
            if(items[i]){
                hotbar.SetSliderValue(i, (float)(items[i].GetComponent<ItemBase>().GetCoolDown()) / items[i].GetComponent<ItemBase>().GetMaxCoolDown());
            }
        }

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
        // Dictionary<string, string> sendData = new Dictionary<string, string>{
        //     {"method", "join"},
        //     {"name", name},
        // };
        List<string> itemNames = new List<string>();
        for(int i = 0; i < itemObjects.Length; i++){
            if(itemObjects[i]){
                itemNames.Add(itemObjects[i].GetComponent<ItemBase>().GetName());
            }
            else{
                itemNames.Add("null");
            }
        }
        JoinData sendData = new JoinData{
            id=myid,
            name=name,
            method="join",
            items = itemNames
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

    public void SetHotBar(GameObject hotbarObj){
        hotbar = hotbarObj.GetComponent<HotBar>();
    }
}
