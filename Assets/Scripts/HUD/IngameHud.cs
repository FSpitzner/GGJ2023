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
        [SerializeField]
        private ControlsPanel controlsPanel = null;
        #endregion

        #region Properties
        public ProgressBar ProgressBar { get { return progressBar; } }
        public CompletionPanel CompletionPanel { get { return completionPanel; } }
        public ScreenTransition ScreenTransition { get { return screenTransition; } }
        public ControlsPanel ControlsPanel { get { return controlsPanel; } }
        #endregion

        private void Awake()
        {
            // Singleton:
            if (References.ingameHud == null)
                References.ingameHud = this;
            else
                Destroy(gameObject);
        }
    }
}