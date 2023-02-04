using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNA
{
    public class Flamethrower : MonoBehaviour
    {
        [SerializeField] RoomStateTracker roomStateTracker = null;
        [SerializeField] Transform point1, point2, point3, point4;
        [SerializeField] LayerMask cleaningMask = 0;

        private FlameBounds flamybounds = new FlameBounds();
        private Ray ray = new Ray();
        private RaycastHit hit;

        private void Start()
        {
            ray.direction = Vector3.down;
        }

        /*private void Update()
        {
            ray.origin = point1.position;
            if (Physics.Raycast(ray, out hit, 1f, cleaningMask))
            {
                flamybounds.a = hit.point;
            }

            ray.origin = point2.position;
            if (Physics.Raycast(ray, out hit, 1f, cleaningMask))
            {
                flamybounds.b = hit.point;
            }

            ray.origin = point3.position;
            if (Physics.Raycast(ray, out hit, 1f, cleaningMask))
            {
                flamybounds.c = hit.point;
            }

            ray.origin = point4.position;
            if (Physics.Raycast(ray, out hit, 1f, cleaningMask))
            {
                flamybounds.d = hit.point;
            }

            roomStateTracker.ApplyFlamethrowerImpact(flamybounds);
        }*/

        public void UseFlamethrower()
        {
            roomStateTracker.ApplyFlamethrowerCircle(transform.position);
        }
    }
}