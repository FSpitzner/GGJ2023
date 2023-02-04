using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DNA
{
    public class PercentageBar : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Settings")]
        [SerializeField]
        private string percentageSuffix = "%";

        [Header("References")]
        [SerializeField]
        private TMP_Text percentageText = null;
        [SerializeField]
        private Image barFill = null;
        #endregion

        #region Internal Variables
        private Tween barFillTween = null;
        #endregion

        #region Properties
        public float Percentage {
            set
            {
                float displayPercentage = Mathf.Clamp(value, 0f, 1f);
                percentageText.text = string.Format("{0}{1}", Mathf.Round(100f * displayPercentage).ToString(), percentageSuffix);
                barFill.fillAmount = displayPercentage;
            }
        }

        private float BarFill
        {
            set
            {

            }
        }
        #endregion
    }
}