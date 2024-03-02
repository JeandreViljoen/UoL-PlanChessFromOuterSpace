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

    public int SpeedUpgrades = 0;
    public int RangeUpgrades = 0;

    private EasyService<CameraManager> _cameraManager;

    void Start()
    {
        SpeedButton.EventHandler.OnMouseDown += RefreshCosts;
        RangeButton.EventHandler.OnMouseDown += RefreshCosts;
        Button3.EventHandler.OnMouseDown += RefreshCosts;
        
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
        Button3.gameObject.SetActive(false);
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

    public void RefreshCosts(PointerEventData _ = null)
    {
        CurrencyBalanceData cData = GlobalGameAssets.Instance.CurrencyBalanceData;
        
        SpeedButton.Cost = cData.UpgradeSpeedCost + cData.UpgradeCostIncrease * SpeedUpgrades;
        RangeButton.Cost = cData.UpgradeRangeCost + cData.UpgradeCostIncrease * RangeUpgrades;
        Button3.Cost = cData.UpgradeSpecialCost;
    }

}

public enum ButtonType
{
    SPEED,
    RANGE,
    BUTTON3
}
