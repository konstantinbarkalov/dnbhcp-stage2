using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonBehaviour : MonoBehaviour
{
    public Renderer montgolfierRenderer;
    public Renderer torchFlameRenderer;
    public Light torchLight;
    public bool isTorchEnabled = true;
    public bool isFullThrottleEnabled = false;
    public bool isStartFly = false;
    private bool wasFullThrottleEnabled = false;
    [SerializeField]
    private float torchRatio;
    [SerializeField]
    private float starterIgnitionTorchRatio;
    [SerializeField]
    private float longtimeTorchRatio;
    [ColorUsageAttribute(false, true)]
    public Color torchColor = new Color(1, 0.5f, 0);
    public float torchLightIntensity = 50;
    public float montgolfierEmissionIntensity = 1;
    public float torchFlameEmissionIntensity = 4;
    [SerializeField]
    private float heatRatio;
    private Material montgolfierMaterial;
    private Material torchFlameMaterial;
    private Vector3 velocity;
    private void Start()
    {
        montgolfierMaterial = montgolfierRenderer.material;
        torchFlameMaterial = torchFlameRenderer.material; 
    }

    // Update is called once per frame
    private void Update()
    {
        bool isTorchActuallyEnabled = torchRatio > 0.001f;
        CalulateTorchRatio__Update();        
        Color montgolfierEmissionColor = torchColor * torchRatio * montgolfierEmissionIntensity;
        montgolfierMaterial.SetColor("_Emission_Color", montgolfierEmissionColor);
        
        torchLight.enabled = isTorchActuallyEnabled;
        torchLight.color = torchColor;
        torchLight.intensity = torchRatio * torchLightIntensity;
        
        torchFlameRenderer.enabled = isTorchActuallyEnabled;
        Color torchFlameEmissionColor = torchColor  * torchFlameEmissionIntensity * Mathf.Pow(torchRatio, 1/3);
        torchFlameMaterial.SetColor("_Emission_Color", torchFlameEmissionColor);
        torchFlameMaterial.SetFloat("_Scale", torchRatio);

        CalulateHeat__Update();
        montgolfierMaterial.SetFloat("_Depth_Strength", 1 + heatRatio * 5);
        montgolfierMaterial.SetFloat("_Inflate_Fat", heatRatio * 0.4f);
        
        Fly__Update();
        
    }
    private void Fly__Update()
    {
        if (isStartFly) 
        {
            float heightRatio  = Mathf.Pow(Mathf.Max(0, transform.position.y) / 150f, 2f);
            float mass = 500;
            float gravityAcceleration = -9.81f;
            float archimedRatio = 0.25f + heatRatio * 0.75f - heightRatio;
            float flyVerticalForce = 500 * 9.81f * archimedRatio * 3;
            float flyVerticalAcceleration = flyVerticalForce / mass;
            velocity.y += flyVerticalAcceleration * Time.deltaTime;
            velocity.y += gravityAcceleration  * Time.deltaTime;
            
            float unsignedAirFrictionForce = Mathf.Pow(velocity.y, 2) * 500;
            float unsignedMaxAirFrictionForce = Mathf.Abs(velocity.y) / Time.deltaTime * mass;
            float unsignedClippedAirFrictionForce = Mathf.Min(unsignedMaxAirFrictionForce, unsignedAirFrictionForce);
            float airFrictionForce = -Mathf.Sign(velocity.y) * unsignedClippedAirFrictionForce;
            float airFrictionAcceleration = airFrictionForce / mass;
            velocity.y += airFrictionAcceleration * Time.deltaTime;
            
            transform.position += velocity * Time.deltaTime;
            
            if (transform.position.y <= 0) {
                Vector3 position = transform.position;
                position.y = 0;
                transform.position = position;
                velocity.y = 0;
            }
        }
    }
    private void CalulateTorchRatio__Update()
    {
        float starterIgnitionTorchRatioImpulse = 4;
        float longtimePassiveTorchRatio = 0.2f;
        float longtimeFullThrottleTorchRatio = 1;
        float starterIgnitionSmoothFactorPerSec = 0.0005f; 
        float longtimeSmoothFactorPerSec = 0.1f; 
        
        bool isFullThrottleJustEnabled = isFullThrottleEnabled && !wasFullThrottleEnabled;
        if (isFullThrottleJustEnabled && isTorchEnabled) {
            starterIgnitionTorchRatio = starterIgnitionTorchRatioImpulse;
        } else {                
            float starterIgnitionSmoothFactorPerFrame = Mathf.Pow(starterIgnitionSmoothFactorPerSec, Time.deltaTime);
            starterIgnitionTorchRatio *= starterIgnitionSmoothFactorPerFrame;    
        }        
        float targetTorchRatio = isTorchEnabled ? (isFullThrottleEnabled ? longtimeFullThrottleTorchRatio : longtimePassiveTorchRatio) : 0;
        float longtimeSmoothFactorPerFrame = Mathf.Pow(longtimeSmoothFactorPerSec, Time.deltaTime);
        longtimeTorchRatio -= (longtimeTorchRatio - targetTorchRatio) * (1 - longtimeSmoothFactorPerFrame);
        
        wasFullThrottleEnabled = isFullThrottleEnabled;
        torchRatio = starterIgnitionTorchRatio + longtimeTorchRatio;
    }
    private void CalulateHeat__Update() 
    {
        float smoothFactorPerSec = 0.6f; 

        float smoothFactorPerFrame = Mathf.Pow(smoothFactorPerSec, Time.deltaTime);
        heatRatio -= (heatRatio - torchRatio) * (1 - smoothFactorPerFrame);         
    }

}
