using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Join : MonoBehaviour
{
    public TMP_InputField name;
    public TMP_InputField adress;
    public static string nameIn;
    public static string adressIn;
    Button btn;
    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if(name.text.Equals("") || adress.text.Equals("")){
            btn.interactable = false;
        }else{
            btn.interactable = true;
        }
    }

    public void OnClick(){
        nameIn = name.text;
        adressIn = adress.text;
        SceneManager.LoadSceneAsync("City");
    }
}
