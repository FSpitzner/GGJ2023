using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VFX;

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
        [SerializeField] VisualEffect launchEffect = null;
        [SerializeField] VisualEffect landEffect = null;

        [SerializeField] SkinnedMeshRenderer playerMesh = null;
        [SerializeField] GameObject projectile = null;
        [SerializeField] GameObject contactPointPrefab;
        [SerializeField] float heightOffsetProjectile = .75f;
        [SerializeField] LayerMask groundLayermask = 0;

        [SerializeField] private Arrow arrow;

        [SerializeField] float spreadRadius = 3f;
        [SerializeField] private float tiltShapeKeySpeed = 1f;
        [SerializeField] private float throwShapeKeySpeed = 1f;

        [SerializeField] private int health = 3;

        private Transform owntransform = null;
        private Vector3 direction = new Vector3();
        private float powerTimer = 0f;
        private Transform projectileTransform = null;
        private Ray groundRay;
        private RaycastHit hit;

        private List<PlayerContactPoint> pointTrackList = new List<PlayerContactPoint>();

        private float tiltShapeKeyPos = 0;
        private float throwShapeKeyPos = 0;

        private bool flying = false;

        private void Start()
        {
            UpdateIndicator(0f);
            owntransform = transform;
            projectileTransform = projectile.transform;
            groundRay = new Ray(projectileTransform.position, Vector3.down);

            // Apply initial overgrowth radius the player starts at:
            roomStateTracker.ApplyPlayerStartImpact(transform.position);
        }

        void Update()
        {
            direction = new Vector3(input.Direction.x, 0, input.Direction.y);
            if (direction.magnitude > 0.1f)
            {
                arrow.SetDirection(direction);
                arrow.SetScale(1);
                playerMesh.transform.forward = -direction;

                direction.y = 0.75f;

                if (input.JumpActive)
                {
                    powerTimer += Time.deltaTime;
                    if (powerTimer > maxPowerAfterSeconds)
                        powerTimer = maxPowerAfterSeconds;
                    float powerNormalized = powerTimer.RemapExclusive(0, maxPowerAfterSeconds, 0, 1f);

                    throwShapeKeyPos = Mathf.Lerp(0, -40, powerTimer / maxPowerAfterSeconds);
                    playerMesh.SetBlendShapeWeight(1, throwShapeKeyPos);

                    UpdateIndicator(powerNormalized);
                }
                else if (!input.JumpActive && powerTimer > 0f)
                {
                    float powerNormalized = powerTimer.RemapExclusive(0, maxPowerAfterSeconds, 0, 1f);
                    Jump(direction, powerNormalized);
                    powerTimer = 0f;

                    throwShapeKeyPos = 0;
                    playerMesh.SetBlendShapeWeight(1, throwShapeKeyPos);

                    UpdateIndicator(0f);
                }

                if (tiltShapeKeyPos < 100)
                {
                    tiltShapeKeyPos += tiltShapeKeySpeed * Time.deltaTime;
                    if (tiltShapeKeyPos > 100)
                        tiltShapeKeyPos = 100;
                }
            }
            else
            {
                if(tiltShapeKeyPos > 0)
                {
                    tiltShapeKeyPos -= tiltShapeKeySpeed * Time.deltaTime;
                    if (tiltShapeKeyPos < 0)
                        tiltShapeKeyPos = 0;
                }
            }

            playerMesh.SetBlendShapeWeight(0, tiltShapeKeyPos);

            if (flying)
            {
                projectileTransform.up = rb.velocity;
            }
        }

        void Jump(Vector3 direction, float power)
        {
            // Tell the tutorial UI that the player has moved:
            if (References.ingameHud != null)
                References.ingameHud.ControlsPanel.RegisterPlayerMovement();

            StartCoroutine(JumpNumerator(direction, power));
        }

        private IEnumerator JumpNumerator(Vector3 direction, float power)
        {
            launchEffect.Play();

            while(throwShapeKeyPos < 100)
            {
                throwShapeKeyPos += throwShapeKeySpeed * Time.deltaTime;
                playerMesh.SetBlendShapeWeight(1, throwShapeKeyPos);

                yield return new WaitForEndOfFrame();
            }

            direction = new Vector3(input.Direction.x, 0.75f, input.Direction.y);

            throwShapeKeyPos = 0;
            playerMesh.SetBlendShapeWeight(1, throwShapeKeyPos);

            projectile.SetActive(true);
            playerMesh.gameObject.SetActive(false);
            arrow.SetActive(false);
            rb.AddRelativeForce(direction * power * impulsPowerModifier, ForceMode.Impulse);
            //launchEffect.Play();

            flying = true;
        }

        void UpdateIndicator(float power)
        {
            References.ingameHud.JumpPowerBar.Fill = power;
            /*powerIndicator.fillAmount = power;*/
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

                playerMesh.gameObject.SetActive(true);
                projectile.SetActive(false);
                arrow.SetActive(true);

                landEffect.Play();

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

                flying = false;
            }
        }
    }
}