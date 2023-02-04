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
        private ProgressBar percentageBar = null;
        [SerializeField]
        private CompletionPanel completionPanel = null;
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
            if (percentageBar != null)
            {
                percentageBar.TargetValue = targetValue;
            }
        }

        #endregion

        #region Progress Checks

        private void UpdateProgress()
        {
            // Update UI:
            if (percentageBar != null)
                percentageBar.Percentage = progress;

            // Check if level is completed:
            if (gameIsRunning && progress >= targetValue)
                CompleteLevel();
        }

        private void CompleteLevel()
        {
            gameIsRunning = false;

            // Hide progress bar:
            if (percentageBar != null)
                percentageBar.Hide();

            // Display completion message:
            if (completionPanel != null)
                completionPanel.Display();
        }

        #endregion
    }
}