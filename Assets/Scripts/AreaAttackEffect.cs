using UnityEngine;

// Toca uma animação de ataque em área usando uma spritesheet fatiada.
// Instanciado pelo PlayerShoot no momento do ataque de área.
// A animação toca uma vez e o objeto se destrói ao terminar.
// O sprite é rotacionado para ficar deitado (visão isométrica).
// O movimento do player fica travado durante a animação.
public class AreaAttackEffect : MonoBehaviour
{
    [Header("Spritesheet")]
    public Sprite[] frames;         // Arraste os frames fatiados da spritesheet aqui
    public float fps = 12f;         // Velocidade da animação

    private SpriteRenderer sr;
    private int currentFrame = 0;
    private float timer = 0f;
    private bool finished = false;
    private PlayerMove playerMove;

    // Chamado pelo PlayerShoot após instanciar, passando o raio da área e o PlayerMove
    public void Init(float areaRadius, PlayerMove pm)
    {
        // Rotaciona para ficar deitado na visão isométrica
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // Ajusta a escala para cobrir o diâmetro do ataque de área com o dobro do tamanho
        float size = areaRadius * 4f;
        transform.localScale = new Vector3(size, size, size);

        // Trava o movimento do player durante a animação
        playerMove = pm;
        if (playerMove != null)
            playerMove.enabled = false;
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();

        // Define a opacidade do efeito em 40%
        sr.color = new Color(1f, 1f, 1f, 0.4f);

        if (frames != null && frames.Length > 0)
            sr.sprite = frames[0];
    }

    void Update()
    {
        if (finished || frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;

        if (timer >= 1f / fps)
        {
            timer = 0f;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                finished = true;

                // Destrava o movimento do player ao terminar a animação
                if (playerMove != null)
                    playerMove.enabled = true;

                Destroy(gameObject);
                return;
            }

            sr.sprite = frames[currentFrame];
        }
    }
}
