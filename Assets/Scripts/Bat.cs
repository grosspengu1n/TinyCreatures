using UnityEngine;

public class Bat : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float circleRadius = 3f;
    [SerializeField] private float changeDirectionInterval = 3f;
    [SerializeField] private float smoothTransitionDuration = 1f;

    private Vector2 currentCenter;
    private float currentAngle;
    private float currentAngleSpeed;
    private float targetAngleSpeed;
    private float timeUntilDirectionChange;
    private float transitionTime;
    private float currentRadius;
    private float targetRadius;

    void Start()
    {
        // Initialize starting position and movement
        currentCenter = transform.position;
        currentAngle = Random.Range(0f, 360f);
        currentAngleSpeed = GetNewAngleSpeed();
        targetAngleSpeed = currentAngleSpeed;
        currentRadius = circleRadius;
        targetRadius = circleRadius;
        timeUntilDirectionChange = changeDirectionInterval;
    }

    void Update()
    {
        // Update timer and change direction if needed
        timeUntilDirectionChange -= Time.deltaTime;
        if (timeUntilDirectionChange <= 0)
        {
            ChangeDirection();
            timeUntilDirectionChange = changeDirectionInterval;
        }

        // Smooth transition of movement parameters
        if (transitionTime < smoothTransitionDuration)
        {
            transitionTime += Time.deltaTime;
            float t = transitionTime / smoothTransitionDuration;
            t = Mathf.SmoothStep(0f, 1f, t); // Use smoothstep for even smoother transition

            // Lerp between current and target values
            currentAngleSpeed = Mathf.Lerp(currentAngleSpeed, targetAngleSpeed, t);
            currentRadius = Mathf.Lerp(currentRadius, targetRadius, t);
        }

        // Update angle and position
        currentAngle += currentAngleSpeed * Time.deltaTime;

        // Calculate new position
        Vector2 offset = new Vector2(
            Mathf.Cos(currentAngle) * currentRadius,
            Mathf.Sin(currentAngle) * currentRadius
        );

        // Move the center point slightly for more randomness
        currentCenter += (Vector2)transform.right * moveSpeed * Time.deltaTime;

        // Update bat position
        Vector2 newPosition = currentCenter + offset;
        transform.position = newPosition;

        // Smoothly rotate the bat to face its movement direction
        float targetAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        float currentRotation = transform.eulerAngles.z;
        float newRotation = Mathf.LerpAngle(currentRotation, targetAngle, Time.deltaTime * 5f);
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
    }

    private void ChangeDirection()
    {
        targetAngleSpeed = GetNewAngleSpeed();

        // Occasionally change circle radius for variety
        if (Random.value > 0.7f)
        {
            targetRadius = Random.Range(2f, 4f);
        }

        // Reset transition timer
        transitionTime = 0f;
    }

    private float GetNewAngleSpeed()
    {
        return Random.Range(1f, 3f) * (Random.value > 0.5f ? 1 : -1);
    }
}