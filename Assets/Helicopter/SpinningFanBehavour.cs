using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningFanBehavour : MonoBehaviour
{
    public Vector3 axis;
    public Transform fan;
    public float angleVelocity = 180 * 3;
    private float angle = 0;

    void FixedUpdate()
    {
        angle += angleVelocity * Time.fixedDeltaTime;
        angle %= 360;
        fan.localRotation = Quaternion.Euler(axis * angle);
    }
}
