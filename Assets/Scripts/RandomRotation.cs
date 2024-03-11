using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public float DegreeIncrement;

    private int _options;
    
    void Start()
    {
        _options = (int)(360 / DegreeIncrement);

        int rng = UnityEngine.Random.Range(0 , _options);
        
        transform.Rotate(Vector3.forward, rng*DegreeIncrement);
    }
}

