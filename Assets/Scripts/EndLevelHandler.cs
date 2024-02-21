using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;


namespace BlastGame
{
    public class EndLevelHandler : MonoBehaviour //if a level ends,we handle the situation here
    {
        public GameObject LevelPassedOverlay;
        public GameObject LevelFailedOverlay;
        public LevelLoadFromJson levelLoader;
        public Button retryButton;
        public Button mainMenuButton;
        public GameObject star;
        public GameManager grid;

        void Start()
        {
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            retryButton.onClick.AddListener(OnRetryButtonClicked);
        }

        public void HandleWinCondition() 
        {
            LevelPassedOverlay.SetActive(true); //leave this overlay and turn back to the main page by tapping the screen

            EndLevelAnimation endLevelAnimation = star.GetComponent<EndLevelAnimation>();//lets celebrate the win with a star animation
            endLevelAnimation.CelebrationAnimation();
            Debug.Log("Level completed - Win");

            int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            int nextLevel = currentLevel + 1;//increase  the level by 1 if the level is succeed

            PlayerPrefs.SetInt("CurrentLevel", nextLevel); 
            PlayerPrefs.Save();

        }

        public void OnOverlayClicked()
        {
            StartCoroutine(LoadMainScene());
        }

        private IEnumerator LoadMainScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            LevelPassedOverlay.SetActive(false); //wait for scene management to complete beforeclosing the overlay
        }

        public void HandleFailCondition()
        {
            LevelFailedOverlay.SetActive(true); //user can decide whether they want to retry or go to the main page

            Debug.Log("Level Completed - Fail");

        }

        [System.Obsolete]
        public void OnRetryButtonClicked()
        {
            Debug.Log("retry button clicked");

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);//refresh the level on retry

            LevelFailedOverlay.SetActive(false);
        }

        public void OnMainMenuButtonClicked()
        {
            Debug.Log("retryButton button clicked");
            LevelFailedOverlay.SetActive(false);
            SceneManager.LoadScene("MainScene");

        }
    }
}