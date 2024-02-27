using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetAutoHighlightColor : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Button _button;
    
    void Start()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _button = gameObject.GetComponent<Button>();

        if (_spriteRenderer!= null)
        {
            _spriteRenderer.color = GlobalGameAssets.Instance.HighlightColor;
        }

        if (_button!= null)
        {
            var colors = _button.colors;
            colors.normalColor = GlobalGameAssets.Instance.HighlightColor;
            _button.colors = colors;
        }
        
    }
    
    void Update()
    {
        
    }
}
