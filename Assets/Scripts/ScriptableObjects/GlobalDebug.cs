using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Global Debug Asset", menuName = "Custom Assets/Global Debug Asset")]
public class GlobalDebug : ScriptableObjectSingleton<GlobalDebug>
{
   public bool ShowIndexCodes;
   public bool ShowGameState;
}
