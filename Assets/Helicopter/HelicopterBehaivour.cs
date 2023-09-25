using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterBehaivour : MonoBehaviour
{
    public Rigidbody rigidBody;
    void Start() {
        MetaManager.level.playerManager.player = this;
    }  
}
