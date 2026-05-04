using UnityEngine;

// Responsável pela movimentação do player via teclado (WASD / setas).
// Usa Rigidbody para mover fisicamente e evitar conflitos com colisões.
public class PlayerMove : MonoBehaviour
{
    [Header("Configuração")]
    public float speed = 3f;

    [Header("Referências")]
    public QuadAnimator anim; // Animador de sprite do player

    private Rigidbody rb;
    private Vector3 moveInput; // Direção de movimento bruta do teclado

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Captura input do teclado
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        moveInput = new Vector3(h, 0f, v);

        HandleAnimation(h);
        HandleFlip(h);
    }

    void FixedUpdate()
    {
        // Move o Rigidbody de forma normalizada para velocidade consistente em diagonais
        if (moveInput.sqrMagnitude > 0.01f)
            rb.MovePosition(rb.position + moveInput.normalized * speed * Time.fixedDeltaTime);
    }

    // Troca entre animação idle e walk conforme o movimento
    void HandleAnimation(float h)
    {
        if (anim == null) return;

        if (moveInput.sqrMagnitude > 0.01f)
            anim.PlayWalk();
        else
            anim.PlayIdle();
    }

    // Vira o sprite do player conforme a direção horizontal
    // Usa apenas o sinal do eixo X para evitar escala acidental no Y/Z
    void HandleFlip(float h)
    {
        if (h > 0.1f)
            transform.localScale = new Vector3(3f, 3f, 3f);
        else if (h < -0.1f)
            transform.localScale = new Vector3(-3f, 3f, 3f);
    }
}
