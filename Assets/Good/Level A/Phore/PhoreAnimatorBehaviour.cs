using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
[RequireComponent(typeof(PhoreBehaviour))]

public class PhoreAnimatorBehaviour : MonoBehaviour
{
    public bool orientation;
    private PhoreBehaviour phore;
    void Start()
    {
        phore = GetComponent<PhoreBehaviour>();
    }

    void Update()
    {
        float beat = MetaManagerBehaviour.metaManager.hypertrackManager.GetBeat();
        if (beat < 128 + 64) {
            bool isGo = (beat % 32 < 16);
            bool isWarn = (beat % 16 < 4);
            phore.state = (orientation ^ isGo) ? 
                (isWarn ? PhoreState.WarnBeforeStop : PhoreState.Stop) : 
                (isWarn ? PhoreState.WarnBeforeGo : PhoreState.Go);
        } else if (beat < 128 + 64 + 32) {
            phore.state = PhoreState.Off;
        } else {
            phore.state = PhoreState.Warn;
        }  
    }
}
