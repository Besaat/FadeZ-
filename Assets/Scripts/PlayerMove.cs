using UnityEngine;
using System.Collections;

// Responsável pela movimentação, corrida, dash e flip do player.
// Controles:
// - WASD: mover
// - Shift: correr (velocidade maior, animação de run)
// - Space: dash na direção do movimento (invencível durante o dash)
public class PlayerMove : MonoBehaviour
{
    [Header("Movimento")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;         // Velocidade ao segurar Shift
    public float stopDamping = 25f;     // Quanto mais alto, mais rápido para ao soltar a tecla

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.8f;
    public KeyCode dashKey = KeyCode.Space;

    [Header("Rastro do Dash")]
    public GameObject dashTrailPrefab;  // Prefab do rastro de luz (opcional)
    public int trailCount = 5;          // Quantidade de rastros gerados durante o dash
    public float trailLifetime = 0.3f;  // Tempo de vida de cada rastro

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

        if (Input.GetKeyDown(dashKey) && dashCooldownTimer <= 0f)
            StartCoroutine(DoDash());
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
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
        float trailInterval = dashDuration / trailCount;
        float nextTrailTime = 0f;

        while (elapsed < dashDuration)
        {
            rb.MovePosition(rb.position + lastMoveDirection * dashSpeed * Time.fixedDeltaTime);

            // Spawna rastros de luz ao longo do dash
            if (dashTrailPrefab != null && elapsed >= nextTrailTime)
            {
                GameObject trail = Instantiate(dashTrailPrefab, transform.position, Quaternion.identity);
                Destroy(trail, trailLifetime);
                nextTrailTime += trailInterval;
            }

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
        isInvincible = false;
    }

    void HandleAnimation()
    {
        if (anim == null) return;

        if (isDashing) return; // Animação de run já foi setada no DoDash

        if (moveInput.sqrMagnitude > 0.01f)
        {
            if (Input.GetKey(KeyCode.LeftShift))
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
