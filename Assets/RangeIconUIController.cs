using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIconUIController : MonoBehaviour
{
    public GameObject RangeIconPrefab;
    //public List<GameObject> RangeIcons;

    private int _range;
    public int Range
    {
        get
        {
            return _range;
        }
        set
        {
            _range = value;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        // for (int i = 0; i < RangeIcons.Count; i++)
        // {
        //     if (i < _range)
        //     {
        //         RangeIcons[i].gameObject.SetActive(true);
        //     }
        //     else
        //     {
        //         RangeIcons[i].gameObject.SetActive(false);
        //     }
        // }
        int delta = _range - transform.childCount;
        
        if (delta > 0)
        {
            for (int i = 0; i < delta; i++)
            {
                Instantiate(RangeIconPrefab, transform);
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
