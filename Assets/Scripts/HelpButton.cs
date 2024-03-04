using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HelpButton : MonoBehaviour
{
    // Start is called before the first frame update

    
    void Start()
    {
        
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
public void Helptext()
    {
        Debug.Log("I am Help");
        gameObject.SetActive(!gameObject.activeSelf);
    }
    
}
