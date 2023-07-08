using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtendedPhysics
{
[RequireComponent(typeof(Rigidbody))]
public class ExtendedPhysicsPropertiesBehaviour : MonoBehaviour
{
    //public Vector3 crossSize;
    public ExtendedPhysics.ExtendedPhysicsProperties extendedProperties;
    public bool isAutomatic = true;
    void Awake() {
        if (isAutomatic) 
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            extendedProperties = new ExtendedPhysics.ExtendedPhysicsProperties(rigidbody);
        }
    }
}
}