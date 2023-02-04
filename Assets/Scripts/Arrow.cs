using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private Transform scaleTransform;

    public void SetDirection(Vector3 direction)
    {
        transform.forward = direction;
    }

    public void SetScale(float scale)
    {
        scaleTransform.localScale = new Vector3(scale, 1, scale);
    }

    public void SetActive(bool active)
    {
        renderer.enabled = active;
    }
}
