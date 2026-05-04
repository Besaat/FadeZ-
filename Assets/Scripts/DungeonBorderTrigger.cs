using UnityEngine;
using System.Collections;

// Trigger nas bordas do mapa de expedição.
// Quando o player toca a borda: fade out → gera novo mapa → player spawna no lado oposto → fade in.
// IMPORTANTE: este objeto precisa ter um Collider com Is Trigger = true.
public class DungeonBorderTrigger : MonoBehaviour
{
    [Header("Referências")]
    public ScreenFade screenFade;
    public PlayerMove playerMove;

    [Header("Direção desta borda")]
    [Tooltip("Vetor normalizado que aponta para dentro do mapa a partir desta borda. Ex: borda Norte = (0,0,-1)")]
    public Vector3 entryDirection = Vector3.forward; // Configurar no Inspector para cada borda

    private bool onCooldown = false;

    void OnTriggerEnter(Collider other)
    {
        if (onCooldown) return;
        if (!other.CompareTag("Player")) return;

        onCooldown = true;
        StartCoroutine(DoTransition());
    }

    IEnumerator DoTransition()
    {
        if (playerMove != null)
            playerMove.enabled = false;

        if (screenFade != null)
            yield return StartCoroutine(screenFade.FadeOut());
        else
            yield return new WaitForSeconds(0.5f);

        // Informa ao MapManager de qual direção o player veio
        // para que ele spawne no lado oposto
        MapManager mapManager = FindFirstObjectByType<MapManager>();
        if (mapManager != null)
        {
            mapManager.lastEntryDirection = entryDirection.normalized;
            mapManager.GenerateNewMap();
        }
        else
        {
            Debug.LogWarning("DungeonBorderTrigger: MapManager não encontrado.");
        }

        yield return new WaitForSeconds(0.2f);

        if (screenFade != null)
            yield return StartCoroutine(screenFade.FadeIn());

        if (playerMove != null)
            playerMove.enabled = true;

        yield return new WaitForSeconds(1f);
        onCooldown = false;
    }
}
