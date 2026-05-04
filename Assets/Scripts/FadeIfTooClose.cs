using UnityEngine;

// Deixa o objeto semitransparente quando a câmera está muito próxima.
// Usado em árvores e objetos grandes para evitar que bloqueiem a visão do player.
// REQUISITO: o material do objeto precisa ter suporte a transparência (modo Transparent ou Alpha).
public class FadeIfTooClose : MonoBehaviour
{
    [Header("Distâncias")]
    public float fadeStartDistance = 4f; // Distância em que começa a ficar transparente
    public float fadeEndDistance = 1.5f; // Distância em que fica totalmente invisível

    private Transform cam;
    private Material mat;

    void Start()
    {
        cam = Camera.main.transform;
        Renderer rend = GetComponentInChildren<Renderer>();

        if (rend != null)
            mat = rend.material; // Cria uma instância do material (não afeta outros objetos)
        else
            Debug.LogWarning("FadeIfTooClose: Renderer não encontrado em " + gameObject.name);
    }

    void Update()
    {
        if (cam == null || mat == null) return;

        float dist = Vector3.Distance(transform.position, cam.position);

        // InverseLerp retorna 0 quando dist = fadeEndDistance e 1 quando dist = fadeStartDistance
        float alpha = Mathf.InverseLerp(fadeEndDistance, fadeStartDistance, dist);

        Color color = mat.color;
        color.a = alpha;
        mat.color = color;
    }
}
