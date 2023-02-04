using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DNA
{
    public class ProgressBar : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Settings")]
        [SerializeField]
        private string percentageSuffix = "%";

        [Header("Animation")]
        private float barFillAnimationTime = 0.5f;
        [SerializeField]
        private Color targetIndicatorSecondaryColor = Color.red;
        [SerializeField]
        private Vector3 targetIndicatorIncreasedSize = new Vector3(2f, 2f, 2f);
        [SerializeField]
        private float targetIndicatorColorAnimationSpeed = 1f;

        [Header("References")]
        [SerializeField]
        private RectTransform barTransform = null;
        [SerializeField]
        private TMP_Text percentageText = null;
        [SerializeField]
        private Image barFill = null;
        [SerializeField]
        private Slider targetIndicator = null;
        #endregion

        #region Internal Variables
        /*private Tween barFillTween = null;*/
        #endregion

        #region Properties
        public float Percentage {
            set
            {
                float displayPercentage = Mathf.Clamp(value, 0f, 1f);
                percentageText.text = string.Format("{0}{1}", Mathf.Round(100f * displayPercentage).ToString(), percentageSuffix);
                BarFill = displayPercentage;
            }
        }

        private float BarFill
        {
            set
            {
                barFill.fillAmount = value;
                /*if (barFillTween != null)
                    barFillTween.Kill();
                barFillTween = barFill.DOFillAmount(value, barFillAnimationTime).SetEase(Ease.InOutExpo);*/
            }
        }

        public float TargetValue
        {
            set
            {
                targetIndicator.value = Mathf.Clamp(value, 0f, 1f);
            }
        }
        #endregion

        #region Setup

        private void Start()
        {
            
        }

        private void InitializeAnimations()
        {
            // Loop color change of target indicator:
            targetIndicator.image.DOColor(targetIndicatorSecondaryColor, targetIndicatorColorAnimationSpeed / 4f).SetLoops(-1);

            // Scale up at start:
            
        }

        #endregion
    }
}