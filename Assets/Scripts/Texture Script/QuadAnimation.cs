using UnityEngine;

// Animador de sprite para o player usando troca de texturas em um quad (plano).
// Simula animação 2D num ambiente 3D trocando a textura do material a cada frame.
// As texturas de cada animação são definidas no Inspector.
public class QuadAnimator : MonoBehaviour
{
    [Header("Referências")]
    public Renderer rend; // Renderer do quad (plano com o sprite)

    [Header("Frames de animação")]
    public Texture[] idleFrames;  // Frames da animação parada
    public Texture[] walkFrames;  // Frames da animação andando
    public Texture[] runFrames;   // Frames da animação correndo (reservado para futuro)
    public Texture[] deathFrames; // Frames da animação de morte

    [Header("Configuração")]
    public float fps = 8f; // Frames por segundo da animação

    // Callback chamado quando a animação de morte termina (usado pelo PlayerDeath)
    public System.Action onDeathComplete;

    private Texture[] currentFrames;
    private int index;
    private float timer;
    private bool isDead = false;
    private bool deathFinished = false;

    void Start()
    {
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
                // Animação de morte não faz loop — para no último frame
                if (index < currentFrames.Length - 1)
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

            // Animações normais fazem loop
            index = (index + 1) % currentFrames.Length;
            rend.material.mainTexture = currentFrames[index];
        }
    }

    public void PlayIdle()
    {
        if (isDead || currentFrames == idleFrames) return;
        currentFrames = idleFrames;
        index = 0;
    }

    public void PlayWalk()
    {
        if (isDead || currentFrames == walkFrames) return;
        currentFrames = walkFrames;
        index = 0;
    }

    public void PlayRun()
    {
        if (isDead || currentFrames == runFrames) return;
        currentFrames = runFrames;
        index = 0;
    }

    public void PlayDeath()
    {
        if (isDead) return;
        isDead = true;
        currentFrames = deathFrames;
        index = 0;

        // Aplica o primeiro frame da morte imediatamente
        if (rend != null && currentFrames != null && currentFrames.Length > 0)
            rend.material.mainTexture = currentFrames[0];
    }
}
