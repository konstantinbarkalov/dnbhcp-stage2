using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadlampBehaviour : MonoBehaviour
{
    public int electricLaneIdx;
    public SuperMeshedLightBehaviour superMeshedLight;
        
    // Update is called once per frame
    void Update()
    {
        superMeshedLight.ratio = MetaManager.level.electricNetworkManager.GetLaneRatio(electricLaneIdx);
    }
}
