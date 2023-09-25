using System;
using Unity.VisualScripting;
using UnityEngine;

public class TimeManagerBehaviour : MonoBehaviour
{
    private float baseTimeScale = 1;
    private float timeScaleFactor = 1;
    public void SetTimeScaleFactor(float timeScaleFactor) {
        this.timeScaleFactor = timeScaleFactor;
        UpdateTimeScale();
    }
    public void SetBaseTimeScale(float baseTimeScale) {
        this.baseTimeScale = baseTimeScale;
        UpdateTimeScale();
    }
    public float GetBaseTimeScale() {
        return baseTimeScale;
    } 
    private void UpdateTimeScale() {
        const float baseFixedDeltaTime = 1/50f;
        float timeScale = baseTimeScale * timeScaleFactor;
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = baseFixedDeltaTime * timeScale;
        MetaManager.level.hypertrackManager.source.pitch = timeScale;
    
    } 
    void Start() {
        float baseTimeScaleFromPrefs = PlayerPrefs.GetFloat("base-time-scale", 1);
        SetBaseTimeScale(baseTimeScaleFromPrefs);
    }
}
