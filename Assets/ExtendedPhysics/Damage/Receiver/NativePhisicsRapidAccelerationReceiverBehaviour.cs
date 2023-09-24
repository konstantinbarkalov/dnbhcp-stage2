using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtendedPhysics.Damage;

namespace ExtendedPhysics.Damage.Receiver
{

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(DamageReceiverBehaviour))]
    public class NativePhysicsRapidAccelerationReceiverBehaviour : MonoBehaviour
    {
        public DamageReceiverBehaviour damageReceiver;
        private new Rigidbody rigidbody;
        private Vector3 previousVelocity;
        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            previousVelocity = rigidbody.velocity;
        }
        void FixedUpdate()
        {
            float mass = rigidbody.mass;
            float dt = Time.fixedDeltaTime;
            float g = UnityEngine.Physics.gravity.magnitude;

            Vector3 velocityDifference = rigidbody.velocity - previousVelocity;
            Vector3 acceleration = (velocityDifference) / dt;
            if (rigidbody.useGravity) acceleration -= UnityEngine.Physics.gravity;
            Vector3 kineticImpulse = acceleration * mass * dt;

            // 10g at dt
            float safeAccelerationMagnitudeThreshold = 2 * g;
            float impulseMagnitudeThreshold = ExtendedPhysics.Utils.ConvertForce(safeAccelerationMagnitudeThreshold, ForceMode.Acceleration, ForceMode.Impulse, mass, dt);
            Damage damage = Damage.FromKineticImpulseWithThreshold(damageReceiver.extendedPhysicsProperties.extendedProperties, kineticImpulse, impulseMagnitudeThreshold, DamageType.NativePhysicsRapidAcceleration);
            damageReceiver.ReceiveDamage(damage);

            previousVelocity = rigidbody.velocity;
        }

    }
}