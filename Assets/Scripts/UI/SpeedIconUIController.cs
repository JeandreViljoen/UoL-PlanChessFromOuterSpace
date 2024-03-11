using System.Collections;
using System.Collections.Generic;
using AutoLayout3D;
using UnityEngine;

public class SpeedIconUIController : MonoBehaviour
{
    public GameObject SpeedIconPrefab;

    private int _speed;
    public int Speed
    {
        get
        {
            return _speed;
        }
        set
        {
            _speed = value;
            UpdateUI();
        }
    }
    
    void UpdateUI()
    {
        int delta = _speed - transform.childCount;
        
        if (delta > 0)
        {
            for (int i = 0; i < delta; i++)
            {
                Instantiate(SpeedIconPrefab, transform);
            }
        }
        else if (delta < 0)
        {
            for (int i = 0; i < (delta*-1); i++)
            {
                Destroy(transform.GetChild(transform.childCount-1).gameObject);
            }
        }
    }
    
    
}
