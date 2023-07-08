using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaytimeManagerBehaviour : MonoBehaviour
{
    public MetaManagerBehaviour metaManagerBehaviour;
    public AnimationCurve nonlinearity;
    public float daytimeRatio = 0;
    public float nighttimeRatio = 0;
    void FixedUpdate() {
        float linearRatio = metaManagerBehaviour.hypertrackManager.source.time / metaManagerBehaviour.hypertrackManager.source.clip.length;
        linearRatio *= 0.8f;
        linearRatio += 0.75f -0.2f;
        linearRatio %= 1;
        daytimeRatio = nonlinearity.Evaluate(linearRatio);
        daytimeRatio %= 1;
        nighttimeRatio = (daytimeRatio + 0.5f) % 1;
    }
}
