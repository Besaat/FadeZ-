using UnityEngine;

// Animador de sprite para o player usando Sprite[] em vez de Texture[].
// ALTERAÇÃO: trocado de Texture[] para Sprite[] para permitir uso de spritesheets fatiadas.
// O Renderer precisa usar um material com shader que suporte sprites (ex: Unlit/Transparent).
public class QuadAnimator : MonoBehaviour
{
    [Header("Referências")]
    public SpriteRenderer spriteRenderer; // SpriteRenderer do quad do player

    [Header("Frames de animação")]
    public Sprite[] idleFrames;
    public Sprite[] walkFrames;
    public Sprite[] runFrames;
    public Sprite[] hitFrames;
    public Sprite[] deathFrames;

    [Header("Configuração")]
    public float fps = 8f;

    // Callback chamado quando a animação de morte termina
    public System.Action onDeathComplete;

    private Sprite[] currentFrames;
    private int index;
    private float timer;
    private bool isDead = false;
    private bool deathFinished = false;

    void Start()
    {
        // Busca o SpriteRenderer automaticamente se não atribuído
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        PlayIdle();
    }

    void Update()
    {
        if (currentFrames == null || currentFrames.Length == 0) return;

        timer += Time.deltaTime;

        if (timer >= 1f / fps)
        {
            timer = 0f;

            if (isDead)
            {
                // Animação de morte não faz loop
                if (index < currentFrames.Length - 1)
                {
                    index++;
                    ApplyFrame();
                }
                else if (!deathFinished)
                {
                    deathFinished = true;
                    onDeathComplete?.Invoke();
                }
                return;
            }

            // Animações normais fazem loop
            index = (index + 1) % currentFrames.Length;
            ApplyFrame();
        }
    }

    void SetAnimation(Sprite[] frames)
    {
        if (frames == null || frames.Length == 0) return;
        if (currentFrames == frames) return; // Evita reiniciar a mesma animação
        currentFrames = frames;
        index = 0;
        timer = 0f;
        ApplyFrame();
    }

    void ApplyFrame()
    {
        if (spriteRenderer != null && currentFrames[index] != null)
            spriteRenderer.sprite = currentFrames[index];
    }

    // Controla o flip horizontal do sprite
    public void SetFacing(float horizontalDir)
    {
        if (spriteRenderer == null) return;
        if (horizontalDir > 0.1f)
            spriteRenderer.flipX = false;
        else if (horizontalDir < -0.1f)
            spriteRenderer.flipX = true;
    }

    public void PlayIdle()
    {
        if (isDead) return;
        SetAnimation(idleFrames);
    }

    public void PlayWalk()
    {
        if (isDead) return;
        SetAnimation(walkFrames);
    }

    public void PlayRun()
    {
        if (isDead) return;
        SetAnimation(runFrames);
    }

    public void PlayHit()
    {
        if (isDead) return;
        SetAnimation(hitFrames);
    }

    public void PlayDeath()
    {
        if (isDead) return;
        isDead = true;
        currentFrames = deathFrames;
        index = 0;
        timer = 0f;
        ApplyFrame();
    }
}
