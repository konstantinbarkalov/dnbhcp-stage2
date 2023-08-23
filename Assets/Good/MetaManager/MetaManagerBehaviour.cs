using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaManagerBehaviour : MonoBehaviour
{
    //public GameManagerBehaviour gameManager;
    public HypertrackManagerBehaviour hypertrackManager;
    public DaytimeManagerBehaviour daytimeManager;
    public DaytimeEnvironmentManagerBehaviour daytimeEnvironmentManager;
    static public MetaManagerBehaviour metaManager;
    private void Awake() {
        MetaManagerBehaviour.metaManager = this;
    }
}
