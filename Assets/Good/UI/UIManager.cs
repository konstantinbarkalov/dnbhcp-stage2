using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Good.UI {
    
    public class UIManager : AbstractAppManagerBehaviour
    {
        public UIDocument MainScreen;
        public UIDocument GameScreen;
        [Range(-1,1)]
        public float helicopterPowerBratio;
        public VectoredFanControllerModeRatios controlModeRatios;
        public VectoredFanControllerMode controlMode;
        
        public string debugInfo;

        private VisualElement mainRoot;
        private VisualElement gameRoot;
        private ProgressBar helicopterPowerGauge;
        private ProgressBar helicopterStuntGauge;
        private ProgressBar helicopterTravelGauge;
        private ProgressBar helicopterBrakeGauge;
        private Label debugTextArea;
        private EnumField controlModeField;

        private void InitMain()
        {
            // TODO: let's remove null checks, memorize nodes and become fail-fast instad of fail-safe
            if (mainRoot != null)
            {
                var playButton = mainRoot.Q<Button>("content-play-button");
                if (playButton != null)
                {
                    playButton.clicked += () =>
                    {
                        // Debug.Log("Play Button Clicked");
                        SceneManager.LoadScene("Level A");
                        GoToGameScreen();
                    };
                }

                var stabilizationAngularCorrectionBudgetRatioSlider = mainRoot.Q<Slider>("stabilization-angular-correction-budget-ratio");
                if (stabilizationAngularCorrectionBudgetRatioSlider != null)
                {
                    stabilizationAngularCorrectionBudgetRatioSlider.value = PlayerPrefs.GetFloat("stabilization-angular-correction-budget-ratio", 0.00001f);
                    stabilizationAngularCorrectionBudgetRatioSlider.RegisterValueChangedCallback(v =>
                    {
                        PlayerPrefs.SetFloat("stabilization-angular-correction-budget-ratio", Mathf.Max(v.newValue, 0.00001f));
                    });
                }

                var stabilizationGravityCorrectionBudgetRatioSlider = mainRoot.Q<Slider>("stabilization-gravity-correction-budget-ratio");
                if (stabilizationGravityCorrectionBudgetRatioSlider != null)
                {
                    stabilizationGravityCorrectionBudgetRatioSlider.value = PlayerPrefs.GetFloat("stabilization-gravity-correction-budget-ratio", 0.00001f);
                    stabilizationGravityCorrectionBudgetRatioSlider.RegisterValueChangedCallback(v =>
                    {
                        PlayerPrefs.SetFloat("stabilization-gravity-correction-budget-ratio", Mathf.Max(v.newValue, 0.00001f));
                    });
                }

                var horizontalInputBudgetRatioSlider = mainRoot.Q<Slider>("horizontal-input-budget-ratio");
                if (horizontalInputBudgetRatioSlider != null)
                {
                    horizontalInputBudgetRatioSlider.value = PlayerPrefs.GetFloat("horizontal-input-budget-ratio", 0.25f);
                    horizontalInputBudgetRatioSlider.RegisterValueChangedCallback(v =>
                    {
                        PlayerPrefs.SetFloat("horizontal-input-budget-ratio", Mathf.Max(v.newValue, 0.00001f));
                    });
                }

                var initialControlType = PlayerPrefs.GetString("control-type", "keybord-control");
                var namesOfControlTypes = new List<string> { "keybord-control", "joystick-control", "sliders-control", "arrows-control" };

                SelectableList selectableList = new(namesOfControlTypes.ConvertAll(id => new SelectableItem(id, initialControlType == id)));

                Dictionary<string, Button> controlTypeButtonsMap = namesOfControlTypes.ToDictionary(
                    item => item,
                    item => mainRoot.Q<Button>(item)
                );

                controlTypeButtonsMap[initialControlType]?.AddToClassList("selected");

                selectableList.Changed += (s, e) =>
                {
                    // Debug.Log("=== selectableList changed:");
                    e.Items.ForEach(item =>
                    {
                        // Debug.Log(item.ID + " " + item.IsSelected);
                        Button button = controlTypeButtonsMap[item.ID];
                        if (item.IsSelected)
                        {
                            PlayerPrefs.SetString("control-type", item.ID);
                            // Debug.Log(button + " add selected class");
                            button.AddToClassList("selected");
                        } else {
                            // Debug.Log(button + " remove selected class");
                            button.RemoveFromClassList("selected");
                        }
                    });
                };

                foreach (KeyValuePair<string, Button> entry in controlTypeButtonsMap)
                {
                    entry.Value.clicked += () =>
                    {
                        // Debug.Log("control type button clicked: " + entry.Key);
                        selectableList.Toggle(entry.Key);
                    };
                }

                // GoToMainScreen();
            }
        }

        private void InitGame()
        {
            // Debug.Log("gameRoot " + gameRoot);
            if (gameRoot != null)
            {
                var toMenuButton = gameRoot.Q<Button>("to-menu-button");
                // Debug.Log("toMenuButton " + toMenuButton);
                if (toMenuButton != null)
                {
                    toMenuButton.clicked += () =>
                    {
                        // Debug.Log("To Menu Button Clicked");
                        SceneManager.LoadScene("Level S");
                        GoToMainScreen();
                    };
                }

                // GoToGameScreen();
            }
        }

        private bool isInitialized = false; // TODO это норм?

        public void Init()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                Debug.Log("INIT NavigationManager");
                InitMain();
                InitGame();
            }
        }

        private void Awake()
        {
            mainRoot = MainScreen.rootVisualElement;
            gameRoot = GameScreen.rootVisualElement;
            helicopterPowerGauge = gameRoot.Q<ProgressBar>("helicopter-power-gauge");
            helicopterStuntGauge = gameRoot.Q<ProgressBar>("helicopter-stunt-gauge");
            helicopterTravelGauge = gameRoot.Q<ProgressBar>("helicopter-travel-gauge");
            helicopterBrakeGauge = gameRoot.Q<ProgressBar>("helicopter-brake-gauge");
            debugTextArea = gameRoot.Q<Label>("debug-text-area");
            controlModeField = gameRoot.Q<EnumField>("control-mode");
            Init();
            GoToMainScreen();
        }
        

        void OnValidate() {
            // TODO: do we need UpdateGauge() here? 
            //UpdateGauge();
        }

        void Update() {
            UpdateGauge();
        }

        void UpdateGauge()
        {
            helicopterPowerGauge.value = Math.Abs(helicopterPowerBratio);
            helicopterStuntGauge.value = controlModeRatios.stunt;
            helicopterTravelGauge.value = controlModeRatios.travel;
            helicopterBrakeGauge.value = controlModeRatios.brake;
            controlModeField.value = controlMode;
        }

        void SetScreenEnableState(VisualElement root, bool finalState)
        {
            if (finalState)
            {
                root.style.display = DisplayStyle.Flex;
                // root.style.visibility = Visibility.Visible;
                root.StretchToParentSize();
            }
            else
            {
                root.style.display = DisplayStyle.None;
                // root.style.visibility = Visibility.Hidden;
            }
        }

        private void GoToMainScreen()
        {
            if (MainScreen != null)
            {
                SetScreenEnableState(mainRoot, true);
            }
            if (GameScreen != null)
            {
                SetScreenEnableState(gameRoot, false);
            }
        }

        private void GoToGameScreen()
        {
            if (MainScreen != null && mainRoot != null)
            {
                SetScreenEnableState(mainRoot, false);
            }
            if (GameScreen != null)
            {
                SetScreenEnableState(gameRoot, true);
            }
        }
    }
    
}

public record SelectableItem(
    string ID,
    bool IsSelected
);

public class SelectableItemsChangedEvent
{

    public readonly List<SelectableItem> Items;

    public SelectableItemsChangedEvent(List<SelectableItem> items) {
        Items = items;
    }

}

public class SelectableList
{
    
    public readonly Dictionary<string, SelectableItem> SelectableItems;
    private readonly bool MultipleChoice;
    private readonly bool IsRadio;

    public delegate void SelectableItemsChangedEventHandler(object sender, SelectableItemsChangedEvent e);

    public event SelectableItemsChangedEventHandler Changed;

    public SelectableList(List<SelectableItem> selectableItems, bool multipleChoice = false, bool isRadio = true)
    {
        SelectableItems = selectableItems.ToDictionary(
            item => item.ID,
            item => item
        );
        MultipleChoice = multipleChoice;
        IsRadio = isRadio;
    }

    public void Toggle(string itemId)
    {
        Toggle(SelectableItems[itemId]);
    }

    public void Toggle(SelectableItem item)
    {
        bool nextIsSelectedValue = !item.IsSelected || (!MultipleChoice && IsRadio);

        List<SelectableItem> changedSelectableItems = new();

        if (!MultipleChoice && nextIsSelectedValue)
        {
            foreach (var id in SelectableItems.Keys.ToList())
            {
                SelectableItem nextSelectableItemValue = new(id, id == item.ID);
            
                if (nextSelectableItemValue.IsSelected != SelectableItems[id].IsSelected)
                {
                    changedSelectableItems.Add(nextSelectableItemValue);
                }
            
                SelectableItems[id] = nextSelectableItemValue;
            }
        } else {
            SelectableItem nextSelectableItemValue = new(item.ID, nextIsSelectedValue);

            changedSelectableItems.Add(nextSelectableItemValue);
            
            SelectableItems[item.ID] = nextSelectableItemValue;
        }

        if (changedSelectableItems.Count > 0)
        {
            Changed?.Invoke(this, new SelectableItemsChangedEvent(changedSelectableItems));
        }
    }

}
