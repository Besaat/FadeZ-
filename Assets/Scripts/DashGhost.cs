using UnityEngine;

// Cópia fantasma do sprite gerada durante o dash.
// Suporta dois modos:
// - Sprite: usado pelo player (SpriteRenderer)
// - Texture: usado pelos inimigos (MeshRenderer) — recebe a textura diretamente do EnemyQuadAnimator
public class DashGhost : MonoBehaviour
{
    private SpriteRenderer sr;
    private float lifetime;
    private float elapsed;
    private Color startColor;

    // Modo Sprite — usado pelo player
    public void Init(Sprite sprite, bool flipX, Color color, float duration)
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();

        sr.sprite = sprite;
        sr.flipX = flipX;
        sr.color = color;

        startColor = color;
        lifetime = duration;
        elapsed = 0f;

        Destroy(gameObject, duration);
    }

    // Modo Texture — usado pelos inimigos
    // Recebe a textura atual diretamente do EnemyQuadAnimator e converte para Sprite
    public void InitFromTexture(Texture2D tex, bool flipX, Color color, float duration)
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();

        if (tex != null)
        {
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            sr.sprite = sprite;
        }

        sr.flipX = flipX;
        sr.color = color;

        startColor = color;
        lifetime = duration;
        elapsed = 0f;

        Destroy(gameObject, duration);
    }

    void Update()
    {
        if (sr == null) return;

        elapsed += Time.deltaTime;
        float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / lifetime);
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }
}
