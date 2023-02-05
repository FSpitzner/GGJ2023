using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNA
{
    public class Flamethrower : MonoBehaviour
    {
        [SerializeField] RoomStateTracker roomStateTracker = null;
        [SerializeField] Transform cleanPoint;

        public void UseFlamethrower()
        {
            roomStateTracker.ApplyFlamethrowerCircle(cleanPoint.position);
        }
    }
}