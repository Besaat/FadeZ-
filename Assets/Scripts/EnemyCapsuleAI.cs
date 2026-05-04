using UnityEngine;
using System.Collections;

// IA dos inimigos: persegue o player, aplica dano por contato e
// executa um dash em direção ao player com brilho vermelho.
public class EnemyCapsuleAI : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 3f;
    public float agroDistance = 20f;
    public float stopDistance = 1f;

    [Header("Dano por contato")]
    public float damageAmount = 150f;
    public float damageCooldown = 1f;

    [Header("Dash do inimigo")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 3f;
    public float dashWindup = 0.5f;
    public float dashTriggerDistance = 6f;

    [Header("Drop")]
    public GameObject lightBallPrefab;

    [Header("Referências")]
    public EnemyQuadAnimator anim;

    private Transform player;
    private float damageTimer = 0f;
    private float dashTimer = 0f;
    private bool isDashing = false;
    private bool isDead = false;

    // Cor original do sprite — salva no Start para restaurar após o windup
    private SpriteRenderer spriteRend;
    private Color originalColor;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (anim == null) anim = GetComponentInChildren<EnemyQuadAnimator>();

        // Busca o SpriteRenderer no filho para o efeito de cor do windup
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        if (spriteRend != null) originalColor = spriteRend.color;
    }

    void Update()
    {
        if (isDead || player == null || isDashing) return;

        damageTimer -= Time.deltaTime;
        dashTimer -= Time.deltaTime;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < dashTriggerDistance && dashTimer <= 0f)
        {
            StartCoroutine(DoDash());
            return;
        }

        if (dist < agroDistance && dist > stopDistance)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0f;
            transform.position += dir * speed * Time.deltaTime;

            if (anim != null)
            {
                anim.PlayWalk();
                anim.FaceDirection(dir.x);
            }
        }
        else
        {
            if (anim != null) anim.PlayIdle();
        }
    }

    // Dash com windup vermelho → move rapidamente em direção ao player
    IEnumerator DoDash()
    {
        isDashing = true;
        dashTimer = dashCooldown;

        // Fase de windup: tinge o SpriteRenderer de vermelho
        // Usa SpriteRenderer.color em vez de material.color para compatibilidade com Unlit
        if (spriteRend != null)
            spriteRend.color = new Color(1f, 0.2f, 0.2f, 1f);

        yield return new WaitForSeconds(dashWindup);

        // Restaura a cor original
        if (spriteRend != null)
            spriteRend.color = originalColor;

        if (isDead) { isDashing = false; yield break; }

        // Calcula direção no momento do dash (não no windup)
        Vector3 dashDir = player != null
            ? (player.position - transform.position).normalized
            : transform.forward;
        dashDir.y = 0f;

        float elapsed = 0f;
        while (elapsed < dashDuration && !isDead)
        {
            transform.position += dashDir * dashSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) DamagePlayer();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) DamagePlayer();
    }

    void DamagePlayer()
    {
        if (isDead || damageTimer > 0f) return;

        PlayerMove pm = FindFirstObjectByType<PlayerMove>();
        if (pm != null && pm.isInvincible) return;

        damageTimer = damageCooldown;
        LightAmmo ammo = FindFirstObjectByType<LightAmmo>();
        if (ammo != null) ammo.Drain(damageAmount);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        StopAllCoroutines();

        // Restaura a cor antes de morrer para evitar sprite vermelho travado
        if (spriteRend != null)
            spriteRend.color = originalColor;

        if (lightBallPrefab != null)
            Instantiate(lightBallPrefab, transform.position, Quaternion.identity);

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
