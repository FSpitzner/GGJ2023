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

        private Vector3 direction = new Vector3();
        private float powerTimer = 0f;

        private void Start()
        {
            UpdateIndicator(0f);
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
            rb.AddRelativeForce(direction * power * impulsPowerModifier, ForceMode.Impulse);
        }

        void UpdateIndicator(float power)
        {
            powerIndicator.fillAmount = power;
        }

        private void OnCollisionEnter(Collision collision)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}