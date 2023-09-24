using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ExtendedPhysics.Damage.Receiver
{

[RequireComponent(typeof(HealthyBehaviour))]
[RequireComponent(typeof(ExtendedPhysicsPropertiesBehaviour))]   
public class DamageReceiverBehaviour : MonoBehaviour
{
    public HealthyBehaviour healthy;
    public ExtendedPhysicsPropertiesBehaviour extendedPhysicsProperties;
    public DamageResistanceSetupEntity[] damageResistanceSetup;
    private Dictionary<DamageType, float> damageResistances;
    public DebugDamage debugDamage = new DebugDamage();
    
    void Start() {
        healthy = GetComponent<HealthyBehaviour>();
        extendedPhysicsProperties = GetComponent<ExtendedPhysicsPropertiesBehaviour>();
        InitDamageResistancesFromSetup();
    }
    
    private void InitDamageResistancesFromSetup()
    {
        damageResistances = new Dictionary<DamageType, float>();
        foreach (DamageResistanceSetupEntity entity in damageResistanceSetup)
        {
            damageResistances.TryAdd(entity.type, entity.factor);
        }
        foreach (DamageType damageType in (DamageType[]) System.Enum.GetValues(typeof(DamageType)))
        {
            damageResistances.TryAdd(damageType, 1);
        }
    }

    public void ReceiveDamage(Damage damage) {
        float damageApplicationFactor = damage.GetDamageApplicationFactor(extendedPhysicsProperties.extendedProperties);
        float damageResistanceFactor = damageResistances[damage.type];
        debugDamage.Add(damage.type, damage.amount);
        healthy.healthRatio -= damage.amount / damageApplicationFactor / damageResistanceFactor;
    }    
}
}