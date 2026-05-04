using UnityEngine;

// Controla o clipping por altura em materiais que suportam esse shader.
// Usado em árvores para cortar visualmente a parte superior quando necessário.
// Os parâmetros _CutHeight e _FadeRange devem existir no shader do material.
public class HeightClipController : MonoBehaviour
{
    [Header("Configuração")]
    public float cutHeight = 0f;   // Altura em que o corte começa
    public float fadeRange = 0.5f; // Suavidade da borda do corte

    private Renderer[] rends;

    void Start()
    {
        rends = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        // Atualiza os parâmetros do shader em todos os renderers filhos
        foreach (Renderer r in rends)
        {
            foreach (Material mat in r.materials)
            {
                mat.SetFloat("_CutHeight", cutHeight);
                mat.SetFloat("_FadeRange", fadeRange);
            }
        }
    }
}
