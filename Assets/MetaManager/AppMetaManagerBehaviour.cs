using UnityEngine;

public class AppMetaManagerBehaviour : MonoBehaviour
{
    public UI.UIManager uIManager;
    public InputManagerBehaviour inputManagerBehaviour;
    public DebugManagerBehaviour debugManager;

    private void Awake()
    {
        Initialize();
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
    private void Initialize() // TODO Move to some initialization Manager or some kind of app init 
    {

        Application.targetFrameRate = 300;
        Screen.orientation = ScreenOrientation.Portrait;
    }
}
