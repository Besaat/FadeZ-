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

    [Header("Rastro do Dash (Ghost)")]
    public GameObject dashGhostPrefab;
    public int ghostCount = 6;
    public float ghostLifetime = 0.2f;
    public Color ghostColor = new Color(0f, 0f, 0f, 0.6f); // Preto semitransparente

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
    private Renderer spriteRend;
    private Color originalColor;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (anim == null) anim = GetComponentInChildren<EnemyQuadAnimator>();

        // Busca o Renderer especificamente no filho chamado "Quad"
        Transform quadTransform = transform.Find("Quad");
        if (quadTransform != null)
            spriteRend = quadTransform.GetComponent<Renderer>();

        // Fallback: se não encontrar "Quad", usa o EnemyQuadAnimator para pegar o Renderer
        if (spriteRend == null && anim != null)
            spriteRend = anim.rend;

        if (spriteRend != null)
            originalColor = spriteRend.material.color;
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

        // Fase de windup: tinge o Renderer de vermelho
        if (spriteRend != null)
            spriteRend.material.color = new Color(1f, 0.2f, 0.2f, 1f);

        yield return new WaitForSeconds(dashWindup);

        // Restaura a cor original
        if (spriteRend != null)
            spriteRend.material.color = originalColor;

        if (isDead) { isDashing = false; yield break; }

        // Calcula direção no momento do dash (não no windup)
        Vector3 dashDir = player != null
            ? (player.position - transform.position).normalized
            : transform.forward;
        dashDir.y = 0f;

        float elapsed = 0f;
        float ghostInterval = dashGhostPrefab != null ? dashDuration / ghostCount : float.MaxValue;
        float nextGhostTime = 0f;

        while (elapsed < dashDuration && !isDead)
        {
            transform.position += dashDir * dashSpeed * Time.deltaTime;

            // Spawna ghost na posição atual do inimigo
            if (dashGhostPrefab != null && elapsed >= nextGhostTime)
            {
                SpawnGhost();
                nextGhostTime += ghostInterval;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    void SpawnGhost()
    {
        Debug.Log("SpawnGhost chamado");

        if (spriteRend == null) { Debug.Log("spriteRend é nulo"); return; }

        // Pega a textura atual diretamente do EnemyQuadAnimator
        Texture2D currentTex = null;
        if (anim != null && anim.rend != null)
            currentTex = anim.rend.material.mainTexture as Texture2D;
        else
            Debug.Log("anim ou anim.rend é nulo — anim: " + (anim != null) + " | anim.rend: " + (anim != null && anim.rend != null));

        Debug.Log("currentTex: " + (currentTex != null ? currentTex.name : "NULO"));

        // Só instancia o ghost se tiver uma textura válida
        if (currentTex == null) return;

        GameObject ghost = Instantiate(dashGhostPrefab, spriteRend.transform.position, spriteRend.transform.rotation);

        // Copia a escala do Quad do inimigo
        ghost.transform.localScale = spriteRend.transform.lossyScale;

        // Detecta o flip horizontal pelo sinal da escala X
        bool flipX = spriteRend.transform.lossyScale.x < 0f;

        DashGhost dg = ghost.GetComponent<DashGhost>();
        if (dg != null)
            dg.InitFromTexture(currentTex, flipX, ghostColor, ghostLifetime);
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
            spriteRend.material.color = originalColor;

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
