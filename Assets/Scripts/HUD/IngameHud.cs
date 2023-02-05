using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNA
{
    public class IngameHud : MonoBehaviour
    {
        #region Inspector Variables
        [Header("References")]
        [SerializeField]
        private ProgressBar progressBar = null;
        [SerializeField]
        private CompletionPanel completionPanel = null;
        [SerializeField]
        private ScreenTransition screenTransition = null;
        #endregion

        #region Properties
        public ProgressBar ProgressBar { get { return progressBar; } }
        public CompletionPanel CompletionPanel { get { return completionPanel; } }
        public ScreenTransition ScreenTransition { get { return screenTransition; } }
        #endregion
    }
}