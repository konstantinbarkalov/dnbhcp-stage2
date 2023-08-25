using UnityEngine;

namespace Good.UI.Core.JoyStick
{

    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class FloatingJoystick : MonoBehaviour
    {
        [HideInInspector]
        public RectTransform RectTransform;
        public RectTransform Knob;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }
    }

}
