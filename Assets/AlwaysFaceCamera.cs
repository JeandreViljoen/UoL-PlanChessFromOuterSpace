using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AlwaysFaceCamera : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private Vector3 _cameraPos;
    private float _cameraAngle;

    public bool Tilt;
    public bool Rotate;
    public float RotateOffset = 1;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if (Rotate)
        {
            Vector3 cameraPos = Camera.main.transform.position;
            Vector3 OffsetPosition = Vector3.forward * RotateOffset;
            Vector3 lookPosition = cameraPos - OffsetPosition;
            
            transform.LookAt(lookPosition);
        }

        if (Tilt)
        {
            _cameraAngle = Camera.main.transform.rotation.eulerAngles.x;

            if (Rotate)
            {
                _cameraAngle *= -1;
            }
            transform.rotation = Quaternion.Euler(_cameraAngle, transform.rotation.eulerAngles.y ,0f);
        }
    }
}
