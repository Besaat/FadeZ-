using UnityEngine;
using System.Collections;

// Responsável pela movimentação, corrida, dash e flip do player.
// Controles:
// - WASD: mover
// - Shift: correr (velocidade maior, animação de run) — requer runEnabled = true
// - Space: dash na direção do movimento (invencível durante o dash) — requer dashEnabled = true
public class PlayerMove : MonoBehaviour
{
    [Header("Movimento")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;         // Velocidade ao segurar Shift
    public float stopDamping = 25f;     // Quanto mais alto, mais rápido para ao soltar a tecla

    [Header("Dash")]
    public bool dashEnabled = false;    // Desativado na base; ativado ao entrar na dungeon
    public float dashSpeed = 18f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.8f;
    public KeyCode dashKey = KeyCode.Space;

    [Header("Corrida")]
    public bool runEnabled = false;     // Desativado na base; ativado ao entrar na dungeon

    [Header("Rastro do Dash (Ghost)")]
    public GameObject dashGhostPrefab;  // Prefab com SpriteRenderer + DashGhost
    public int ghostCount = 6;          // Quantidade de ghosts gerados durante o dash
    public float ghostLifetime = 0.2f;  // Tempo que cada ghost leva para sumir
    public Color ghostColor = new Color(1f, 1f, 1f, 0.6f); // Cor inicial do ghost

    [Header("Referências")]
    public QuadAnimator anim;
    public SpriteRenderer spriteRenderer;

    [HideInInspector] public bool isInvincible = false;

    private Rigidbody rb;
    private Vector3 moveInput;
    private bool isDashing = false;
    private float dashCooldownTimer = 0f;
    private Vector3 lastMoveDirection = Vector3.forward;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (isDashing) return;

        dashCooldownTimer -= Time.deltaTime;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        moveInput = new Vector3(h, 0f, v);

        if (moveInput.sqrMagnitude > 0.01f)
            lastMoveDirection = moveInput.normalized;

        HandleFlip(h);
        HandleAnimation();

        if (dashEnabled && Input.GetKeyDown(dashKey) && dashCooldownTimer <= 0f)
            StartCoroutine(DoDash());
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            // Corrida só ocorre se runEnabled estiver ativo
            float currentSpeed = (runEnabled && Input.GetKey(KeyCode.LeftShift)) ? runSpeed : walkSpeed;
            rb.MovePosition(rb.position + moveInput.normalized * currentSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, stopDamping * Time.fixedDeltaTime);
        }
    }

    IEnumerator DoDash()
    {
        isDashing = true;
        isInvincible = true;
        dashCooldownTimer = dashCooldown;

        if (anim != null) anim.PlayRun();

        float elapsed = 0f;
        float ghostInterval = dashGhostPrefab != null ? dashDuration / ghostCount : float.MaxValue;
        float nextGhostTime = 0f;

        while (elapsed < dashDuration)
        {
            rb.MovePosition(rb.position + lastMoveDirection * dashSpeed * Time.fixedDeltaTime);

            if (dashGhostPrefab != null && elapsed >= nextGhostTime)
            {
                SpawnGhost();
                nextGhostTime += ghostInterval;
            }

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
        isInvincible = false;
    }

    void SpawnGhost()
    {
        if (spriteRenderer == null) return;

        GameObject ghost = Instantiate(dashGhostPrefab, spriteRenderer.transform.position, spriteRenderer.transform.rotation);
        ghost.transform.localScale = spriteRenderer.transform.lossyScale;

        DashGhost dg = ghost.GetComponent<DashGhost>();
        if (dg != null)
            dg.Init(spriteRenderer.sprite, spriteRenderer.flipX, ghostColor, ghostLifetime);
    }

    void HandleAnimation()
    {
        if (anim == null) return;
        if (isDashing) return;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            if (runEnabled && Input.GetKey(KeyCode.LeftShift))
                anim.PlayRun();
            else
                anim.PlayWalk();
        }
        else
        {
            anim.PlayIdle();
        }
    }

    void HandleFlip(float h)
    {
        if (spriteRenderer == null) return;
        if (h > 0.1f) spriteRenderer.flipX = false;
        else if (h < -0.1f) spriteRenderer.flipX = true;
    }
}
