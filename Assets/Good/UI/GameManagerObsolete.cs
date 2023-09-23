using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Good.UI
{

    public class GameManagerObsolete
    {
        public UIDocument GameScreen;

        private VisualElement gameRoot;

        private ProgressBar gauge;
        private Label debugTextArea;

        [Range(-1,1)]
        public float bratio;

        public string debugInfo;

        private void Awake()
        {
            gameRoot = GameScreen?.rootVisualElement;

            if (gameRoot != null) {
                gauge = gameRoot.Q<ProgressBar>("gauge");
                debugTextArea = gameRoot.Q<Label>("debug-text-area");

                var toMenuButton = gameRoot.Q<Button>("to-menu-button");
                if (toMenuButton != null)
                {
                    toMenuButton.clicked += () =>
                    {
                        Debug.Log("To Menu Button Clicked");
                        SceneManager.LoadScene("Main");
                    };
                }
            }
        }
        
        void OnValidate()
        {
            UpdateGauge();
        }

        void Update()
        {
            UpdateGauge();
        }

        void UpdateGauge()
        {
            if (gauge != null)
            {
                gauge.value = bratio + 1f;
            }

            if (debugTextArea != null) {
                debugTextArea.text = debugInfo;
            }
        }

    }
    
}
