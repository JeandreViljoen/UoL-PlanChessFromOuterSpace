using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIconUIController : MonoBehaviour
{
    public GameObject RangeIconPrefab;

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
