using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AirFictionBehaivour : MonoBehaviour
{
    public float airFrictionFactor = 500;
    private Rigidbody rigidBody;
    void Awake() {
        rigidBody = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        float airFrictionForceMagnitude = rigidBody.velocity.sqrMagnitude * airFrictionFactor * Time.fixedDeltaTime;
        float airFrictionForceMagnitudeLimit = rigidBody.velocity.magnitude / Time.fixedDeltaTime * rigidBody.mass;
        float limitedAirFrictionForceMagnitude = Mathf.Min(airFrictionForceMagnitudeLimit, airFrictionForceMagnitude);
        Vector3 airFrictionForce = -limitedAirFrictionForceMagnitude * rigidBody.velocity.normalized;
        rigidBody.AddForce(airFrictionForce);
    }
}
