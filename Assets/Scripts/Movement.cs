using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float deceleration = 2f;
    [SerializeField] private float minMovementThreshold = 0.1f; 

    private Vector2 movement;
    private Vector2 currentVelocity;
    private bool isMoving;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
        rb.drag = 0f;
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();

        if (movement.x != 0)
        {
            spriteRenderer.flipX = movement.x < 0;
        }

        isMoving = movement.magnitude > 0;
    }
    public void UpdateSpeed(float newSpeed)
    {
        maxSpeed = newSpeed;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 targetVelocity;
        float currentSpeed = currentVelocity.magnitude;

        if (isMoving)
        {
            targetVelocity = movement * maxSpeed;
            currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            if (currentSpeed > minMovementThreshold)
            {
                targetVelocity = Vector2.zero;
                currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                currentVelocity = Vector2.zero;
            }
        }

        rb.velocity = currentVelocity;
    }
    void OnGUI()
    {
        GUILayout.Label($"Speed: {currentVelocity.magnitude:F2}");
    }
}

