using UnityEngine;

public class EnemyCapsuleAI : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 3f;
    public float agroDistance = 20f;
    public float stopDistance = 1f;

    [Header("Dano")]
    public float damageAmount = 400f;
    public float damageCooldown = 1f;
    private float damageTimer = 0f;

    [Header("Drop")]
    public GameObject lightBallPrefab;

    [Header("ReferÃªncias")]
    public EnemyQuadAnimator anim;

    private Transform player;
    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (anim == null)
            anim = GetComponentInChildren<EnemyQuadAnimator>();
    }

    void Update()
    {
        if (isDead || player == null) return;

        damageTimer -= Time.deltaTime;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < agroDistance && dist > stopDistance)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            transform.position += dir * speed * Time.deltaTime;

            // Walk animation + flip
            anim.PlayWalk();
            anim.FaceDirection(dir.x);
        }
        else
        {
            anim.PlayIdle();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            DamagePlayer();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
            DamagePlayer();
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
            DamagePlayer();
    }

    void DamagePlayer()
    {
        if (isDead || damageTimer > 0f) return;
        damageTimer = damageCooldown;

        LightAmmo ammo = FindFirstObjectByType<LightAmmo>();
        if (ammo != null)
            ammo.Drain(damageAmount);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Drop light ball
        if (lightBallPrefab != null)
            Instantiate(lightBallPrefab, transform.position, Quaternion.identity);

        // Play death animation then destroy
        if (anim != null)
        {
            anim.PlayDeath();
            anim.onDeathComplete = () => Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
