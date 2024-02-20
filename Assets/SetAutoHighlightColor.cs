using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAutoHighlightColor : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    void Start()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        if (_spriteRenderer!= null)
        {
            _spriteRenderer.color = GlobalGameAssets.Instance.HighlightColor;
        }
        
    }
    
    void Update()
    {
        
    }
}
