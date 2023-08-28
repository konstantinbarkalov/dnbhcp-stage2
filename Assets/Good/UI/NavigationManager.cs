using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Good.UI {
    
    public class NavigationManager : MonoBehaviour
    {
        public UIDocument MainScreen;
        public UIDocument GameScreen;

        private VisualElement mainRoot;
        private VisualElement gameRoot;

        [Range(-1,1)]
        public float bratio;

        public string debugInfo;
        // public RectTransform positivePlate;
        // public RectTransform negativePlate;
        // public RectTransform neutralPlate;
        // public TMPro.TMP_Text text;

        private void Awake()
        {
            DontDestroyOnLoadManager.DontDestroyOnLoad(this.gameObject);

            mainRoot = MainScreen?.rootVisualElement;
            gameRoot = GameScreen?.rootVisualElement;

            if (mainRoot != null)
            {
                var playButton = mainRoot.Q<Button>("content-play-button");
                if (playButton != null)
                {
                    playButton.clicked += () => 
                    {
                        Debug.Log("Play Button Clicked");
                        // SceneManager.LoadScene("Level0");
                        SceneManager.LoadScene("Level A");
                        GoToGameScreen();
                    };
                }

                var stabilizationAngularCorrectionBudgetRatioSlider = mainRoot.Q<Slider>("stabilization-angular-correction-budget-ratio");
                if (stabilizationAngularCorrectionBudgetRatioSlider != null) {
                    stabilizationAngularCorrectionBudgetRatioSlider.value = GlobalVariables.Get<float>("stabilization-angular-correction-budget-ratio", 0.00001f);
                    stabilizationAngularCorrectionBudgetRatioSlider.RegisterValueChangedCallback(v => {
                        GlobalVariables.Set("stabilization-angular-correction-budget-ratio", Mathf.Max(v.newValue, 0.00001f));
                    });
                }

                var stabilizationGravityCorrectionBudgetRatioSlider = mainRoot.Q<Slider>("stabilization-gravity-correction-budget-ratio");
                if (stabilizationGravityCorrectionBudgetRatioSlider != null) {
                    stabilizationGravityCorrectionBudgetRatioSlider.value = GlobalVariables.Get<float>("stabilization-gravity-correction-budget-ratio", 0.00001f);
                    stabilizationGravityCorrectionBudgetRatioSlider.RegisterValueChangedCallback(v => {
                        GlobalVariables.Set("stabilization-gravity-correction-budget-ratio", Mathf.Max(v.newValue, 0.00001f));
                    });
                }

                var horizontalInputBudgetRatioSlider = mainRoot.Q<Slider>("horizontal-input-budget-ratio");
                if (horizontalInputBudgetRatioSlider != null) {
                    horizontalInputBudgetRatioSlider.value = GlobalVariables.Get<float>("horizontal-input-budget-ratio", 0.25f);
                    horizontalInputBudgetRatioSlider.RegisterValueChangedCallback(v => {
                        GlobalVariables.Set("horizontal-input-budget-ratio", Mathf.Max(v.newValue, 0.00001f));
                    });
                }

                // GoToMainScreen();
            }
            
            if (gameRoot != null) {
                var toMenuButton = gameRoot.Q<Button>("to-menu-button");
                if (toMenuButton != null)
                {
                    toMenuButton.clicked += () =>
                    {
                        Debug.Log("To Menu Button Clicked");
                        SceneManager.LoadScene("Main");
                        GoToMainScreen();
                    };
                }
                
                // GoToGameScreen();
            }

            GoToMainScreen();
        }
        
        // private void Start()
        // {
            // GoToMainScreen();
        // }

        void OnValidate() {
            UpdateGauge();
        }

        void Update() {
            UpdateGauge();
        }

        void UpdateGauge()
        {
            // float positiveRatio = Mathf.Max(0, bratio);
            // float negatveRatio = Mathf.Max(0, -bratio);
            // float neutralRatio = 1 - positiveRatio - negatveRatio;
            
            // Vector2 positivePlateAnchorMax = positivePlate.anchorMax;
            // positivePlateAnchorMax.x = positiveRatio;    
            // positivePlate.anchorMax = positivePlateAnchorMax;
            
            // Vector2 negativePlateAnchorMin = negativePlate.anchorMin;
            // negativePlateAnchorMin.x = 1 - negatveRatio;    
            // negativePlate.anchorMin = negativePlateAnchorMin;
            
            // Vector2 neutralPlateAnchorMax = neutralPlate.anchorMax;
            // neutralPlateAnchorMax.x = positiveRatio + neutralRatio;    
            // neutralPlate.anchorMax = neutralPlateAnchorMax;
            
            // Vector2 neutralPlateAnchorMin = neutralPlate.anchorMin;
            // neutralPlateAnchorMin.x = positiveRatio;    
            // neutralPlate.anchorMin = neutralPlateAnchorMin;
            
            // text.text = Mathf.Round(bratio * 100).ToString() + "%";

            var root = GameScreen.rootVisualElement;

            if (root != null)
            {
                var gauge = root.Q<ProgressBar>("gauge");
                if (gauge != null)
                {
                    gauge.value = bratio + 1f;
                }

                var debugTextArea = root.Q<Label>("debug-text-area");
                if (debugTextArea != null)
                {
                    debugTextArea.text = debugInfo;
                }
            }
        }

        void SetScreenEnableState(UIDocument screen, VisualElement root, bool finalState)
        {
            // var root = screen.rootVisualElement;

            if (finalState)
            {
                root.style.display = DisplayStyle.Flex;
            }
            else
            {
                root.style.display = DisplayStyle.None;
            }                

            screen.enabled = finalState;
            // screen.gameObject.GetComponent<UnityEventQueueSystem>().enabled = finalState;
            // screen.gameObject.GetComponent<EventSystem>().enabled = finalState;
        }

        private void GoToMainScreen()
        {
            SetScreenEnableState(MainScreen, mainRoot, true);
            SetScreenEnableState(GameScreen, gameRoot, false);
        }

        private void GoToGameScreen()
        {
            if (MainScreen != null && mainRoot != null) {
                SetScreenEnableState(MainScreen, mainRoot, false);
            }
            SetScreenEnableState(GameScreen, gameRoot, true);
        }
    }
    
}
