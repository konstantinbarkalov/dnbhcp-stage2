using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum BeaconMode {
    On,
    Flash,
    Off
}
public class BeaconBehaviour : MonoBehaviour
{
    public MeshedLightBehaviour meshedLight;
    public BeaconMode mode = BeaconMode.Flash;
    public float phaseRatio = 0;
    public float speedFactor = 1;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool isBeaconEnabled = false;
        if (mode == BeaconMode.On) {
            isBeaconEnabled = true;
        } else if (mode == BeaconMode.Flash) {
            float beat = MetaManagerBehaviour.metaManager.hypertrackManager.source.time / 60 * 180 * speedFactor;
            isBeaconEnabled = (beat + phaseRatio * 4) % 4 < 1;
        } else if (mode == BeaconMode.Off) {
            isBeaconEnabled = false;
        }
        meshedLight.isEnabled = isBeaconEnabled;
    }
}
