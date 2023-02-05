using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNA
{
    public class LevelProgress : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Target")]
        [SerializeField]
        [MinMaxRange(0f, 1f)]
        private float targetValue = 0.5f;

        [Header("References")]
        [SerializeField]
        private IngameHud hud = null;
        #endregion

        #region Internal Variables
        private float progress = 0f;
        private bool gameIsRunning = true;
        #endregion

        #region Properties
        public float Progress { get { return progress; } set { progress = Mathf.Clamp(value, 0f, 1f); UpdateProgress(); } }
        #endregion

        #region Setup

        private void Start()
        {
            // Display target indicator in progress bar:
            if (hud != null && hud.ProgressBar != null)
            {
                hud.ProgressBar.TargetValue = targetValue;
            }
        }

        #endregion

        #region Progress Checks

        private void UpdateProgress()
        {
            // Update UI:
            if (hud != null && hud.ProgressBar != null)
                hud.ProgressBar.Percentage = progress;

            // Check if level is completed:
            if (gameIsRunning && progress >= targetValue)
                CompleteLevel();
        }

        private void CompleteLevel()
        {
            gameIsRunning = false;

            // Hide progress bar:
            if (hud != null && hud.ProgressBar != null)
                hud.ProgressBar.Hide();

            // Display completion message:
            if (hud != null && hud.CompletionPanel != null)
                hud.CompletionPanel.Display();

            // Invoke transition to next level:
            Invoke(nameof(EndLevel), 3f);
            Invoke(nameof(LoadNextLevel), 6f);
        }

        private void EndLevel()
        {
            // Fade screen to black:
            if (hud != null && hud.ScreenTransition != null)
                hud.ScreenTransition.FadeToBlack();
        }

        private void LoadNextLevel()
        {
            if (References.progressionManager != null)
                References.progressionManager.LoadNextLevel();
        }

        #endregion
    }
}