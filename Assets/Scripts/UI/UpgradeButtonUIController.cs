using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeButtonUIController : MonoBehaviour
{
    public MouseEventHandler SpeedButton;
    public MouseEventHandler RangeButton;
    public MouseEventHandler Button3;

    void Start()
    {

    }


    void Update()
    {

    }

//     public void SubscribeToButtonPress(ButtonType type, Action<PointerEventData> callback)
//     {
//         switch (type)
//         {
//             case ButtonType.SPEED:
//                 SpeedButton.OnMouseDown += callback;
//                 break;
//             case ButtonType.RANGE:
//                 RangeButton.OnMouseDown += callback;
//                 break;
//             case ButtonType.BUTTON3:
//                 Button3.OnMouseDown += callback;
//                 break;
//             default:
//                 throw new ArgumentOutOfRangeException(nameof(type), type, null);
//         }
//     }
// }

}

public enum ButtonType
{
    SPEED,
    RANGE,
    BUTTON3
}
