using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DNA
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [SerializeField] float impulsPowerModifier = 1f;
        [SerializeField] Rigidbody rb = null;
        [SerializeField] InputWrapper input = null;
        [SerializeField] float maxPowerAfterSeconds = 2f;
        [SerializeField] Image powerIndicator = null;
        [SerializeField] RoomStateTracker roomStateTracker = null;
        [SerializeField] PlantGrowthController plantGrowthController = null;

        [SerializeField] GameObject playerMesh = null;
        [SerializeField] GameObject projectile = null;
        [SerializeField] GameObject contactPointPrefab;
        [SerializeField] float heightOffsetProjectile = .75f;
        [SerializeField] LayerMask groundLayermask = 0;

        [SerializeField] private Arrow arrow;

        [SerializeField] float spreadRadius = 3f;

        [SerializeField] private int health = 3;

        private Transform owntransform = null;
        private Vector3 direction = new Vector3();
        private float powerTimer = 0f;
        private Transform projectileTransform = null;
        private Ray groundRay;
        private RaycastHit hit;

        private List<PlayerContactPoint> pointTrackList = new List<PlayerContactPoint>();

        private void Start()
        {
            UpdateIndicator(0f);
            owntransform = transform;
            projectileTransform = projectile.transform;
            groundRay = new Ray(projectileTransform.position, Vector3.down);
        }

        void Update()
        {
            direction = new Vector3(input.Direction.x, 0, input.Direction.y);
            if (direction.magnitude <= 0.1f)
                return;

            arrow.SetDirection(direction);
            arrow.SetScale(1);
            playerMesh.transform.forward = direction;

            Debug.Log($"Player direction: {direction}");

            direction.y = 0.75f;

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
            direction = new Vector3(input.Direction.x, 0.75f, input.Direction.y);

            projectile.SetActive(true);
            playerMesh.SetActive(false);
            arrow.SetActive(false);
            rb.AddRelativeForce(direction * power * impulsPowerModifier, ForceMode.Impulse);
        }

        void UpdateIndicator(float power)
        {
            powerIndicator.fillAmount = power;
            arrow.SetScale(1 + power);
        }

        public void ApplyDamage(int damage = 1)
        {
            health -= damage;

            Debug.Log($"Player damaged; health: {health}");

            if (health <= 0)
                SceneManager.LoadScene(0);

            for (int i = pointTrackList.Count - 1; i >= 0; i--)
            {
                if (!pointTrackList[i])
                    pointTrackList.RemoveAt(i);
                else
                {
                    transform.position = pointTrackList[i].transform.position;
                    break;
                }
            }
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
                arrow.SetActive(true);

                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                plantGrowthController.CheckForGrowth();

                PlayerContactPoint contactPoint = Instantiate(contactPointPrefab, transform.position, Quaternion.identity).GetComponent<PlayerContactPoint>();
                pointTrackList.Add(contactPoint);

                if(pointTrackList.Count > 5)
                {
                    Destroy(pointTrackList[0].gameObject);
                    pointTrackList.RemoveAt(0);
                }
            }
        }
    }
}