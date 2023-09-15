using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManagerBehaviour : MonoBehaviour
{
    public string text;
    public TMPro.TMP_Text debugText;
    void Update() {
        debugText.text = text;
    }
}
