using UnityEngine;

// IA básica dos inimigos: persegue o player quando próximo,
// aplica dano por contato com cooldown e dropa um orbe de luz ao morrer.
// NOTA: inimigos dropam luz atualmente — isso será revisado em versão futura
// quando tivermos recursos temáticos próprios de sombra.
public class EnemyCapsuleAI : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 3f;          // Velocidade de perseguição
    public float agroDistance = 20f;  // Distância para começar a perseguir
    public float stopDistance = 1f;   // Distância mínima do player (não fica em cima)

    [Header("Dano")]
    public float damageAmount = 400f;   // Luz drenada por contato
    public float damageCooldown = 1f;   // Intervalo mínimo entre danos (segundos)

    [Header("Drop")]
    public GameObject lightBallPrefab; // Orbe dropado ao morrer

    [Header("Referências")]
    public EnemyQuadAnimator anim;     // Animador de sprite do inimigo

    private Transform player;
    private float damageTimer = 0f;
    private bool isDead = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("EnemyCapsuleAI: Player não encontrado. Verifique a tag 'Player'.");

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
            // Move em direção ao player ignorando o eixo Y
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

    // CORREÇÃO: unificado em um único método para evitar dano duplo.
    // Anteriormente havia OnTriggerEnter + OnCollisionEnter + OnCollisionStay
    // que poderiam acionar o dano simultaneamente no mesmo frame.
    // Agora só o Trigger é usado — ajuste o Collider do inimigo para Is Trigger = true.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            DamagePlayer();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
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

    // Chamado externamente pelo BulletProjectile ao acertar o inimigo
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Dropa o orbe na posição atual
        if (lightBallPrefab != null)
            Instantiate(lightBallPrefab, transform.position, Quaternion.identity);

        // Toca animação de morte e destrói o objeto ao terminar
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
