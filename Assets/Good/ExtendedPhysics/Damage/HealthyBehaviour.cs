using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthyBehaviour : MonoBehaviour
{
    public float healthRatio = 1;
    public bool IsDead() {
        return healthRatio <= 0;
    }
}
