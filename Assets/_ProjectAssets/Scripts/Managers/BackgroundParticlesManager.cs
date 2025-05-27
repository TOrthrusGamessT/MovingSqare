using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BackgroundParticlesManager : MonoBehaviour
{
    public Color defColor, actionColor, deathColor;
    public ParticleSystem[] children;
    private int _tween;

    #region Singleton

    public static BackgroundParticlesManager instance;

    private void Awake()
    {
        instance = FindObjectOfType<BackgroundParticlesManager>();

        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    public async void TakeCoinAnimation()
    {
        foreach (var child in children)
        {
            _tween = LeanTween.value(0, 1, 1f).setOnUpdate((value) =>
            {
                try
                {
                    child.GetComponent<Renderer>().material.color =
                        Color.Lerp(child.GetComponent<Renderer>().material.color, actionColor, value);
                }
                catch (Exception e)
                {
                    ClearTween();
                }
            }).id;

            await Task.Delay(2000);

            _tween = LeanTween.value(0, 1, 1500f).setOnUpdate((value) =>
            {
                try
                {
                    child.GetComponent<Renderer>().material.color =
                        Color.Lerp(child.GetComponent<Renderer>().material.color, defColor, value);
                }
                catch (Exception e)
                {
                    ClearTween();
                }

            }).id;
        }
    }

    public async void TakeDamageAnimation()
    {
        foreach (var child in children)
        {
            _tween = LeanTween.value(0, 1, 1f).setOnUpdate((value) =>
            {
                try
                {
                    child.GetComponent<Renderer>().material.color =
                        Color.Lerp(child.GetComponent<Renderer>().material.color, deathColor, value);
                }
                catch (Exception e)
                {
                    ClearTween();
                }

            }).id;

            await Task.Delay(2000);

            _tween = LeanTween.value(0, 1, 1500f).setOnUpdate((value) =>
            {
                try
                {
                    child.GetComponent<Renderer>().material.color =
                        Color.Lerp(child.GetComponent<Renderer>().material.color, defColor, value);
                }
                catch (Exception e)
                {
                    ClearTween();
                }

            }).id;
        }
    }

    private void ClearTween()
    {
        LeanTween.cancel(_tween);
    }

    void OnDestroy()
    {
        LeanTween.cancel(this.gameObject);
    }

}
