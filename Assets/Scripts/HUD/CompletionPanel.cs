using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletionPanel : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField]
    private RectTransform panelTransform = null;
    #endregion

    #region Setup

    void Start()
    {
        // Hide panel below screen:
        panelTransform.anchoredPosition = new Vector2(0f, -1000f);
    }

    #endregion

    public void Display()
    {
        panelTransform.DOAnchorPosY(0f, 1f).SetEase(Ease.OutExpo);
    }
}
