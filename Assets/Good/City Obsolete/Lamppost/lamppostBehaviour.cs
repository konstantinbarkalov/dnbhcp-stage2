using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lamppostBehaviour : MonoBehaviour
{
    public HealthyBehaviour healthy;
    public MeshedLightBehaviour meshedLight;
    void FixedUpdate()
    {
        if (healthy.healthRatio < 0) {
            meshedLight.isEnabled = false;
        }
        
    }
}
