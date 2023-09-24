using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ExtendedPhysics.Damage
{
    [System.Serializable]
    public enum DamageType
    {
        NativePhysicsRapidAcceleration,
        Bump,
        Slash,
        Explosion,
        Heat,
        Electric,
        Poison,
    }

    [System.Serializable]
    public enum DamageApplicationType
    {
        Mass,
        Volume,
        SurfaceSquare,
        EquatorSize,
        AverageCrossSquare,
        AverageCrossSize,
        CrossSquare,
        CrossSize,
        Identity,
    }

    [System.Serializable]
    public struct Damage
    {
        public Vector3 direction;
        public float amount;
        public DamageType type;
        public DamageApplicationType applicationType;


        public float GetDamageApplicationFactor(ExtendedPhysics.ExtendedPhysicsProperties extendedProperties)
        {
            switch (applicationType)
            {
                case DamageApplicationType.Mass:
                    return extendedProperties.Mass;
                case DamageApplicationType.Volume:
                    return extendedProperties.Volume;
                case DamageApplicationType.SurfaceSquare:
                    return extendedProperties.SurfaceSquare;
                case DamageApplicationType.EquatorSize:
                    return extendedProperties.EquatorSize;
                case DamageApplicationType.AverageCrossSize:
                    return extendedProperties.AverageCrossSize;
                case DamageApplicationType.AverageCrossSquare:
                    return extendedProperties.AverageCrossSquare;
                case DamageApplicationType.CrossSize:
                    return extendedProperties.GetCrossSizeTowardsDirection(direction);
                case DamageApplicationType.CrossSquare:
                    return extendedProperties.GetCrossSquareTowardsDirection(direction);
                case DamageApplicationType.Identity:
                default:
                    return 1;
            }
        }

        static public Damage FromVector(Vector3 vector, DamageType type, DamageApplicationType applicationType)
        {
            float vectorMagnitude = vector.magnitude;
            Vector3 vectorDirection = vector.normalized;
            Damage damage = new Damage();
            damage.amount = vectorMagnitude;
            damage.direction = vectorDirection;
            damage.type = type;
            damage.applicationType = applicationType;
            return damage;
        }

        static public Damage FromKineticImpulse(Vector3 impulse, DamageType type, DamageApplicationType applicationType = DamageApplicationType.Mass)
        {
            // just a wrapper actually, because all logic are same and no addings for now
            return FromVector(impulse, type, applicationType);
        }

        static public Damage FromKineticImpulseWithThreshold(ExtendedPhysics.ExtendedPhysicsProperties debug, Vector3 impulse, float impulseMagnitudeThreshold, DamageType type, DamageApplicationType applicationType = DamageApplicationType.Mass)
        {
            float impulseMagnitude = impulse.magnitude;
            Vector3 impulseDirection = impulse.normalized;
            float thresholdedImpulseMagnitude = Mathf.Max(0, impulseMagnitude - impulseMagnitudeThreshold);
            Vector3 thresholdedImpulse = thresholdedImpulseMagnitude * impulseDirection;

            float accelerationMagnitude = ExtendedPhysics.Utils.ConvertForce(impulseMagnitude, ForceMode.Impulse, ForceMode.Acceleration, debug.Mass, Time.fixedDeltaTime);
            float velocityChangeMagnitude = ExtendedPhysics.Utils.ConvertForce(impulseMagnitude, ForceMode.Impulse, ForceMode.VelocityChange, debug.Mass, Time.fixedDeltaTime);
            float forceMagnitude = ExtendedPhysics.Utils.ConvertForce(impulseMagnitude, ForceMode.Impulse, ForceMode.Force, debug.Mass, Time.fixedDeltaTime);

            float gMagnitude = accelerationMagnitude / 9.81f;

            //if (impulseMagnitude > 0) {
            // if (type == DamageType.Bump)
            // {
            //     Debug.Log("CREATING DAMAGE!" +
            //     "\r\n target rb: " + debug.Rigidbody.name +
            //     "\r\n impulse: " + impulseMagnitude +
            //     "\r\n acceleration: " + accelerationMagnitude +
            //     "\r\n velocityChange: " + velocityChangeMagnitude +
            //     "\r\n force: " + forceMagnitude +
            //     "\r\n gMagnitude: " + gMagnitude +
            //     "\r\n thresholdedImpulseMagnitude: " + thresholdedImpulseMagnitude +
            //     "\r\n type: " + type);
            // }
            return FromKineticImpulse(thresholdedImpulse, type, applicationType);
        }


    }

    [System.Serializable]
    public struct DamageResistanceSetupEntity
    {
        public DamageType type;
        public float factor;
    }

    [System.Serializable]
    public class DebugDamage
    {
        public float nativePhysicsRapidAcceleration;
        public float bump;
        public float explosion;
        public void Add(DamageType type, float amount)
        {
            if (type == DamageType.NativePhysicsRapidAcceleration) { nativePhysicsRapidAcceleration += amount; }
            else if (type == DamageType.Bump) { bump += amount; }
            else if (type == DamageType.Explosion) { explosion += amount; }
        }
    }

}