using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtendedPhysics.Damage.Receiver
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(DamageReceiverBehaviour))]
    public class ExplosionReceiverBehaviour : MonoBehaviour
    {
        public DamageReceiverBehaviour damageReceiver;
        private new Rigidbody rigidbody;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            damageReceiver = GetComponent<DamageReceiverBehaviour>();
        }

        public virtual void AddExplosionForce(
            float explosionForce,
            Vector3 explosionPosition,
            float explosionRadius,
            float upwardsModifier,
            ForceMode forceMode
        )
        {
            float sailFactor = damageReceiver
                .extendedPhysicsProperties
                .extendedProperties
                .AverageCrossSize; // to make force proportional to area (cuz initialy its proportional by mass)
            float sailedExplosionForce = explosionForce * sailFactor;
            //Debug.Log("boom " + explosionForce + " " + explosionPosition + " sf " + sailFactor);

            Vector3 centerPosition = rigidbody.centerOfMass;

            // TODO: redo to contact position
            Vector3 difference = centerPosition - explosionPosition;
            float distance = difference.magnitude;
            Vector3 direction = difference.normalized;
            Vector3 vectoredExplosionForce = sailedExplosionForce * direction;
            float unclampedProximityRatio = 1 - distance / explosionRadius;
            float proximityRatio = Mathf.Clamp01(unclampedProximityRatio);
            Vector3 attenuatedExplosionForce = vectoredExplosionForce * proximityRatio;

            float mass = rigidbody.mass;
            float dt = Time.fixedDeltaTime;

            // Making force **for damage** to be always a pure impulse. Independed of input ForceMode.
            Vector3 kineticImpulse = ExtendedPhysics.Utils.ConvertForce(
                attenuatedExplosionForce,
                forceMode,
                ForceMode.Force,
                mass,
                dt
            );
            float accelerationMagnitudeThreshold = 1; // TODO
            float impulseMagnitudeThreshold = ExtendedPhysics.Utils.ConvertForce(
                accelerationMagnitudeThreshold,
                ForceMode.Acceleration,
                ForceMode.Impulse,
                mass,
                dt
            );
            Damage damage = Damage.FromKineticImpulseWithThreshold(
                damageReceiver.extendedPhysicsProperties.extendedProperties,
                kineticImpulse,
                impulseMagnitudeThreshold,
                DamageType.Explosion,
                DamageApplicationType.Mass
            );
            damageReceiver.ReceiveDamage(damage);

            rigidbody.AddExplosionForce(
                sailedExplosionForce,
                explosionPosition,
                explosionRadius,
                upwardsModifier,
                forceMode
            );
        }

        private Vector3 AnyForceTypeToPureImpulse(
            Vector3 force,
            ForceMode forceMode,
            float mass,
            float dt
        )
        {
            float massFactor = 1;
            float timeFactor = 1;
            switch (forceMode)
            {
                case ForceMode.Acceleration:
                case ForceMode.VelocityChange:
                    massFactor = mass;
                    break;
                case ForceMode.Force:
                case ForceMode.Impulse:
                    massFactor = 1;
                    break;
            }
            switch (forceMode)
            {
                case ForceMode.Force:
                case ForceMode.Acceleration:
                    timeFactor = dt;
                    break;
                case ForceMode.Impulse:
                case ForceMode.VelocityChange:
                    timeFactor = 1;
                    break;
            }
            float factor = massFactor * timeFactor;
            Vector3 impulse = force * factor;
            return impulse;
        }
    }
}
