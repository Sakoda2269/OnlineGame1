using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_1 : MonoBehaviour
{
    public float sensitiveRotate = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform mytrans = this.transform;
        float rotateY = Input.GetAxis("Mouse Y") * sensitiveRotate;
        float muki = mytrans.localEulerAngles.x;
        if(muki >= 180){
            muki -= 360;
        }
        if(muki >= -60 && rotateY > 0){
            mytrans.Rotate(-rotateY, 0, 0);
        }
        else if(muki <= 60 && rotateY < 0){
            mytrans.Rotate(-rotateY, 0, 0);
        }
    }
}
