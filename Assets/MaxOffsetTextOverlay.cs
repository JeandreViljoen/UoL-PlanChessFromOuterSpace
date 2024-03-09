using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;

public class MaxOffsetTextOverlay : MonoBehaviour
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
        _text.text = "Max Node Offset:    " + ServiceLocator.GetService<UnitOrderTimelineController>()._maxNodeOffset.ToString();
    }
}
