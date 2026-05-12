using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Exibe 3 cartas de upgrade quando todos os inimigos da fase são eliminados.
// Sorteia upgrades aleatórios do UpgradeManager e exibe o nome em cada carta.
public class CardSelectionUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject cardPanel;
    public Button[] cardButtons;            // Array com os 3 botões de carta
    public TextMeshProUGUI[] cardTexts;     // Array com os 3 textos de nome do upgrade

    [Header("Referências")]
    public PlayerMove playerMove;

    private int[] currentOptions = new int[3]; // Índices dos upgrades sorteados

    void Update()
    {
        // Debug: pressione Y para abrir a seleção de cartas a qualquer momento
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Y))
            ShowCards();
#endif
    }

    public void ShowCards()
    {
        if (cardPanel == null) return;

        if (playerMove != null)
            playerMove.enabled = false;

        // Sorteia 3 upgrades
        if (UpgradeManager.instance != null)
        {
            currentOptions = UpgradeManager.instance.GetRandomUpgradeOptions();

            // Atualiza o texto de cada carta
            for (int i = 0; i < cardButtons.Length; i++)
            {
                if (i < currentOptions.Length)
                {
                    cardButtons[i].gameObject.SetActive(true);
                    if (cardTexts != null && i < cardTexts.Length && cardTexts[i] != null)
                        cardTexts[i].text = UpgradeManager.upgradeNames[currentOptions[i]];
                }
                else
                {
                    cardButtons[i].gameObject.SetActive(false);
                }
            }
        }

        cardPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Chamado por cada botão de carta — o índice (0, 1 ou 2) indica qual carta foi clicada
    public void OnCardSelected(int cardIndex)
    {
        if (UpgradeManager.instance != null && cardIndex < currentOptions.Length)
            UpgradeManager.instance.ApplyUpgrade(currentOptions[cardIndex]);

        cardPanel.SetActive(false);

        if (playerMove != null)
            playerMove.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
