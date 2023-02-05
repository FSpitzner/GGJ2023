using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNA {
    public class BombKillzone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            PlayerController pc = other.attachedRigidbody.GetComponent<PlayerController>();
            if (pc)
            {
                pc.ApplyDamage();
            }
        }
    }
}