using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BASE_SETTINGS : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 300;
        Screen.orientation = ScreenOrientation.Portrait;
    }
}
