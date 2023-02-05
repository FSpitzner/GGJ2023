using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    #region Inspector Variables
    [Header("Settings")]
    [SerializeField]
    private bool fadeFromBlackAtStart = true;

    [Header("Animation")]
    [SerializeField]
    private float fadeToBlackDuration = 2f;
    [SerializeField]
    private float fadeFromBlackDuration = 2f;

    [Header("References")]
    [SerializeField]
    private RectTransform barsTransform = null;
    #endregion

    void Start()
    {
        barsTransform.gameObject.SetActive(false);
        if (fadeFromBlackAtStart)
            FadeFromBlack();
    }

    public void FadeToBlack()
    {
        barsTransform.gameObject.SetActive(true);
        barsTransform.anchoredPosition = new Vector2(0f, 0f);
        barsTransform.DOAnchorPosX(9000f, fadeToBlackDuration).SetEase(Ease.InExpo);
    }

    public void FadeFromBlack()
    {
        barsTransform.gameObject.SetActive(true);
        barsTransform.anchoredPosition = new Vector2(9000f, 0f);
        barsTransform.DOAnchorPosX(15000f, fadeFromBlackDuration).SetEase(Ease.InExpo).OnComplete(() => barsTransform.gameObject.SetActive(false));
    }
}