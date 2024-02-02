using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanChess
{
    [CreateAssetMenu(fileName = "Global Game Asset", menuName = "Custom Assets/New Global Game Asset")]
    public class GlobalGameAssets : ScriptableObjectSingleton<GlobalGameAssets>
    {
        public string test;
        //Asset data goes here
    }
}

