using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SetAutoHighlightColor : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Image _image;
    private Button _button;

    [Range(0f,1f)] public float OverrideOpacity = 1f;
    
    void Start()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _button = gameObject.GetComponent<Button>();
        _image = gameObject.GetComponent<Image>();

        if (_spriteRenderer!= null)
        {
            _spriteRenderer.color = GlobalGameAssets.Instance.HighlightColor;
            _spriteRenderer.DOFade(OverrideOpacity, 0.00001f).SetUpdate(true);
        }

        if (_button!= null)
        {
            var colors = _button.colors;
            colors.normalColor = GlobalGameAssets.Instance.HighlightColor;
            _button.colors = colors;
        }
        
        if (_image!= null)
        {
            _image.color = GlobalGameAssets.Instance.HighlightColor;
            _image.DOFade(OverrideOpacity, 0.00001f).SetUpdate(true);
        }

        
    }
    
    void Update()
    {
        
    }
}
