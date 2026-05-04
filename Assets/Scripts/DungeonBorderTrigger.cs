using UnityEngine;

// Trigger colocado nas bordas do mapa de expedição.
// Quando o player atravessa a borda, gera um novo mapa (próxima sala).
// IMPORTANTE: Este objeto precisa ter um Collider com Is Trigger = true.
// TODO: integrar fade de tela aqui para transição suave entre salas.
public class DungeonBorderTrigger : MonoBehaviour
{
    // Cooldown para evitar que múltiplos triggers sejam ativados ao mesmo tempo
    // (o player pode tocar 2 bordas simultaneamente no canto do mapa)
    private bool onCooldown = false;

    void OnTriggerEnter(Collider other)
    {
        if (onCooldown) return;

        if (other.CompareTag("Player"))
        {
            onCooldown = true;

            MapManager mapManager = FindFirstObjectByType<MapManager>();
            if (mapManager != null)
                mapManager.GenerateNewMap();
            else
                Debug.LogWarning("DungeonBorderTrigger: MapManager não encontrado na cena.");
        }
    }
}
