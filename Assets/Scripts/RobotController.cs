using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class RobotController : MonoBehaviour
{
    private new Transform transform;

    [SerializeField] private SplineContainer spline;
    [SerializeField] private float moveSpeed = 0.1f;

    private float splinePosition = 0;

    private void Awake()
    {
        transform = gameObject.transform;
    }

    private void Update()
    {
        splinePosition += moveSpeed * Time.deltaTime;
        if (splinePosition > 1)
            splinePosition -= 1;

        Vector3 position = spline.EvaluatePosition(splinePosition);
        position.y = 0;
        transform.position = position;
        transform.forward = spline.EvaluateTangent(splinePosition);
    }
}
