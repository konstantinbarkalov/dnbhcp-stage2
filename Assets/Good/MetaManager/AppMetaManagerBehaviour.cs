using UnityEngine;

public class AppMetaManagerBehaviour : MonoBehaviour
{
    public Good.UI.UIManager uIManager;
    public InputManagerBehaviour inputManagerBehaviour;
    public DebugManagerBehaviour debugManager;

    private void Awake()
    {
        if (MetaManager.app)
        {
            if (MetaManager.app != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            MetaManager.app = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
