using UnityEngine;

public class LevelMetaManagerBehaviour : MonoBehaviour
{
    public HypertrackManagerBehaviour hypertrackManager;
    public DaytimeManagerBehaviour daytimeManager;
    public DaytimeEnvironmentManagerBehaviour daytimeEnvironmentManager;
    public ElectricNetworkManagerBehaviour electricNetworkManager;
    
    private void Awake()
    {
        if (MetaManager.level)
        {
            if (MetaManager.level != this)
            {
                Destroy(MetaManager.level);
            }
        }
        MetaManager.level = this;
    }
}
