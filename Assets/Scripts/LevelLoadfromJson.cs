using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlastGame;


namespace BlastGame
{
    public class LevelLoadFromJson : MonoBehaviour
    {
        public GameManager grid;

        public void LoadLevel(int levelNumber)
        {
            LevelDataJson levelData = LoadLevelData(levelNumber);
            if (levelData != null)
            {
                grid.currentLevelData = levelData; 
                grid.InitializeGridFromLevelData();
            }
            else
            {
                Debug.LogError("failed");
            }
        }

        LevelDataJson LoadLevelData(int levelNumber)
        {
            string levelPath;
            if (levelNumber < 10) {  levelPath = $"Level_0{levelNumber}"; }
            else { levelPath = $"Level_{levelNumber}"; }
            TextAsset levelJson = Resources.Load<TextAsset>(levelPath);
            if (levelJson != null)
            {
                return JsonUtility.FromJson<LevelDataJson>(levelJson.text);
            }
            return null;
        }


    }
}