using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Controla o efeito de fade (escurecer/clarear) da tela usando uma Image de UI.
// A Image deve cobrir toda a tela e estar acima de todos os outros elementos de UI.
// Use StartCoroutine(FadeOut()) e StartCoroutine(FadeIn()) para acionar os efeitos.
public class ScreenFade : MonoBehaviour
{
    [Header("Referências")]
    public Image fadeImage; // Image preta que cobre a tela

    [Header("Configuração")]
    public float fadeDuration = 1f; // Duração do fade em segundos

    // Escurece a tela (transparente → preto)
    public IEnumerator FadeOut()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        // Garante que termine completamente preto
        fadeImage.color = new Color(0f, 0f, 0f, 1f);
    }

    // Clareia a tela (preto → transparente)
    public IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        // Garante que termine completamente transparente
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
    }
}
