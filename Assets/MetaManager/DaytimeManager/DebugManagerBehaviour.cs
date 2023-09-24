using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManagerBehaviour : AbstractAppManagerBehaviour
{
    public string text;
    void Update() {
        MetaManager.app.uIManager.debugInfo = text;
    }
}
