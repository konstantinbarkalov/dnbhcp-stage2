using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Crowd
{

    public class Pool 
    {
        protected GameObject prefab;
        protected List<GameObject> poolList = new List<GameObject>();

        //Protected for any inheritance ideas

        public Pool(GameObject prefab)
        {
            this.prefab = prefab;
        }



        protected GameObject GetFromPool()
        {
            GameObject gameObject = poolList[0];
            poolList.RemoveAt(0);
            gameObject.SetActive(true);
            return gameObject;
        }

        protected GameObject GetNewInstance()
        {
            return Object.Instantiate(prefab);
        }

        public GameObject Get()
        {
            if (poolList.Count > 0)
            {
                return GetFromPool();
            }
            else
            {
                return GetNewInstance();
            }
        }

        public void Put(GameObject gameObject)
        {
            poolList.Add(gameObject);
            gameObject.transform.parent = null;
            gameObject.SetActive(false);
        }
    }
}