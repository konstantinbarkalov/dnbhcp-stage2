using UnityEngine;
using UnityEngine.UIElements;
using Debug = System.Diagnostics.Debug;

namespace UI.SlideToggle
{
    // Derives from BaseField<bool> base class. Represents a container for its input part.
    public class SlideToggle : BaseField<bool>
    {
        public new class UxmlFactory : UxmlFactory<SlideToggle, UxmlTraits> { }

        public new class UxmlTraits : BaseFieldTraits<bool, UxmlBoolAttributeDescription> { }

        // In the spirit of the BEM standard, the SlideToggle has its own block class and two element classes. It also
        // has a class that represents the enabled state of the toggle.
        public const string USSClassName = "slide-toggle";
        public const string InputUssClassName = "slide-toggle__input";
        public const string InputKnobUssClassName = "slide-toggle__input-knob";
        public const string InputCheckedUssClassName = "slide-toggle__input--checked";

        private readonly VisualElement mInput;
        private readonly VisualElement mKnob;

        // Custom controls need a default constructor. This default constructor calls the other constructor in this
        // class.
        public SlideToggle() : this(null) { }

        // This constructor allows users to set the contents of the label.
        public SlideToggle(string label) : base(label, null)
        {
            // Style the control overall.
            AddToClassList(USSClassName);

            // Get the BaseField's visual input element and use it as the background of the slide.
            mInput = this.Q(className: BaseField<bool>.inputUssClassName);
            mInput.AddToClassList(InputUssClassName);
            Add(mInput);

            // Create a "knob" child element for the background to represent the actual slide of the toggle.
            mKnob = new();
            mKnob.AddToClassList(InputKnobUssClassName);
            mInput.Add(mKnob);

            // There are three main ways to activate or deactivate the SlideToggle. All three event handlers use the
            // static function pattern described in the Custom control best practices.

            // ClickEvent fires when a sequence of pointer down and pointer up actions occurs.
            RegisterCallback<ClickEvent>(OnClick);
            // KeydownEvent fires when the field has focus and a user presses a key.
            RegisterCallback<KeyDownEvent>(OnKeydownEvent);
            // NavigationSubmitEvent detects input from keyboards, gamepads, or other devices at runtime.
            RegisterCallback<NavigationSubmitEvent>(OnSubmit);
        }

        static void OnClick(ClickEvent evt)
        {
            var slideToggle = evt.currentTarget as SlideToggle;
            Debug.Assert(slideToggle != null, nameof(slideToggle) + " != null");
            slideToggle.ToggleValue();

            evt.StopPropagation();
        }

        static void OnSubmit(NavigationSubmitEvent evt)
        {
            var slideToggle = evt.currentTarget as SlideToggle;
            Debug.Assert(slideToggle != null, nameof(slideToggle) + " != null");
            slideToggle.ToggleValue();

            evt.StopPropagation();
        }

        static void OnKeydownEvent(KeyDownEvent evt)
        {
            var slideToggle = evt.currentTarget as SlideToggle;

            // NavigationSubmitEvent event already covers keydown events at runtime, so this method shouldn't handle
            // them.
            Debug.Assert(slideToggle != null, nameof(slideToggle) + " != null");
            if (slideToggle.panel?.contextType == ContextType.Player)
                return;

            // Toggle the value only when the user presses Enter, Return, or Space.
            if (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.Space)
            {
                slideToggle.ToggleValue();
                evt.StopPropagation();
            }
        }

        // All three callbacks call this method.
        void ToggleValue()
        {
            value = !value;
        }

        // Because ToggleValue() sets the value property, the BaseField class dispatches a ChangeEvent. This results in a
        // call to SetValueWithoutNotify(). This example uses it to style the toggle based on whether it's currently
        // enabled.
        public override void SetValueWithoutNotify(bool newValue)
        {
            base.SetValueWithoutNotify(newValue);

            //This line of code styles the input element to look enabled or disabled.
            mInput.EnableInClassList(InputCheckedUssClassName, newValue);
        }
    }
    
}
