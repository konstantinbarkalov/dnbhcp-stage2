using System.Diagnostics;
using UnityEngine.UIElements;

namespace UI.Gauge
{
    
    public class Gauge : ProgressBar
    {
        
        public new class UxmlFactory : UxmlFactory<Gauge, UxmlTraits> { }

        // public new class UxmlTraits : BaseFieldTraits<bool, UxmlBoolAttributeDescription> { }

        private readonly VisualElement mBRatioInput;
        
        public Gauge()
        {
            lowValue = 0f;
            highValue = 2f;
        }

        // public new class UxmlTraits : BindableElement.UxmlTraits
        // {
        //     private readonly UxmlFloatAttributeDescription mBRatio;
        //     
        //     public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        //     {
        //         base.Init(ve, bag, cc);
        //         ProgressBar bRatioInput = ve as ProgressBar;
        //         Debug.Assert(bRatioInput != null, nameof(bRatioInput) + " != null");
        //         bRatioInput.value = mBRatio.GetValueFromBag(bag, cc) + 1f;
        //     }
        //     
        //     public UxmlTraits()
        //     {
        //         UxmlFloatAttributeDescription attributeDescription3 = new UxmlFloatAttributeDescription
        //         {
        //             name = "bRatio",
        //             defaultValue = 0.0f
        //         };
        //         mBRatio = attributeDescription3;
        //     }
        // }

        public new class UxmlTraits : ProgressBar.UxmlTraits
        {
            private readonly UxmlFloatAttributeDescription mBRatio;
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ProgressBar bRatioInput = ve as ProgressBar;
                Debug.Assert(bRatioInput != null, nameof(bRatioInput) + " != null");
                bRatioInput.value = mBRatio.GetValueFromBag(bag, cc) + 1f;
            }
            
            public UxmlTraits()
            {
                UxmlFloatAttributeDescription attributeDescription3 = new UxmlFloatAttributeDescription
                {
                    name = "bRatio",
                    defaultValue = 0.0f
                };
                mBRatio = attributeDescription3;
            }
        }
        
    }
    
}
