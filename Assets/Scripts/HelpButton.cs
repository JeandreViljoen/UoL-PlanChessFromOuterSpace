using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HelpButton : MonoBehaviour
{
    public void Helptext()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    
}
