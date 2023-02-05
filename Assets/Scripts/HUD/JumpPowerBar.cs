using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DNA
{
    public class JumpPowerBar : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Settings")]
        [SerializeField]
        private Vector3 positionOffset = Vector3.zero;

        [Header("References")]
        [SerializeField]
        private Image barFill = null;
        [SerializeField]
        private RectTransform barTransform = null;
        #endregion

        #region Internal Variables
        private PlayerController playerController = null;
        #endregion

        #region Properties
        public float Fill
        {
            set
            {
                barFill.fillAmount = value;
            }
        }
        #endregion

        private void Start()
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        private void LateUpdate()
        {
            SnapToPlayer();
        }

        private void SnapToPlayer()
        {
            if (playerController == null || barTransform == null)
            {
                barTransform.gameObject.SetActive(false);
                return;
            }

            Vector3 targetPosition = Camera.main.WorldToScreenPoint(playerController.transform.position);
            barTransform.position = targetPosition + positionOffset;
        }
    }
}