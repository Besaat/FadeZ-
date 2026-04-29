using UnityEngine;

public class FadeIfTooClose : MonoBehaviour
{
    public float fadeStartDistance = 4f; // começa a sumir
    public float fadeEndDistance = 1.5f; // totalmente invisível

    private Transform cam;
    private Renderer rend;
    private Material mat;

    void Start()
    {
        cam = Camera.main.transform;
        rend = GetComponentInChildren<Renderer>();

        if (rend != null)
        {
            mat = rend.material;
        }
    }

    void Update()
    {
        if (cam == null || mat == null) return;

        float dist = Vector3.Distance(transform.position, cam.position);

        float alpha = Mathf.InverseLerp(fadeEndDistance, fadeStartDistance, dist);

        Color color = mat.color;
        color.a = alpha;
        mat.color = color;
    }
}