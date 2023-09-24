using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace UI.Core.Buttons
{

    public class BaseButton : Button
    {
        
        // [FormerlySerializedAs("m_VisualTreeAsset")] [SerializeField]
        // private VisualTreeAsset mVisualTreeAsset;

        // public BaseButton()
        // {
        //     VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Core/Buttons/BaseButton.uxml");
        //     StyleSheet uss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI/Core/Buttons/BaseButton.uss");
        //     
        //     mVisualTreeAsset.Instantiate();
        // }
        
        public new class UxmlFactory : UxmlFactory<BaseButton, UxmlTraits>
        {
        }

        public new class UxmlTraits : TextElement.UxmlTraits
        {
            public UxmlTraits() => focusable.defaultValue = true;
        }
        
    }

}