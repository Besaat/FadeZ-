using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Gerencia a HUD do jogo, atualizando todos os elementos visuais a cada frame.
// Requer referências aos scripts LightAmmo e MapManager.
public class HUDManager : MonoBehaviour
{
    [Header("Barra de Luz (Amarelo)")]
    public Image lightBar;              // Image com Image Type = Filled

    [Header("Recursos (Azul)")]
    public TextMeshProUGUI resourcesText;

    [Header("Fase (Vermelho)")]
    public TextMeshProUGUI floorText;

    [Header("Inimigos Mortos (Roxo)")]
    public TextMeshProUGUI killsText;

    [Header("Referências")]
    public LightAmmo lightAmmo;
    public MapManager mapManager;

    void Update()
    {
        if (lightAmmo != null && lightBar != null)
            lightBar.fillAmount = lightAmmo.currentIntensity / lightAmmo.maxIntensity;

        if (mapManager != null)
        {
            if (floorText != null)
                floorText.text = "Fase: " + mapManager.currentFloor;

            if (killsText != null)
                killsText.text = "Abates: " + mapManager.totalKills;
        }

        if (resourcesText != null)
            resourcesText.text = "Recursos: " + LightAmmo.lightBallsCollected;
    }
}
