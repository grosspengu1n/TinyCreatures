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
    public string stateBelly;
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
        upgradeBody = "neutral";
        upgradeTail = "neutral";
        upgradeWings = "neutral";
        upgradeStinger = "neutral";
        stateBelly = "skinny";
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
            
            stinger.Play(upgradeStinger+"_E_stinger_idle");
            legs.Play("neutral_E_legs_fly");
            wings.Play(upgradeWings+"_E_wings_idle");
            belly.Play(stateBelly+"_E_belly_idle");
            tail.Play(upgradeTail+"_E_tail_idle");
            head.Play("neutral_E_head_idle");
            body.Play(upgradeBody+"_E_body_idle");
            stingerSprite.flipX = movement.x < 0;
            legsSprite.flipX = movement.x < 0;
            wingsSprite.flipX = movement.x < 0;
            bellySprite.flipX = movement.x < 0;
            tailSprite.flipX = movement.x < 0;
            headSprite.flipX = movement.x < 0;
            bodySprite.flipX = movement.x < 0;

            //upgrade string has to be set as selected upgrade from upgrade script, respecting animation naming conventions
            //and set transitions, by mapping or animator
            //AND check for the bloodbelly
        }
        if (movement.y != 0 && movement.x==0)
        {

            if (movement.y > 0)
            {
                lastDir = "back";
                stinger.Play(upgradeStinger + "_N_stinger_idle");
                legs.Play("neutral_N_legs_fly");
                wings.Play(upgradeWings + "_N_wings_idle");
                belly.Play(/*stateBelly+*/"mid"+"_N_belly_idle");
                //have to make this invisible when low on blood, need an empty sprite
                tail.Play(upgradeTail + "_N_tail_idle");
                head.Play("neutral_N_head_idle");
                body.Play(upgradeBody + "_N_body_idle");
            }
            if (movement.y < 0)
            {
                lastDir = "front";
                stinger.Play(upgradeStinger + "_S_stinger_idle");
                legs.Play("neutral_S_legs_fly");
                wings.Play(upgradeWings + "_S_wings_idle");
                belly.Play(/*stateBelly+*/"mid" + "_S_belly_idle");
                //have to make this invisible when low on blood, need an empty sprite
                tail.Play(upgradeTail + "_S_tail_idle");
                head.Play("neutral_S_head_idle");
                body.Play(upgradeBody + "_S_body_idle");
            }
        }
        else if (movement.y == 0 && movement.x == 0)
        {
            if (lastDir == "left")
            {
                legs.Play("neutral_E_legs_idle");
            }
            if (lastDir == "right")
            {
                legs.Play("neutral_E_legs_idle");
            }
            if (lastDir == "front")
            {
                legs.Play("neutral_S_legs_idle");
            }
            if (lastDir == "back")
            {
                legs.Play("neutral_N_legs_idle");
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

