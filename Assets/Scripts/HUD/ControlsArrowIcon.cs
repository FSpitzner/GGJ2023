using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsArrowIcon : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField]
    private float animationDuration = 1f;
    [SerializeField]
    private float timeBetweenRotations = 1.5f;
    #endregion

    void Start()
    {
        InvokeRepeating(nameof(TurnRandom), 1f, timeBetweenRotations);
    }

    private void TurnRandom()
    {
        float randomRotation = Random.Range(0f, 360f);
        transform.DORotate(new Vector3(0f, 0f, randomRotation), animationDuration).SetEase(Ease.InOutExpo);
    }
}