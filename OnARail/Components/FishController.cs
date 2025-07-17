using UnityEngine;

namespace OnARail.Components
{
    public class FishController : MonoBehaviour
    {
        [Header("Rotation Timing")]
        public AnimationCurve easingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public float minRotationDuration = 15f;
        public float maxRotationDuration = 30f;
        public float endAngleThreshold = 2f;

        [Header("Fishy Settings")]
        public Transform fishTransform;
        public float fishRotationSpeed = 6f;
        public float fishDirectionSpeed = 6f;
        public float minY = 41f;
        public float maxY = 49f;
        public float heightLerpSpeed = 0.5f;

        [Header("Obstacle Detection")]
        public LayerMask obstacleLayers = 1 << 10 | 1 << 12;
        public float obstacleCheckDistance = 8f;
        public float obstacleCheckCooldown = 0.5f;

        private Quaternion startRotation;
        private Quaternion targetRotation;
        private float rotationDuration;
        private float rotationTimer;
        private float obstacleLastCheckTime = -1f;

        private Vector3 fishDirectionSmoothed;
        private Quaternion fishRotationSmoothed;
        private Vector3 previousFishPosition;
        private Vector3 previousOriginPosition;
        private float targetYPosition;

        private void Awake()
        {
            fishTransform = transform.GetChild(0);
        }

        private void Start()
        {
            previousFishPosition = fishTransform.position;
            previousOriginPosition = transform.position;
            fishDirectionSmoothed = fishTransform.forward;
            fishRotationSmoothed = fishTransform.rotation;
            targetYPosition = fishTransform.localPosition.y;

            PickNewRotation();
        }

        private void Update()
        {
            RotateParent();
            RotateFish();
            AdjustFishHeight();
            ObstacleDetection();
        }

        private void RotateParent()
        {
            rotationTimer += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, easingCurve.Evaluate(Mathf.Clamp01(rotationTimer / rotationDuration)));

            if (Quaternion.Angle(transform.rotation, targetRotation) < endAngleThreshold)
            {
                PickNewRotation();
            }
        }

        private void RotateFish()
        {
            Vector3 adjustedMovement = (fishTransform.position - previousFishPosition) - (transform.position - previousOriginPosition);
            Vector3 movementDir = adjustedMovement.normalized;

            if (movementDir.sqrMagnitude > 0.0001f)
            {
                fishDirectionSmoothed = Vector3.Slerp(fishDirectionSmoothed, movementDir, Time.deltaTime * fishDirectionSpeed);
                fishRotationSmoothed = Quaternion.Slerp(fishRotationSmoothed, Quaternion.LookRotation(fishDirectionSmoothed, transform.up), Time.deltaTime * fishRotationSpeed);
                fishTransform.rotation = fishRotationSmoothed;
            }

            previousFishPosition = fishTransform.position;
            previousOriginPosition = transform.position;
        }

        private void AdjustFishHeight()
        {
            Vector3 localPos = fishTransform.localPosition;
            localPos.y = Mathf.Lerp(localPos.y, targetYPosition, Time.deltaTime * heightLerpSpeed);
            fishTransform.localPosition = localPos;
        }

        private void ObstacleDetection()
        {
            if (Time.time - obstacleLastCheckTime < obstacleCheckCooldown) return;

            Vector3 origin = fishTransform.position;
            Vector3 direction = fishDirectionSmoothed.normalized;

            if (Physics.SphereCast(origin, 1f, direction, out RaycastHit hit, obstacleCheckDistance, obstacleLayers))
            {
                obstacleLastCheckTime = Time.time;
                PickNewRotation();
            }
        }

        private void PickNewRotation()
        {
            startRotation = transform.rotation;
            targetRotation = Random.rotation;
            rotationDuration = Random.Range(minRotationDuration, maxRotationDuration);
            rotationTimer = 0f;

            fishDirectionSmoothed = fishTransform.forward;
            targetYPosition = Random.Range(minY, maxY);
        }
    }
}