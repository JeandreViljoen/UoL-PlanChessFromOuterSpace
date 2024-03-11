using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;

public class OffsetTextOverlay : MonoBehaviour
{
    private TextMeshProUGUI _text;
    void Start()
    {
        if (!GlobalDebug.Instance.ShowNodeOffsetValues)
        {
            gameObject.SetActive(false);
            return;
        }
        _text = GetComponent<TextMeshProUGUI>();
    }
    
    void Update()
    {
        if (!GlobalDebug.Instance.ShowNodeOffsetValues)
        {
            gameObject.SetActive(false);
            return;
        }
        _text.text = "Node Offset:    " + ServiceLocator.GetService<UnitOrderTimelineController>().NodeOffset.ToString();
    }
}
