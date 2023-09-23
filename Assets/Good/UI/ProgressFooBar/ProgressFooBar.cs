using System.Diagnostics;
using UnityEngine.UIElements;

namespace Good.UI.ProgressFooBar
{
    internal class ProgressFooBar : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ProgressFooBar, UxmlTraits> {}

        // Must expose your element class to a { get; set; } property that has the same name 
        // as the name you set in your UXML attribute description with the camel case format
        public string MyString { get; set; }
        public int MyInt { get; set; }
        
        // Add the two custom UXML attributes.
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription mString =
                new UxmlStringAttributeDescription { name = "my-string", defaultValue = "default_value" };

            private readonly UxmlIntAttributeDescription mInt =
                new UxmlIntAttributeDescription { name = "my-int", defaultValue = 2 };
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as ProgressFooBar;

                Debug.Assert(ate != null, nameof(ate) + " != null");
                
                ate.MyString = mString.GetValueFromBag(bag, cc);
                ate.MyInt = mInt.GetValueFromBag(bag, cc);
            }
        }
    }

}
