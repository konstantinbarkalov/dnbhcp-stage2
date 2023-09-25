using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI {
    
    public class UIManager : AbstractAppManagerBehaviour
    {
        public UIDocument mainScreen;
        public UIDocument gameScreen;
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
        private Label scoreLabel;

        private float Naturalize(float value, int factor) { // TODO: put to utils
            bool isInverted = (value < 1);
            if (isInverted) {
                value = 1 / value;
            }
            value = Mathf.Round(value * factor) / factor;
            if (isInverted) {
                value = 1 / value;
            }
            return value;
        }
        private void InitMain()
        {
            var playButton = mainRoot.Q<Button>("content-play-button");
            playButton.clicked += () =>
            {
                // Debug.Log("Play Button Clicked");
                SceneManager.LoadScene("Level A");
                GoToGameScreen();
            };

            var zombiesCountSlider = mainRoot.Q<Slider>("zombies-count");
            zombiesCountSlider.value = PlayerPrefs.GetInt("zombies-count", 100);
            zombiesCountSlider.RegisterValueChangedCallback(v =>
            {
                int intValue = (int)Mathf.Round(v.newValue / 25) * 25;                
                PlayerPrefs.SetInt("zombies-count", (int)v.newValue);
                zombiesCountSlider.value = intValue; 
            });
            

            var baseTimeScaleSlider = mainRoot.Q<Slider>("base-time-scale");
            baseTimeScaleSlider.value = PlayerPrefs.GetFloat("base-time-scale", 1);
            baseTimeScaleSlider.RegisterValueChangedCallback(v =>
            {
                float roundishValue = Naturalize(v.newValue, 6);                
                PlayerPrefs.SetFloat("base-time-scale", v.newValue);
                baseTimeScaleSlider.value = roundishValue;
                MetaManager.level.timeManager.SetBaseTimeScale(roundishValue); 
            });


            var initialControlType = PlayerPrefs.GetString("control-type", "keybord-control");
            var namesOfControlTypes = new List<string> { "keybord-control", "joystick-control", "sliders-control", "arrows-control" };

            SelectableList selectableList = new(namesOfControlTypes.ConvertAll(id => new SelectableItem(id, initialControlType == id)));

            Dictionary<string, Button> controlTypeButtonsMap = namesOfControlTypes.ToDictionary(
                item => item,
                item => mainRoot.Q<Button>(item)
            );

            controlTypeButtonsMap[initialControlType].AddToClassList("selected");

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
        }

        private void InitGame()
        {
            var toMenuButton = gameRoot.Q<Button>("to-menu-button");
            toMenuButton.clicked += () =>
            {
                // Debug.Log("To Menu Button Clicked");
                SceneManager.LoadScene("Level S");
                GoToMainScreen();
            };
        }

        private bool isInitialized = false;

        public void Init()
        {
            if (!isInitialized)
            {
                InitMain();
                InitGame();
                isInitialized = true;
            }
        }

        private void Awake()
        {
            mainRoot = mainScreen.rootVisualElement;
            gameRoot = gameScreen.rootVisualElement;
            helicopterPowerGauge = gameRoot.Q<ProgressBar>("helicopter-power-gauge");
            helicopterStuntGauge = gameRoot.Q<ProgressBar>("helicopter-stunt-gauge");
            helicopterTravelGauge = gameRoot.Q<ProgressBar>("helicopter-travel-gauge");
            helicopterBrakeGauge = gameRoot.Q<ProgressBar>("helicopter-brake-gauge");
            debugTextArea = gameRoot.Q<Label>("debug-text-area");
            controlModeField = gameRoot.Q<EnumField>("control-mode");
            scoreLabel = gameRoot.Q<Label>("score");
            Init();
            GoToMainScreen();
        }
        

        void OnValidate() {
            // TODO: do we need UpdateGauge() here? 
            //UpdateGauge();
        }

        void Update() {
            scoreLabel.text = MetaManager.level.scoreManager.score.ToString();
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
            if (mainScreen != null)
            {
                SetScreenEnableState(mainRoot, true);
            }
            if (gameScreen != null)
            {
                SetScreenEnableState(gameRoot, false);
            }
        }

        private void GoToGameScreen()
        {
            if (mainScreen != null && mainRoot != null)
            {
                SetScreenEnableState(mainRoot, false);
            }
            if (gameScreen != null)
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
