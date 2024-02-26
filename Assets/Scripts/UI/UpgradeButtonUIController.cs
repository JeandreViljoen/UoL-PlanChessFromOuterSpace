using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeButtonUIController : MonoBehaviour
{
    public UpgradeButton SpeedButton;
    public UpgradeButton RangeButton;
    public UpgradeButton Button3;

    private EasyService<CameraManager> _cameraManager;

    void Start()
    {
        // _cameraManager.Value.OnCameraFocus += Show;
        _cameraManager.Value.OnCameraTopDown += Hide;
        Hide();
    }


    void Update()
    {

    }

    public void Show()
    {
        RefreshCosts();
        
        SpeedButton.gameObject.SetActive(true);
        RangeButton.gameObject.SetActive(true);
        Button3.gameObject.SetActive(true);
    }

    public void Hide()
    {
        SpeedButton.gameObject.SetActive(false);
        RangeButton.gameObject.SetActive(false);
        Button3.gameObject.SetActive(false);
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


    private void OnDestroy()
    {
        _cameraManager.Value.OnCameraTopDown -= Hide;
    }

    public void RefreshCosts()
    {
        SpeedButton.Cost = (GlobalGameAssets.Instance.CurrencyBalanceData.UpgradeSpeedCost);
        RangeButton.Cost = (GlobalGameAssets.Instance.CurrencyBalanceData.UpgradeRangeCost);
        Button3.Cost = (GlobalGameAssets.Instance.CurrencyBalanceData.UpgradeSpecialCost);
    }

}

public enum ButtonType
{
    SPEED,
    RANGE,
    BUTTON3
}
