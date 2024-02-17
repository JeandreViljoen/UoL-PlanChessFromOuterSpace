using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Global Game Asset", menuName = "Custom Assets/New Global Game Asset")]
public class GlobalGameAssets : ScriptableObjectSingleton<GlobalGameAssets>
{
   [Header("Currency")]
   public int StartCurrency = 0;
}
