using ExtendedPhysics;
using UnityEngine;
namespace Crowd.Zombie
{
public class DamageCalculator : MonoBehaviour
{
    private const float impulseDamageCost = 0.25f;
    public float CalculateDamage(Vector3 impulse)
    {
        return impulse.magnitude * impulseDamageCost;

    }
    public float CalculateDamage(float mass, float force, Vector3 explosionPosition, float explosionRadius, ForceMode forceMode)
    {
        float explosionInpulse = Utils.ConvertForce(force, forceMode, ForceMode.Impulse, mass, Time.fixedDeltaTime);
        Vector3 diff = transform.position - explosionPosition; 
        float dist = diff.magnitude;
        Vector3 dir = diff.normalized;
        float distanceRatio = dist / explosionRadius;
        Vector3 damageImpulse = explosionInpulse * (1 - distanceRatio) * dir;
        float damage = CalculateDamage(damageImpulse);
        Debug.Log($"Distance is {dist}, radius is {explosionRadius}, distanceRatio is {distanceRatio}, final damage is {damage}");
        return damage;
    }
}
}