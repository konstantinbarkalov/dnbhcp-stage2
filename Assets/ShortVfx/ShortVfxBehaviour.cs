using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortVfxBehaviour : MonoBehaviour
{
    public float duration = 3f;
    void Start()
    {
        Destroy(this.gameObject, duration);
    } 
}
