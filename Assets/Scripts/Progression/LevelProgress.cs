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
        #endregion

        #region Internal Variables
        private float progress = 0f;
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

        #region UI

        private void UpdateProgress()
        {
            if (percentageBar != null)
                percentageBar.Percentage = progress;
        }

        #endregion
    }
}