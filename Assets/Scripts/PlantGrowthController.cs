using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace DNA
{
    public class PlantGrowthController : MonoBehaviour
    {
        class GrowthTracker
        {
            public Vector2Int arrayPosition;
            public GameObject spawnedObject;
        }

        [SerializeField] RoomStateTracker roomStateTracker;
        [SerializeField] int numberOfGrowths = 10;
        [SerializeField] List<GameObject> growthPrefabs = new List<GameObject>();
        [SerializeField] LayerMask groundMask = 0;

        Dictionary<int, GrowthTracker> growths = new Dictionary<int, GrowthTracker>();
        Random random;
        Ray ray = new Ray(Vector3.zero, Vector3.down);
        RaycastHit hit;

        void Start()
        {
            int2 dimentions = new int2(50, 50);
            random = new Random(5611556);

            for (int i = 0; i < numberOfGrowths; i++)
            {
                Vector2Int point = new Vector2Int(random.NextInt(0, dimentions.x), random.NextInt(0, dimentions.y));

                switch (roomStateTracker.GetState(point.x, point.y))
                {
                    case RoomState.WALL:
                        i--;
                        continue;
                    case RoomState.EMPTY:
                        i--;
                        continue;
                }

                growths.Add(i, new GrowthTracker()
                {
                    arrayPosition = point,
                    spawnedObject = null
                });
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            if (growths.Count > 0)
            {
                for (int i = 0; i < growths.Count; i++)
                {
                    Vector3 pointPos2D = roomStateTracker.GridToPosition(growths[i].arrayPosition.x, growths[i].arrayPosition.y);
                    pointPos2D.y = 20f;
                    if (Physics.Raycast(ray, out hit, 30f, groundMask))
                    {
                        Gizmos.DrawWireSphere(hit.point, .5f);
                    }
                }
            }
        }

        public void CheckForGrowth()
        {
            StartCoroutine(DelayedAction(() =>
            {
                CheckGrowthState();
            }, 0f));
        }

        private void CheckGrowthState()
        {
            for (int i = 0; i < numberOfGrowths; i++)
            {
                RoomState stateAtPosition = RoomState.EMPTY;

                switch (stateAtPosition)
                {
                    case RoomState.CLEAN_FLOOR:
                        if (growths[i].spawnedObject != null)
                        {
                            Destroy(growths[i].spawnedObject);
                            growths[i].spawnedObject = null;
                        }
                        break;

                    case RoomState.OVERGROWN_FLOOR:
                        if (growths[i].spawnedObject == null)
                        {
                            Vector3 pointPos2D = roomStateTracker.GridToPosition(growths[i].arrayPosition.x, growths[i].arrayPosition.y);
                            pointPos2D.y = 20f;
                            if (Physics.Raycast(ray, out hit, 30f, groundMask))
                            {
                                growths[i].spawnedObject = Instantiate(growthPrefabs[random.NextInt(0, growthPrefabs.Count - 1)], hit.point, Quaternion.identity);
                            }
                        }
                        break;
                }
            }
        }

        IEnumerator DelayedAction(Action a, float delay)
        {
            yield return new WaitForSeconds(delay);

            a.Invoke();
        }
    }
}