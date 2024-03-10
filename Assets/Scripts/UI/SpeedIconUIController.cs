using System.Collections;
using System.Collections.Generic;
using AutoLayout3D;
using UnityEngine;

public class SpeedIconUIController : MonoBehaviour
{
    public GameObject SpeedIconPrefab;
    //private LayoutGroup3D _layout;
    //public List<GameObject> SpeedIcons;

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
    
    void Start()
    {
        //_layout = GetComponent<LayoutGroup3D>();
    }

    void Update()
    {
        
    }

    void UpdateUI()
    {
        
        // for (int i = 0; i < SpeedIcons.Count; i++)
        // {
        //     if (i < _speed)
        //     {
        //         SpeedIcons[i].gameObject.SetActive(true);
        //     }
        //     else
        //     {
        //         SpeedIcons[i].gameObject.SetActive(false);
        //     }
        // }
        
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
