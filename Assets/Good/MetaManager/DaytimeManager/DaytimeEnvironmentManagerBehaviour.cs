using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaytimeEnvironmentManagerBehaviour : MonoBehaviour
{
    public DaytimeManagerBehaviour daytimeManager;
    public Light sunLight;
    public Light fillerLight;
    public AnimationCurve sunIntensityCurve;
    public AnimationCurve fillerIntensityCurve;
    public Gradient sunColorGradient;
    public Gradient fillerColorGradient;
    void FixedUpdate() 
    {
        float daytimeRatio = daytimeManager.daytimeRatio;
        float nighttimeRatio = daytimeManager.nighttimeRatio;
        float sunAngle = Mathf.Lerp(0, 360, daytimeRatio) + 270; 
        sunLight.transform.rotation = Quaternion.Euler(sunAngle, 0, 0);
        sunLight.intensity = sunIntensityCurve.Evaluate(nighttimeRatio) * 2f;
        sunLight.color = sunColorGradient.Evaluate(nighttimeRatio);
        fillerLight.intensity = fillerIntensityCurve.Evaluate(nighttimeRatio);
        fillerLight.color = fillerColorGradient.Evaluate(nighttimeRatio);
    }
}
