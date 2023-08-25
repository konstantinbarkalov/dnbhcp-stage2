using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct SuperMeshedLightMaterialSetup {
    public Renderer mesh;
    public int materialIdx;
    public float emission;
    public bool useAlpha;
    [HideInInspector]
    public Material material;
}

[System.Serializable]
public struct SuperMeshedLightLightSetup {
    public Light light;
    public float intensity;
}
public class SuperMeshedLightBehaviour : MonoBehaviour
{
    public SuperMeshedLightMaterialSetup[] materialSetups;
    public Renderer[] meshSetups;
    public SuperMeshedLightLightSetup[] lightSetups;
    public float ratio;   
    void Start() {
        for (int meshIdx = 0; meshIdx < materialSetups.Length; meshIdx++)
        {
            Renderer mesh = materialSetups[meshIdx].mesh;
            int materialIdx = materialSetups[meshIdx].materialIdx;
            Material material = mesh.materials[materialIdx];
            materialSetups[meshIdx].material = material;
        }
    }
    void Update()
    {
        foreach (SuperMeshedLightMaterialSetup materialSetup in materialSetups)
        {
            string key = materialSetup.useAlpha ? "_Alpha" : "_Emission";
            materialSetup.material.SetFloat(key, ratio * materialSetup.emission);
        }
        foreach (SuperMeshedLightLightSetup lightSetup in lightSetups)
        {
            lightSetup.light.intensity = ratio * lightSetup.intensity;
            lightSetup.light.enabled = ratio > 0;
        }
        foreach (Renderer meshSetup in meshSetups)
        {
            meshSetup.enabled = ratio > 0;
        }
    }
}
