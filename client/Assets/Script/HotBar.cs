using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotBar : MonoBehaviour
{
    public int selectNum = 0;
    GameObject selected;
    // public GameObject[] itemObjects = new GameObject[10];
    // private GameObject[] items = new GameObject[10];
    public Slider[] sliders = new Slider[10];

    // Start is called before the first frame update
    void Start()
    {
        selectNum = 0;
        selected = this.gameObject.transform.GetChild(0).gameObject;
        // for(int i = 0; i < 10; i++){
        //     if(itemObjects[i] == null){
        //         continue;
        //     }
        //     items[i] = Instantiate(itemObjects[i]);
        // }
        
    }

    // Update is called once per frame
    void Update()
    {
        selected.transform.localPosition = new Vector3(-863 + selectNum * 192f, 0, 0);
        if(Input.GetAxis("Mouse ScrollWheel") < 0){
            selectNum += 1;
            selectNum %= 10;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") > 0){
            selectNum -= 1;
            if(selectNum < 0){
                selectNum = 9;
            }
            selectNum %= 10;
        }

    }

    void FixedUpdate(){
        // for(int i = 0; i < 10; i++){
        //     if(sliders[i]){
        //         sliders[i].value = (float)(items[i].GetComponent<ItemBase>().GetCoolDown()) / items[i].GetComponent<ItemBase>().GetMaxCoolDown();
        //     }
        // }
    }

    public void SetSliderValue(int sliderNum, float value){
        sliders[sliderNum].value = value;
    }

    // public bool Use(GameObject user){
    //     if(items[selectNum] == null){
    //         return false;
    //     }
    //     return items[selectNum].GetComponent<ItemBase>().UseItem(user);
    // }

}
