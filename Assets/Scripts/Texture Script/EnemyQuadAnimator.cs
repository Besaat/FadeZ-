using UnityEngine;

// Animador de sprite para inimigos — funciona igual ao QuadAnimator do player,
// mas separado para permitir customizações específicas dos inimigos no futuro.
// Inclui FaceDirection para virar o sprite conforme a direção de movimento.
public class EnemyQuadAnimator : MonoBehaviour
{
    [Header("Referências")]
    public Renderer rend;

    [Header("Frames de animação")]
    public Texture[] idleFrames;
    public Texture[] walkFrames;
    public Texture[] deathFrames;

    [Header("Configuração")]
    public float fps = 8f;

    // Callback chamado quando a animação de morte termina (usado pelo EnemyCapsuleAI)
    public System.Action onDeathComplete;

    private Texture[] currentFrames;
    private int index;
    private float timer;
    private bool isDead = false;
    private bool deathFinished = false;

    void Start()
    {
        if (rend == null)
            rend = GetComponentInChildren<Renderer>();
        PlayIdle();
    }

    void Update()
    {
        if (rend == null || currentFrames == null || currentFrames.Length == 0) return;

        timer += Time.deltaTime;

        if (timer >= 1f / fps)
        {
            timer = 0f;

            if (isDead)
            {
                // Sem loop na morte — para no último frame e dispara o callback
                if (index < deathFrames.Length - 1)
                {
                    index++;
                    rend.material.mainTexture = currentFrames[index];
                }
                else if (!deathFinished)
                {
                    deathFinished = true;
                    onDeathComplete?.Invoke();
                }
                return;
            }

            index = (index + 1) % currentFrames.Length;
            rend.material.mainTexture = currentFrames[index];
        }
    }

    void SetAnimation(Texture[] frames, bool force = false)
    {
        if (frames == null || frames.Length == 0) return;
        if (!force && currentFrames == frames) return; // Evita reiniciar a mesma animação
        currentFrames = frames;
        index = 0;
        timer = 0f;
        rend.material.mainTexture = currentFrames[0];
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

    public void PlayDeath()
    {
        if (isDead) return;
        isDead = true;
        SetAnimation(deathFrames, force: true);
    }

    // Vira o sprite do inimigo conforme a direção horizontal de movimento
    public void FaceDirection(float horizontalDir)
    {
        if (isDead) return;

        Vector3 scale = transform.localScale;
        float absX = Mathf.Abs(scale.x);

        if (horizontalDir > 0.1f)
            transform.localScale = new Vector3(absX, scale.y, scale.z);
        else if (horizontalDir < -0.1f)
            transform.localScale = new Vector3(-absX, scale.y, scale.z);
    }
}
