using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DNA
{
    public class ControlsPanel : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Settings")]
        [SerializeField]
        private float panelDelay = 3f;
        [SerializeField]
        private float animationDuration = 2f;

        [SerializeField]
        private Image panelMask = null;
        #endregion

        #region Internal Variables
        private bool playerHasMoved = false;
        #endregion

        private void Start()
        {
            panelMask.fillAmount = 0f;
            if (References.progressionManager != null)
            {
                if (References.progressionManager.CurrentLevel == 0)
                    Invoke(nameof(ShowPanel), panelDelay);
                else
                    DisablePanel();
            }
        }

        private void ShowPanel()
        {
            panelMask.gameObject.SetActive(true);
            panelMask.DOFillAmount(1f, animationDuration).SetEase(Ease.OutExpo);
        }

        private void HidePanel()
        {
            panelMask.DOFillAmount(0f, animationDuration).SetEase(Ease.InExpo)
                .OnComplete(() => panelMask.gameObject.SetActive(false));
        }

        private void DisablePanel()
        {
            panelMask.gameObject.SetActive(false);
        }

        public void RegisterPlayerMovement()
        {
            if (!playerHasMoved)
            {
                playerHasMoved = true;
                CancelInvoke(nameof(ShowPanel));
                Invoke(nameof(HidePanel), 1.5f);
            }
        }
    }
}