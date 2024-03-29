﻿using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using Random = UnityEngine.Random;

public class BeamController : MonoBehaviour
{
    [SerializeField] private List<Sprite> _beamSprites;
    [SerializeField] private List<Sprite> _floorSprites;

    [SerializeField] private SpriteRenderer _beamRenderer;
    [SerializeField] private SpriteRenderer _floorRenderer;

    [SerializeField] private float _cycleSpeed;
    [SerializeField] private int _cycles;
    [SerializeField] private Light _light;
    
    public int Order
    {
        get
        {
            return _beamRenderer.sortingOrder;
        }
        set
        {
            _beamRenderer.sortingOrder = value;
        }
    }

    public event Action OnFinished;
    public event Action OnDestroyed;

    /// <summary>
    /// Logic to animate the lighting beams
    /// </summary>
    /// <returns></returns>
    public void PlayVFX()
    {
        StartCoroutine(VFXLoop());
    }
    
    /// <summary>
    /// Logic to animate the lighting beams
    /// </summary>
    /// <returns></returns>
    private IEnumerator VFXLoop()
    {
        ServiceLocator.GetService<AudioManager>().PlaySound(Sound.Lightning, gameObject);
        
        //scope variables used for randomisation
        int beamRNG = 0;
        int floorRNG = 0;
        int colorRNG = 0;
        float lightRNG = 0f;
        _light.color =  GlobalGameAssets.Instance.HighlightColor;

        float lightStartIntensity = _light.intensity;
        float lightStartSize = _light.range;

        //For each defined number of cycles
        for (int i = 0; i < _cycles; i++)
        {
            //pick a random sprite
            beamRNG = Random.Range(0, _beamSprites.Count);
            floorRNG = Random.Range(0, _floorSprites.Count);

            if(_beamSprites!= null && _beamSprites.Count > 0) _beamRenderer.sprite = _beamSprites[beamRNG];
            _floorRenderer.sprite = _floorSprites[floorRNG];

            //pick a random color
            colorRNG = Random.Range(0,2);

            if (colorRNG == 0)
            {
                if(_beamSprites!= null && _beamSprites.Count > 0) _beamRenderer.color = Color.white;
            }
            else
            {
                if(_beamSprites!= null && _beamSprites.Count > 0)  _beamRenderer.color = GlobalGameAssets.Instance.HighlightColor;
            }
            
            colorRNG = Random.Range(0,2);

            if (colorRNG == 0)
            {
                _floorRenderer.color = Color.white;
            }
            else
            {
                _floorRenderer.color = GlobalGameAssets.Instance.HighlightColor;
            }

            lightRNG = Random.Range(0f, 0.5f);

            //Flash lights randomly
            _light.intensity = lightStartIntensity + lightRNG;
            _light.range = lightStartSize + lightRNG*4;
            
            //Wait time between cycles
            yield return new WaitForSeconds(_cycleSpeed);
        }
        OnFinished?.Invoke();
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}
