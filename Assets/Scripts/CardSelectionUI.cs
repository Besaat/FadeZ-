using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Exibe 3 cartas de upgrade quando todos os inimigos da fase são eliminados.
// As cartas são em branco por enquanto — prontas para receber nome e efeito futuramente.
// Chamado pelo MapManager quando allEnemiesDead se torna true.
public class CardSelectionUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject cardPanel;        // Painel que contém as 3 cartas
    public Button[] cardButtons;        // Array com os 3 botões de carta
    public PlayerMove playerMove;       // Referência para travar o player durante a seleção

    // Chamado pelo MapManager ao detectar que todos os inimigos morreram
    public void ShowCards()
    {
        if (cardPanel == null) return;

        // Trava o movimento do player
        if (playerMove != null)
            playerMove.enabled = false;

        cardPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Chamado por cada botão de carta ao ser clicado
    public void OnCardSelected(int cardIndex)
    {
        // TODO: aplicar o efeito do upgrade correspondente ao cardIndex
        Debug.Log("Carta selecionada: " + cardIndex);

        cardPanel.SetActive(false);

        // Destrava o movimento do player
        if (playerMove != null)
            playerMove.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
