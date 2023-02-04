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
        [Header("References")]
        [SerializeField]
        private TMP_Text percentageText = null;
        [SerializeField]
        private Image barFill = null;
        #endregion

        #region Properties
        public float Percentage {
            set
            {
                float displayPercentage = Mathf.Clamp(value, 0f, 1f);
                percentageText.text = Mathf.Round(100f * displayPercentage).ToString();
                barFill.fillAmount = displayPercentage;
            }
        }
        #endregion
    }
}