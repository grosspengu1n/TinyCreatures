using UnityEngine;
using System.Collections;

public class SpiderEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float walkDuration = 2f;
    public float idleDuration = 1f;

    [Header("Combat")]
    public GameObject projectilePrefab;
    public float shootingRange = 5f;
    public float shootCooldown = 2f;
    public float projectileSpeed = 5f;

    private Transform player;
    private Rigidbody2D rb;
    private bool isWalking = false;
    private float lastShootTime;
    private Vector2 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(MovementRoutine());
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) <= shootingRange)
        {
            TryShoot();
        }

        if (isWalking)
        {
            rb.velocity = moveDirection * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void TryShoot()
    {
        if (Time.time - lastShootTime >= shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }

    void Shoot()
    {
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        projectileRb.velocity = direction * projectileSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    IEnumerator MovementRoutine()
    {
        while (true)
        {
            isWalking = false;
            yield return new WaitForSeconds(idleDuration);

            isWalking = true;
            moveDirection = Random.insideUnitCircle.normalized;
            yield return new WaitForSeconds(walkDuration);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
