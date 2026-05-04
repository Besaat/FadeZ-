using UnityEngine;

// Animação simples de sprite por troca de textura em loop.
// Usado em objetos decorativos ou efeitos que precisam de animação básica
// sem necessidade dos estados (idle/walk/death) do QuadAnimator.
public class SpriteAnimation : MonoBehaviour
{
    [Header("Frames")]
    public Texture[] frames;       // Texturas da animação em ordem

    [Header("Configuração")]
    public float frameRate = 10f;  // Frames por segundo

    private Renderer rend;
    private int currentFrame;
    private float timer;

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (frames.Length > 0 && rend != null)
            rend.material.SetTexture("_BaseMap", frames[0]);
    }

    void Update()
    {
        if (frames.Length == 0 || rend == null) return;

        timer += Time.deltaTime;

        if (timer >= 1f / frameRate)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            rend.material.SetTexture("_BaseMap", frames[currentFrame]);
        }
    }
}
