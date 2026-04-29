using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    public Texture[] frames;     // suas texturas
    public float frameRate = 10f; // frames por segundo

    private Renderer rend;
    private int currentFrame;
    private float timer;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        if (frames.Length == 0) return;

        timer += Time.deltaTime;

        if (timer >= 1f / frameRate)
        {
            timer = 0f;

            currentFrame++;
            if (currentFrame >= frames.Length)
                currentFrame = 0;

            rend.material.SetTexture("_BaseMap", frames[currentFrame]);
        }
    }
}