using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestarterBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var isRestartPressed = Input.GetKeyDown(KeyCode.R);
        if (isRestartPressed) {
            SceneManager.LoadScene( SceneManager.GetActiveScene().name );
        }
    }
}
