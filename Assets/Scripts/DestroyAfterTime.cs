using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float time = 0.5f;

    private void Start()
    {
        Destroy(gameObject, time);
    }
}
