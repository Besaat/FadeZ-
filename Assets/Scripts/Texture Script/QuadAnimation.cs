using UnityEngine;

public class QuadAnimator : MonoBehaviour
{
    public Renderer rend;

    [Header("Animações")]
    public Texture[] idleFrames;
    public Texture[] walkFrames;
    public Texture[] runFrames;
    public Texture[] deathFrames;

    [Header("Config")]
    public float fps = 8f;

    private Texture[] currentFrames;
    private int index;
    private float timer;
    private bool isDead = false;
    private bool deathFinished = false;

    // Callback when death animation ends
    public System.Action onDeathComplete;

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
            timer = 0;

            // If death animation, don't loop — stop at last frame
            if (isDead)
            {
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
        rend.material.mainTexture = currentFrames[0];
    }
}