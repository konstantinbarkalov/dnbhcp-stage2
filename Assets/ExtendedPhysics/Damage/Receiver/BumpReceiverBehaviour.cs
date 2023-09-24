using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtendedPhysics.Damage;

namespace ExtendedPhysics.Damage.Receiver
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(DamageReceiverBehaviour))]
    public class BumpReceiverBehaviour : MonoBehaviour
    {
        private DamageReceiverBehaviour damageReceiver;
        private new Rigidbody rigidbody;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            damageReceiver = GetComponent<DamageReceiverBehaviour>();
        }

        void OnCollisionEnter(Collision collision)
        {
            float mass = rigidbody.mass;
            float dt = Time.fixedDeltaTime;
            float g = UnityEngine.Physics.gravity.magnitude;
            Vector3 kineticImpulse = collision.impulse;
            ExtendedPhysics.ExtendedPhysicsProperties extendedProperties = damageReceiver.extendedPhysicsProperties.extendedProperties;
            Rigidbody otherRigidbody = collision.rigidbody;
            ExtendedPhysics.ExtendedPhysicsProperties otherExtendedProperties = ExtendedPhysics.ExtendedPhysicsProperties.FromRigidbody(otherRigidbody);

            float hitCrossSquare = Mathf.Min(extendedProperties.AverageCrossSquare, otherExtendedProperties.AverageCrossSquare);
            float penetrationFactor = Mathf.Max(1, 1 / hitCrossSquare);
            float safeFreeFallDistance = 2;
            // free fall from 2m affects no damage
            // and (also funny and convenient) kills at 13m when resistance is 1g 
            float velocityChangeMagnitudeThreshold = ExtendedPhysics.Utils.CalculateFreeFallVelocity(safeFreeFallDistance, g, dt);
            float impulseMagnitudeThreshold = ExtendedPhysics.Utils.ConvertForce(velocityChangeMagnitudeThreshold, ForceMode.VelocityChange, ForceMode.Impulse, mass, dt);
            Damage damage = Damage.FromKineticImpulseWithThreshold(damageReceiver.extendedPhysicsProperties.extendedProperties, kineticImpulse * penetrationFactor, impulseMagnitudeThreshold, DamageType.Bump);
            damageReceiver.ReceiveDamage(damage);
        }
        void FixedUpdate()
        {

        }

    }
}