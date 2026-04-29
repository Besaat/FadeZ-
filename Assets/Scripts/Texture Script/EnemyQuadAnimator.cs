using UnityEngine;

public class EnemyQuadAnimator : MonoBehaviour
{
    public Renderer rend;

    [Header("Animações")]
    public Texture[] idleFrames;
    public Texture[] walkFrames;
    public Texture[] deathFrames;

    [Header("Config")]
    public float fps = 8f;

    private Texture[] currentFrames;
    private int index;
    private float timer;
    private bool isDead = false;
    private bool deathFinished = false;

    public System.Action onDeathComplete;

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
        if (!force && currentFrames == frames) return;
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
        SetAnimation(deathFrames, true);
    }

    // Flip to face direction
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