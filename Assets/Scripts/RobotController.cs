using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;
using Unity.Mathematics;
using Random = UnityEngine.Random;

namespace DNA {
    public class RobotController : MonoBehaviour
    {
        private enum RobotState
        {
            Patrol, MoveWithNavMesh, DoAction, MoveToPatrol
        }

        private new Transform transform;

        [SerializeField] private SplineContainer splineContainer;
        [SerializeField] private NavMeshAgent navMeshAgent;

        [SerializeField] private RoomStateTracker roomStateTracker;
        [SerializeField] private Transform bombDropPoint;
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private LayerMask floorMask;

        [SerializeField] private float splineSpeed = 0.3f;
        [SerializeField] private float navSpeed = 3;
        [SerializeField] private float doTime = 3;
        [SerializeField] private int searchesPerFrame = 3;
        [SerializeField] private float searchDistance = 15f;

        private RobotState robotState = RobotState.Patrol;

        private float splinePosition = 0;

        private Vector3 savePosition;
        private float doTimer = 0;
        private bool bombDropped = false;

        private void Awake()
        {
            transform = gameObject.transform;

            navMeshAgent.speed = 0;
        }

        private void Update()
        {
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

            SearchForTarget();
        }

        private void SearchForTarget()
        {
            for (int i = 0; i < searchesPerFrame; i++)
            {
                Vector3 testPosition = GetNewRandomVector();
                int2 gridPos = roomStateTracker.PositionToGrid(testPosition);

                if (roomStateTracker.GetState(gridPos.x, gridPos.y) == RoomState.OVERGROWN_FLOOR)
                {
                    Ray ray = new Ray(transform.TransformPoint(testPosition), Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 999, floorMask))
                    {
                        navMeshAgent.speed = navSpeed;
                        navMeshAgent.SetDestination(hit.point);

                        savePosition = transform.position;
                        robotState = RobotState.MoveWithNavMesh;
                        return;
                    }
                }
            }
        }

        private void ProcessMoveWithNavMesh()
        {
            if (Vector3.Distance(navMeshAgent.destination, transform.position) < 0.5f)
            {
                doTimer = 0;

                robotState = RobotState.DoAction;
            }
        }

        private void ProcessDoAction()
        {
            doTimer += Time.deltaTime;

            if (doTimer >= doTime / 2 && !bombDropped)
            {
                BombController bombChef = Instantiate(bombPrefab, bombDropPoint.position, Quaternion.identity).GetComponent<BombController>();
                bombChef?.Initialize(roomStateTracker);
                bombDropped = true;
            }

            if (doTimer >= doTime)
            {
                navMeshAgent.SetDestination(savePosition);

                bombDropped = false;
                robotState = RobotState.MoveToPatrol;
            }
        }

        private void ProcessMoveToPatrol()
        {
            if (Vector3.Distance(navMeshAgent.destination, transform.position) < 0.5f)
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

        private Vector3 GetNewRandomVector()
        {
            return new Vector3(Random.Range(-searchDistance, searchDistance), 10f, Random.Range(-searchDistance, searchDistance));
        }
    }
}