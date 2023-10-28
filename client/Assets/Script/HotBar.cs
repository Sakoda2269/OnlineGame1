using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotBar : MonoBehaviour
{
    int selectNum = 0;
    GameObject selected;
    // Start is called before the first frame update
    void Start()
    {
        selectNum = 0;
        selected = this.gameObject.transform.GetChild(0).gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {
        selected.transform.localPosition = new Vector3(-863 + selectNum * 192f, 0, 0);
        if(Input.GetAxis("Mouse ScrollWheel") > 0){
            selectNum += 1;
            selectNum %= 10;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0){
            selectNum -= 1;
            if(selectNum < 0){
                selectNum = 9;
            }
            selectNum %= 10;
        }
    }
}
