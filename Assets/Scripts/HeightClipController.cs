using UnityEngine;

public class HeightClipController : MonoBehaviour
{
    public float cutHeight = 0f;
    public float fadeRange = 0.5f;

    private Renderer[] rends;

    void Start()
    {
        rends = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
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