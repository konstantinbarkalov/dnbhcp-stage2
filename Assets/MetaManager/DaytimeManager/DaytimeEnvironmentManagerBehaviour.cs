using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaytimeEnvironmentManagerBehaviour : AbstractLevelManagerBehaviour
{
    public Light sunLight;
    public Light fillerLight;
    public AnimationCurve sunIntensityCurve;
    public AnimationCurve fillerIntensityCurve;
    public Gradient sunColorGradient;
    public Gradient fillerColorGradient;
    public Gradient dustColorGradient;
    public Gradient fogColorGradient;
    public AnimationCurve fogStartCurve;
    public AnimationCurve fogEndCurve;

    void FixedUpdate()
    {
        float daytimeRatio = MetaManager.level.daytimeManager.daytimeRatio;
        float nighttimeRatio = MetaManager.level.daytimeManager.nighttimeRatio;
        float sunAngle = Mathf.Lerp(0, 360, daytimeRatio) + 270;
        sunLight.transform.rotation = Quaternion.Euler(sunAngle, 0, 0);
        sunLight.intensity = sunIntensityCurve.Evaluate(nighttimeRatio) * 2f;
        sunLight.color = sunColorGradient.Evaluate(nighttimeRatio);
        fillerLight.intensity = fillerIntensityCurve.Evaluate(nighttimeRatio);
        fillerLight.color = fillerColorGradient.Evaluate(nighttimeRatio);
        RenderSettings.fogColor = fogColorGradient.Evaluate(nighttimeRatio);
        RenderSettings.fogStartDistance = fogStartCurve.Evaluate(nighttimeRatio) * 1000;
        RenderSettings.fogEndDistance = fogEndCurve.Evaluate(nighttimeRatio) * 1000;
        RenderSettings.ambientSkyColor = fillerColorGradient.Evaluate(nighttimeRatio) * fillerIntensityCurve.Evaluate(nighttimeRatio) * 2;
        RenderSettings.ambientEquatorColor = fogColorGradient.Evaluate(nighttimeRatio) * fillerIntensityCurve.Evaluate(nighttimeRatio) * 2;
        RenderSettings.ambientGroundColor = (fillerColorGradient.Evaluate(nighttimeRatio) + fogColorGradient.Evaluate(nighttimeRatio) + GetDustColor()) / 2;
        
    }

    public Color GetDustColor()
    {
        float nighttimeRatio = MetaManager.level.daytimeManager.nighttimeRatio;
        Color dustColor = fogColorGradient.Evaluate(nighttimeRatio);
        dustColor.r = Mathf.Pow(dustColor.r * 0.9f + +0.1f, 1 / 2f);
        dustColor.g = Mathf.Pow(dustColor.g * 0.9f + +0.1f, 1 / 2f);
        dustColor.b = Mathf.Pow(dustColor.b * 0.9f + +0.1f, 1 / 2f);
        return dustColor;
    }
}
