using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshedLightBehaviour : MonoBehaviour
{
    public Renderer mesh;
    public new Light light;
    public bool isEnabled;
    private bool _isEnabled;
    
    public void Switch(bool isEnabled)
    {
        mesh.enabled = isEnabled;
        light.enabled = isEnabled;
    }
    
    void Start() {
        Switch(isEnabled);
    }
    void Update()
    {
        if (isEnabled != _isEnabled) {
            _isEnabled = isEnabled;
            Switch(isEnabled);
        }
    }
}
