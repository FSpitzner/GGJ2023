using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsButton : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField]
    private float timeBetweenAnimations = 1f;
    [SerializeField]
    private Vector3 regularScale = Vector3.one;
    [SerializeField]
    private Vector3 smallScale = Vector3.one;
    [SerializeField]
    private RectTransform playerIcon = null;
    #endregion

    #region Internal Variables
    private bool isSmall = false;
    #endregion

    void Start()
    {
        InvokeRepeating(nameof(ChangeScale), timeBetweenAnimations, timeBetweenAnimations);
    }

    private void ChangeScale()
    {
        isSmall = !isSmall;
        transform.localScale = isSmall ? smallScale : regularScale;

        if (isSmall)
            ShakePlayerIcon();
        else
            JumpPlayerIcon();
    }

    private void ShakePlayerIcon()
    {
        playerIcon.anchoredPosition = new Vector2(55f, -40f);
        playerIcon.DOShakeAnchorPos(timeBetweenAnimations - 0.1f, 5f, fadeOut: false);
    }

    private void JumpPlayerIcon()
    {
        playerIcon.DOAnchorPosX(125f, timeBetweenAnimations - 0.25f).SetEase(Ease.OutExpo).SetDelay(0.05f);
        Sequence jumpSequence = DOTween.Sequence();
        jumpSequence.Append(playerIcon.DOAnchorPosY(10f, (timeBetweenAnimations - 0.1f) / 2f)).SetEase(Ease.OutExpo);
        jumpSequence.Append(playerIcon.DOAnchorPosY(-40f, (timeBetweenAnimations - 0.1f) / 2f)).SetEase(Ease.OutExpo).SetDelay((timeBetweenAnimations - 0.1f) / 2f);
    }
}