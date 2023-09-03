using UnityEngine;

public class DamageCalculator : MonoBehaviour
{
    private const float VelocityDamageCost = 0.5f;
    public float CalculateDamage(Vector3 impulse)
    {
        return impulse.magnitude * VelocityDamageCost;

    }
    public float CalculateDamage(Vector3 explPos, float explRadius)
    {
        float distance = Vector3.Distance(explPos, transform.position);
        float distancePercent = distance * 100 / explRadius;
        float damage = 100 - distancePercent + 1;
        Debug.Log($"Distance is {distance}, radius is {explRadius}, percent is {distancePercent}, final damage is {damage}");
        return damage;
    }
}
