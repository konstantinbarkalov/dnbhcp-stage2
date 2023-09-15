using System.Collections.Generic;
using Good.UI;
using UnityEngine;

public class MetaManagerBehaviour : MonoBehaviour
{
    public NavigationManager navigationManager;
    public HypertrackManagerBehaviour hypertrackManager;
    public DaytimeManagerBehaviour daytimeManager;
    public InputManagerBehaviour inputManagerBehaviour;
    public DaytimeEnvironmentManagerBehaviour daytimeEnvironmentManager;
    public ElectricNetworkManagerBehaviour electricNetworkManager;
    public DebugManagerBehaviour debugManager;

    static public MetaManagerBehaviour instance;

    private void Awake()
    {
        if (instance)
        {
            if (instance != this)
            {
                Destroy(this);
            }
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
}
