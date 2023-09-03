using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    protected GameObject poolObject;
    protected GameObject timedObject;
    protected List<GameObject> poolList = new List<GameObject>();

    //Protected for any inheritance ideas
    
    public ObjectPool(GameObject poolObject)
    {
        this.poolObject = poolObject;
    }

    public GameObject GetObjectPoolObject()
    {
        if(poolList.Count > 0)
        {
            return GetFromPool();
        }
        else
        {
            return GetNewObject();
        }
    }


    protected GameObject GetFromPool()
    {
        timedObject = poolList[0];
        poolList.RemoveAt(0);
        timedObject.SetActive(true);
        return timedObject;
    }

    protected GameObject GetNewObject()
    {
        return Instantiate(poolObject);
    }

    public void AddToPool(GameObject objToPool)
    {
        poolList.Add(objToPool);
        objToPool.transform.parent = null;
        objToPool.SetActive(false);
    }
}
