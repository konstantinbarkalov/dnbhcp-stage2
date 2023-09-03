using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private bool _visible;

    public bool Visible
    {
        get
        {
            return _visible;
        }
    }

    private void OnBecameInvisible() {
        _visible = false;
    }

    private void OnBecameVisible()
    {
        _visible = true;
    }
    
}
