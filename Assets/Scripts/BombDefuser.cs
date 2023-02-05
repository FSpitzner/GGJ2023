using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNA
{
    public class BombDefuser : MonoBehaviour
    {
        [SerializeField] BombController bombController = null;

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                bombController.Defuse();
            }
        }
    }
}