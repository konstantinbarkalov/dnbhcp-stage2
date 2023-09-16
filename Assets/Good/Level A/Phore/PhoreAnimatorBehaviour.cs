using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PhoreBehaviour))]

public class PhoreAnimatorBehaviour : MonoBehaviour
{
    public bool orientation;
    public int electricLaneIdx;
    private PhoreBehaviour phore;
    void Start()
    {
        phore = GetComponent<PhoreBehaviour>();
    }
    private PhoreState GetRegularState() {
        float beat = MetaManager.level.hypertrackManager.GetBeat();
        bool isGo = (beat % 32 < 16);
        bool isWarn = (beat % 16 < 4);
        return (orientation ^ isGo) ? 
            (isWarn ? PhoreState.WarnBeforeStop : PhoreState.Stop) : 
            (isWarn ? PhoreState.WarnBeforeGo : PhoreState.Go);
    }
    void Update()
    {
        float laneRatio = MetaManager.level.electricNetworkManager.GetLaneRatio(electricLaneIdx);
        if (laneRatio == 0) {
            phore.state = PhoreState.Warn;
        } else if (laneRatio < 1) {
            phore.state = PhoreState.Error;
        } else {
            phore.state = GetRegularState();
        }
          
    }
}
