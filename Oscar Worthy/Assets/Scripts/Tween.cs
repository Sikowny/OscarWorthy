using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tween : IUpdateable
{
    private float timer;
    private float endTime;

    private float value;
    private float startValue;
    private float endValue;

    private Interpolator interpolator;

    private Action<float> onUpdate;
    private Action onFinish;

    public Tween() { }

    public enum Interpolator
    {
        Linear,
        Cosine,
        EaseInCosine
    }

    public void StartTween(Action<float> onUpdate, float startVal, float endVal, float duration, Interpolator interpolator, Action onFinish = null)
    {
        if (active) return;

        active = true;

        timer = 0f;
        endTime = duration;

        startValue = startVal;
        endValue = endVal;

        this.interpolator = interpolator;

        this.onUpdate = onUpdate;
        this.onFinish = onFinish;

        GameManager.UpdateManager.Subscribe(this);
    }

    public bool active = false;
    // Update is called once per frame
    public void Update(float deltaTime)
    {
        timer += deltaTime;
        if (timer >= endTime)
        {
            onUpdate(endValue);
            Finish();
        }
        else
        {
            onUpdate(Interpolate(interpolator, startValue, endValue, endTime, timer));
        }
    }

    public void Cancel()
    {
        Finish();
    }

    private void Finish()
    {
        active = false;
        GameManager.UpdateManager.Unsubscribe(this);
        if (onFinish != null) onFinish();
    }

    private float Interpolate(Interpolator interpolator, float startValue, float endValue, float endTime, float currTime)
    {
        float valueMultiplier = endValue - startValue;
        float currTimePercent = currTime / endTime;

        float currValue;
        switch (interpolator)
        {
            case Interpolator.Linear:
                currValue = valueMultiplier * currTimePercent + startValue;
                return currValue;
            case Interpolator.Cosine:
                float currCosInterpolation = 0.5f * Mathf.Cos(currTimePercent * Mathf.PI + Mathf.PI) + 0.5f;
                currValue = valueMultiplier * currCosInterpolation + startValue;
                return currValue;
            case Interpolator.EaseInCosine:
                float currSinInterpolation = currTimePercent < 0.5f ?    
                    0.5f * Mathf.Sin(currTimePercent * Mathf.PI) :
                    0.5f * Mathf.Sin(currTimePercent * Mathf.PI + Mathf.PI) + 1;
                currValue = valueMultiplier * currSinInterpolation + startValue;
                return currValue;

        }

        return 0;
    }
}
