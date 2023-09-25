using UnityEngine;

public class LevelMetaManagerBehaviour : MonoBehaviour
{
    public HypertrackManagerBehaviour hypertrackManager;
    public DaytimeManagerBehaviour daytimeManager;
    public DaytimeEnvironmentManagerBehaviour daytimeEnvironmentManager;
    public ElectricNetworkManagerBehaviour electricNetworkManager;
    public PlayerManagerBehaviour playerManager;
    public BombManagerBehaviour bombManager;
    public ScoreManagerBehaviour scoreManager;
    public TimeManagerBehaviour timeManager;
    
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
