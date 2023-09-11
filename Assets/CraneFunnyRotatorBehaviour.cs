using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneFunnyRotatorBehaviour : MonoBehaviour
{
    public Transform anchor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float t = Time.fixedTime;
        anchor.localRotation = Quaternion.Euler(new Vector3(0, 0, t*5));
    }
}
