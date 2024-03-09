using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineRangeIcons : MonoBehaviour
{
    public LayoutGroup Layout;
    public List<Image> RangeIcons;

    private int _iconsToShow;
    public int IconsToShow
    {
        get
        {
            return _iconsToShow;
        }
        set
        {
            _iconsToShow = value;
            SetIcons(_iconsToShow);
        }
    }

    void Start()
    {
    }

    void Update()
    {
        
    }

    private void SetIcons(int value)
    {
        for (int i = 0; i < RangeIcons.Count; i++)
        {
            if (i < value)
            {
                RangeIcons[i].gameObject.SetActive(true);
            }
            else
            {
                RangeIcons[i].gameObject.SetActive(false);
            }
        }
    }
}
