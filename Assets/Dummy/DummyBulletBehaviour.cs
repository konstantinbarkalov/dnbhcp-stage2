using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DummyBulletBehaviour : MonoBehaviour
{
    public float impulseMagnitude;
    new Rigidbody rigidbody;
    void Start()
    {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForce(Vector3.right* impulseMagnitude, ForceMode.Impulse);
    }
}
