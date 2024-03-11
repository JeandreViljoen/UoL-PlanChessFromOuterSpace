using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineSpeedIcons : MonoBehaviour
{
    public LayoutGroup Layout;
    public List<Image> SpeedIcons;

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

    private void SetIcons(int value)
    {
        for (int i = 0; i < SpeedIcons.Count; i++)
        {
            if (i < value)
            {
                SpeedIcons[i].gameObject.SetActive(true);
            }
            else
            {
                SpeedIcons[i].gameObject.SetActive(false);
            }
        }
    }
}
