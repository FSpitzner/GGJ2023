using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace DNA
{
    public class BombController : MonoBehaviour
    {
        [SerializeField] float explosionTimer = 5f;
        [SerializeField] Collider killzone = null;
        [SerializeField] GameObject xplosionPrefab = null;

        private RoomStateTracker roomStateTracker;

        private void Update()
        {
            explosionTimer -= Time.deltaTime;
            if (explosionTimer < 0)
            {
                roomStateTracker.ApplyBombCircle(transform.position);

                killzone.enabled = true;

                if (xplosionPrefab != null)
                {
                    VisualEffect boom = Instantiate(xplosionPrefab, transform.position, Quaternion.identity).GetComponent<VisualEffect>();
                    boom?.Play();
                }

                Destroy(gameObject, .25f);
            }
        }


        public void Initialize(RoomStateTracker tracker)
        {
            roomStateTracker = tracker;
        }

        public void Defuse()
        {
            Destroy(gameObject);
        }
    }
}