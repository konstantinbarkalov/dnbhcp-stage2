using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaManagerBehaviour : MonoBehaviour
{

    //public GameManagerBehaviour gameManager;
    // public DontDestroyOnLoadManager dontDestroyOnLoadManager = DontDestroyOnLoadManager;
    public HypertrackManagerBehaviour hypertrackManager;
    public DaytimeManagerBehaviour daytimeManager;
    public InputManagerBehaviour inputManagerBehaviour;
    public DaytimeEnvironmentManagerBehaviour daytimeEnvironmentManager;
    public ElectricNetworkManagerBehaviour electricNetworkManager;
    static public MetaManagerBehaviour metaManager;

    private void Awake()
    {
        MetaManagerBehaviour.metaManager = this;
    }

}

public static class DontDestroyOnLoadManager
{

    static List<GameObject> _ddolObjects = new List<GameObject>();

    public static void DontDestroyOnLoad(this GameObject go)
    {
        UnityEngine.Object.DontDestroyOnLoad(go);
        _ddolObjects.Add(go);
    }

    public static void DestroyAll()
    {
        foreach (var go in _ddolObjects)
        {
            if (go != null)
            {
                UnityEngine.Object.Destroy(go);
            }
        }

        _ddolObjects.Clear();
    }

}
