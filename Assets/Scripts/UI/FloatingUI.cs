using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    public float FloatScale = 2.0f;
    public float SpeedMultiplier = 0.5f;
   
    void Update()
    {
        float variatonInSize = Mathf.PingPong(Time.time * SpeedMultiplier, FloatScale) + 1;
        transform.localScale = new Vector3(variatonInSize, variatonInSize, variatonInSize);
    }
}
