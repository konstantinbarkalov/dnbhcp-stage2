// using UnityEditor;
// using UnityEngine;
// using UnityEngine.Serialization;
// using UnityEngine.UIElements;

// namespace Good.UI
// {
//     public class Editor : EditorWindow
//     {
        
//         [FormerlySerializedAs("m_VisualTreeAsset")] [SerializeField]
//         private VisualTreeAsset mVisualTreeAsset;

//         private int mClickCount = 0;

//         private const string MButtonPrefix = "button";

//         [MenuItem("Window/UI Toolkit/Editor")]
//         public static void ShowExample()
//         {
//             var wnd = GetWindow<Editor>();
//             wnd.titleContent = new GUIContent("Editor");
//         }

//         [MenuItem("File/KhKhKhKhKhKh")]
//         public static void KhKhKhKhKhKh()
//         {
//             var wnd = GetWindow<Editor>();
//             wnd.ShowPopup();
//         }

//         public void CreateGUI()
//         {
//             // Each editor window contains a root VisualElement object
//             var root = rootVisualElement;

//             // VisualElements objects can contain other VisualElements following a tree hierarchy.
//             var label = new Label("khkhkkhkh");
//             root.Add(label);

//             // Instantiate UXML
//             VisualElement labelFromUxml = mVisualTreeAsset.Instantiate();
//             root.Add(labelFromUxml);
            
//             //Call the event handler
//             SetupButtonHandler();
//         }
        
//         //Functions as the event handlers for your button click and number counts 
//         private void SetupButtonHandler()
//         {
//             VisualElement root = rootVisualElement;

//             var buttons = root.Query<Button>();
//             buttons.ForEach(RegisterHandler);
//         }

//         private void RegisterHandler(Button button)
//         {
//             button.RegisterCallback<ClickEvent>(PrintClickMessage);
//         }

//         private void PrintClickMessage(ClickEvent evt)
//         {
//             VisualElement root = rootVisualElement;

//             ++mClickCount;

//             //Because of the names we gave the buttons and toggles, we can use the
//             //button name to find the toggle name.
//             Button button = evt.currentTarget as Button;
//             string buttonNumber = button.name.Substring(MButtonPrefix.Length);
//             string toggleName = "toggle" + buttonNumber;
//             Toggle toggle = root.Q<Toggle>(toggleName);

//             Debug.Log("Button was clicked!" + (toggle.value ? " Count: " + mClickCount : ""));
//         }
        
//     }

// }
