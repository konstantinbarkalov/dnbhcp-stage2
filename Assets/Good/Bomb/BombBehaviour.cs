using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehaviour : MetaManagerSourceBehaviour
{
    public ExtendedPhysics.Explosion.ExplosionBehaviour explosionPrefab;
    public float force = 300;
    public BeaconBehaviour bombBeacon;
    public float exlpodeTime = 2f;
    public float alarmDuration = 60f/180f * 2f;
    public bool isAlarmed = false;
    void Start()
    {
        bombBeacon.mode = BeaconMode.On;
    }

    void FixedUpdate()
    {
        float alarmTime = (exlpodeTime - alarmDuration);
        if (!isAlarmed && Time.fixedTime > alarmTime ) {
            isAlarmed = true;
            bombBeacon.mode = BeaconMode.Flash;
        }
        if (Time.fixedTime > exlpodeTime) {
            var explosion = Instantiate<ExtendedPhysics.Explosion.ExplosionBehaviour>(explosionPrefab, transform.position, transform.rotation);
            explosion.explosionSphere.force=force;
            Destroy(this.gameObject);
        }
    }

}
