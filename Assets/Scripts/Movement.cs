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

    public Animator body;
    public Animator head;
    public Animator tail;
    public Animator belly;
    public Animator wings;
    public Animator legs;
    public Animator stinger;
    public SpriteRenderer bodySprite;
    public SpriteRenderer headSprite;
    public SpriteRenderer tailSprite;
    public SpriteRenderer bellySprite;
    public SpriteRenderer wingsSprite;
    public SpriteRenderer legsSprite;
    public SpriteRenderer stingerSprite;
    public string upgradeBody;
    public string upgradeTail;
    public string upgradeWings;
    public string upgradeStinger;
    private string lastDir = null;

    public string state;
    public GameObject interactIcon;
    public bool nearTV, nearLair;
    public GameObject feedScreen, upgradeScreen;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
            if (movement.x < 0)
            {
                lastDir = "left";
            }
            if (movement.x > 0)
            {
                lastDir = "right";
            }
            
            /*if (movement.x < 0) 
            {
                float time = Map(currentVelocity.x, maxSpeed, -maxSpeed, 0, 1, true);
                legs.Play("S_FlyTransition_Legs", 0, time);
            }*/
            /*if (movement.x > 0)
            {
                float time = Map(currentVelocity.x, -maxSpeed, maxSpeed, 0, 1, true);
                legs.Play("S_FlyTransition_Legs", 0, time);
            }*/

            //animator.Play(upgradePart+"View_State_Part")
            stinger.Play(upgradeStinger+"S_N_Stinger");
            legs.Play("S_Fly_Legs");
            wings.Play(upgradeWings+"S_N_Wings");
            belly.Play("S_N_Belly");
            tail.Play(upgradeTail+"S_N_Tail");
            head.Play("S_N_Head");
            body.Play(upgradeBody+"S_N_Body");
            stingerSprite.flipX = movement.x < 0;
            legsSprite.flipX = movement.x < 0;
            wingsSprite.flipX = movement.x < 0;
            bellySprite.flipX = movement.x < 0;
            tailSprite.flipX = movement.x < 0;
            headSprite.flipX = movement.x < 0;
            bodySprite.flipX = movement.x < 0;
            //Now have to check for upgrade parts
            //and set transitions, by mapping?
            //AND check for the bloodbelly
        }
        if (movement.y != 0 && movement.x==0)
        {

            if (movement.y > 0)
            {
                lastDir = "back";
                stinger.Play(upgradeStinger + "B_N_Stinger");
                legs.Play("B_Fly_Legs");
                wings.Play(upgradeWings + "B_N_Wings");
                belly.Play("B_N_MidBelly");
                //have to make this invisible when low on blood, need an empty sprite
                tail.Play(upgradeTail + "B_N_Tail");
                head.Play("B_N_Head");
                body.Play(upgradeBody + "B_N_Body");
            }
            if (movement.y < 0)
            {
                lastDir = "front";
                stinger.Play(upgradeStinger + "F_N_Stinger");
                legs.Play("F_Fly_Legs");
                wings.Play(upgradeWings + "F_N_Wings");
                belly.Play("F_N_MidBelly");
                //have to make this invisible when low on blood, need an empty sprite
                tail.Play(upgradeTail + "F_N_Tail");
                head.Play("F_N_Head");
                body.Play(upgradeBody + "F_N_Body");
            }
        }
        else if (movement.y == 0 && movement.x == 0)
        {
            if (lastDir == "left")
            {
                legs.Play("S_N_Legs");
            }
            if (lastDir == "right")
            {
                legs.Play("S_N_Legs");
            }
            if (lastDir == "front")
            {
                legs.Play("F_N_Legs");
            }
            if (lastDir == "back")
            {
                legs.Play("B_N_Legs");
            }
        }
        isMoving = movement.magnitude > 0;

        if (nearLair)
        {
            if (Input.GetKey("Space"))
            {

            }
        }

        if (nearTV)
        {
            if (Input.GetKey("Space"))
            {
                upgradeScreen.SetActive(true);
            }
        }
    }
    public void UpdateSpeed(float newSpeed)
    {
        maxSpeed = newSpeed;
    }

    private void FixedUpdate()
    {
        if (state!= "stuck" || state!= "hypno" || state!= "locked")
        {
            Move();
        }

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

    public static float Map(float value, float min1, float max1, float min2, float max2, bool clamp = false)
    {
        float val = min2 + (max2 - min2) * ((value - min1) / (max1 - min1));

        return clamp ? Mathf.Clamp(val, Mathf.Min(min2, max2), Mathf.Max(min2, max2)) : val;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TV"))
        {
            interactIcon.SetActive(true);
            nearTV = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("TV"))
        {
            interactIcon.SetActive(false);
            nearTV = false;
        }
    }
}

