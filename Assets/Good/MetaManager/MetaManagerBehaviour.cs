using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaManagerBehaviour : MonoBehaviour
{
    //public GameManagerBehaviour gameManager;
    public HypertrackManagerBehaviour hypertrackManager;
    public DaytimeManagerBehaviour daytimeManager;
    public DaytimeEnvironmentManagerBehaviour daytimeEnvironmentManager;
    public ElectricNetworkManagerBehaviour electricNetworkManager;
    static public MetaManagerBehaviour metaManager;
    private void Awake() {
        MetaManagerBehaviour.metaManager = this;
    }
}
