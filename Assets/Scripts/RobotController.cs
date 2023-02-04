using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

public class RobotController : MonoBehaviour
{
    private enum RobotState
    {
        Patrol, MoveWithNavMesh, DoAction, MoveToPatrol
    }

    private new Transform transform;

    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private NavMeshAgent navMeshAgent;

    [SerializeField] private LayerMask floorMask;

    [SerializeField] private float splineSpeed = 0.3f;
    [SerializeField] private float navSpeed = 3;
    [SerializeField] private float doTime = 3;
    
    private RobotState robotState = RobotState.Patrol;

    private float splinePosition = 0;

    private Vector3 savePosition;
    private float doTimer = 0;

    private void Awake()
    {
        transform = gameObject.transform;

        navMeshAgent.speed = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 999, floorMask))
            {
                navMeshAgent.speed = navSpeed;
                navMeshAgent.SetDestination(hit.point);

                savePosition = transform.position;
                robotState = RobotState.MoveWithNavMesh;
            }
        }

        switch (robotState)
        {
            case RobotState.Patrol:
                ProcessPatrol();
                break;
            case RobotState.MoveWithNavMesh:
                ProcessMoveWithNavMesh();
                break;
            case RobotState.DoAction:
                ProcessDoAction();
                break;
            case RobotState.MoveToPatrol:
                ProcessMoveToPatrol();
                break;
            default:
                break;
        }
    }

    private void ProcessPatrol()
    {
        splinePosition += splineSpeed * Time.deltaTime;
        if (splinePosition > 1)
            splinePosition -= 1;

        Vector3 position = splineContainer.EvaluatePosition(splinePosition);
        position.y = 0;
        transform.position = position;

        transform.forward = splineContainer.EvaluateTangent(splinePosition);
    }

    private void ProcessMoveWithNavMesh()
    {
        if(Vector3.Distance(navMeshAgent.destination, transform.position) < 0.5f)
        {
            doTimer = 0;

            robotState = RobotState.DoAction;
        }
    }

    private void ProcessDoAction()
    {
        doTimer += Time.deltaTime;
        if(doTimer >= doTime)
        {
            navMeshAgent.SetDestination(savePosition);

            robotState = RobotState.MoveToPatrol;
        }
    }

    private void ProcessMoveToPatrol()
    {
        if(Vector3.Distance(navMeshAgent.destination, transform.position) < 0.5f)
        {
            navMeshAgent.speed = 0;

            robotState = RobotState.Patrol;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.ApplyDamage();
        }
        else if (other.attachedRigidbody)
        {
            damageable = other.attachedRigidbody.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.ApplyDamage();
            }
        }
        
    }
}
