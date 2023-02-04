using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNA
{
    public class Flamethrower : MonoBehaviour
    {
        [SerializeField] RoomStateTracker roomStateTracker = null;

        public void UseFlamethrower()
        {
            roomStateTracker.ApplyFlamethrowerCircle(transform.position);
        }
    }
}