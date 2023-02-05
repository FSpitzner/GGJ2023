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
        private float startAnimationDelay = 1f;
        [SerializeField]
        private string percentageSuffix = "%";

        [Header("Animation")]
        [SerializeField]
        private Color targetIndicatorPrimaryColor = Color.green;
        [SerializeField]
        private Color targetIndicatorSecondaryColor = Color.red;
        [SerializeField]
        private Vector3 targetIndicatorDefaultScale = new Vector3(1f, 1f, 1f);
        [SerializeField]
        private Vector3 targetIndicatorIncreasedScale = new Vector3(2f, 2f, 2f);
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
        private bool acceptValues = false;
        #endregion

        #region Properties
        public float Percentage {
            set
            {
                if (acceptValues)
                {
                    float displayPercentage = Mathf.Clamp(value, 0f, 1f);
                    percentageText.text = string.Format("{0}{1}", Mathf.Floor(100f * displayPercentage).ToString(), percentageSuffix);
                    BarFill = displayPercentage;
                }
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
            barFill.fillAmount = 1f;
            Percentage = 1f;
            percentageText.text = string.Format("{0}{1}", 100f, percentageSuffix);
            Invoke(nameof(InitializeAnimations), startAnimationDelay);
        }

        private void InitializeAnimations()
        {
            // Loop color change of target indicator:
            Sequence colorSequence = DOTween.Sequence();
            colorSequence.Append(targetIndicator.image.DOColor(targetIndicatorSecondaryColor, targetIndicatorColorAnimationSpeed));
            colorSequence.Append(targetIndicator.image.DOColor(targetIndicatorPrimaryColor, targetIndicatorColorAnimationSpeed));
            colorSequence.SetLoops(-1);

            // Scale up at start:
            Sequence scaleSequence = DOTween.Sequence();
            scaleSequence.Append(barTransform.DOScale(targetIndicatorIncreasedScale, 0.5f).SetEase(Ease.OutExpo));

            // Empty bar with animation:
            barFill.DOFillAmount(References.levelProgress != null ? References.levelProgress.Progress : 0f, 0.6f).SetEase(Ease.InOutExpo).SetDelay(0.5f)
                .OnUpdate(() => percentageText.text = string.Format("{0}{1}", Mathf.Floor(100f * barFill.fillAmount).ToString(), percentageSuffix))
                .OnComplete(() => percentageText.transform.DOShakePosition(0.25f, 5f));

            // Scale back down to base size:
            scaleSequence.Append(barTransform.DOScale(targetIndicatorDefaultScale, 1f).SetEase(Ease.OutExpo).SetDelay(1.25f));

            // Move back and forth in sync with scaling:
            Sequence moveSequence = DOTween.Sequence();
            moveSequence.Append(barTransform.DOAnchorPosY(-400f, 0.5f).SetEase(Ease.OutExpo));
            moveSequence.Append(barTransform.DOAnchorPosY(-50f, 1f).SetEase(Ease.OutExpo).SetDelay(1.25f));
            moveSequence.OnComplete(() => acceptValues = true);
        }

        #endregion

        public void Hide()
        {
            barTransform.DOAnchorPosY(200f, 1f).SetEase(Ease.InExpo);
        }
    }
}