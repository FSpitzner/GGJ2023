using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DNA
{
    public class ProgressionManager : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField]
        private string[] levelScenes = null;
        [SerializeField]
        private string finalScene = null;
        #endregion

        #region Internal Variables
        private int currentLevel = 0;
        #endregion

        #region Properties
        public int CurrentLevel { get { return currentLevel; } }
        #endregion

        #region Setup

        void Awake()
        {
            // Scene-persistent Singleton:
            if (References.progressionManager != null)
                Destroy(gameObject);
            else
            {
                References.progressionManager = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        #endregion

        #region Level Loading

        public void LoadNextLevel()
        {
            currentLevel++;

            // Load final scene if all levels have been played:
            if (currentLevel >= levelScenes.Length)
                SceneManager.LoadScene(finalScene);

            // Load next level if it exists:
            else
                SceneManager.LoadScene(levelScenes[currentLevel]);
        }

        #endregion
    }
}