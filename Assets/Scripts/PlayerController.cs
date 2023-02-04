using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DNA
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float impulsPowerModifier = 1f;
        [SerializeField] Rigidbody rb = null;
        [SerializeField] InputWrapper input = null;
        [SerializeField] float maxPowerAfterSeconds = 2f;
        [SerializeField] Image powerIndicator = null;
        [SerializeField] RoomStateTracker roomStateTracker = null;

        [SerializeField] GameObject playerMesh = null;
        [SerializeField] GameObject projectile = null;
        [SerializeField] float heightOffsetProjectile = .75f;
        [SerializeField] LayerMask groundLayermask = 0;

        [SerializeField] float spreadRadius = 3f;

        private Transform owntransform = null;
        private Vector3 direction = new Vector3();
        private float powerTimer = 0f;
        private Transform projectileTransform = null;
        private Ray groundRay;
        private RaycastHit hit;

        private void Start()
        {
            UpdateIndicator(0f);
            owntransform = transform;
            projectileTransform = projectile.transform;
            groundRay = new Ray(projectileTransform.position, Vector3.down);
        }

        void Update()
        {
            direction = new Vector3(input.Direction.x, 0.75f, input.Direction.y);

            if (input.JumpActive)
            {
                powerTimer += Time.deltaTime;
                if (powerTimer > maxPowerAfterSeconds)
                    powerTimer = maxPowerAfterSeconds;
                float powerNormalized = powerTimer.RemapExclusive(0, maxPowerAfterSeconds, 0, 1f);
                UpdateIndicator(powerNormalized);
            }
            else if (!input.JumpActive && powerTimer > 0f)
            {
                float powerNormalized = powerTimer.RemapExclusive(0, maxPowerAfterSeconds, 0, 1f);
                Jump(direction, powerNormalized);
                powerTimer = 0f;
                UpdateIndicator(0f);
            }
        }

        void Jump(Vector3 direction, float power)
        {
            projectile.SetActive(true);
            playerMesh.SetActive(false);
            rb.AddRelativeForce(direction * power * impulsPowerModifier, ForceMode.Impulse);
        }

        void UpdateIndicator(float power)
        {
            powerIndicator.fillAmount = power;
        }

        private void OnCollisionEnter(Collision collision)
        {
            groundRay.origin = projectileTransform.position;

            if (Physics.Raycast(groundRay, out hit, .3f, groundLayermask))
            {
                owntransform.position = new Vector3(owntransform.position.x, hit.point.y + heightOffsetProjectile, owntransform.position.z);
                roomStateTracker.ApplyPlayerImpact(hit.point);

                playerMesh.SetActive(true);
                projectile.SetActive(false);

                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}