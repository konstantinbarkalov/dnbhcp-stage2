using UnityEngine;
using UnityEngine.InputSystem;
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
        var isRestartPressed = Keyboard.current.rKey.isPressed;
        // var isRestartPressed = Input.GetKeyDown(KeyCode.R);
        if (isRestartPressed) {
            SceneManager.LoadScene( SceneManager.GetActiveScene().name );
        }
    }
}
