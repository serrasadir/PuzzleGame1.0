using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
namespace BlastGame
{
    public class MainMenuManager : MonoBehaviour
    {
        public Button levelButton;
        public TextMeshProUGUI buttonText;

        private int maxLevel = 10;
        private int currentLevel;

        void Start()
        {
            currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            LoadProgress();
            UpdateLevelButtonText();
            if (currentLevel <= maxLevel)
            {
                levelButton.onClick.AddListener(OnLevelButtonClicked);
            }
        }

        void UpdateLevelButtonText()
        {
            buttonText.text = currentLevel <= maxLevel ? $" Level {currentLevel}" : " Finished";
        }

        public void OnLevelButtonClicked()
        {
            if (currentLevel <= maxLevel)
            {
                Debug.Log("Loading game scene");
                SceneManager.LoadScene("GameScene");
            }
        }
        public void SaveProgress()
        {
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            PlayerPrefs.Save();
        }
        public void LoadProgress()
        {
            currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        }

    }
}