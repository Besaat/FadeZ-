using UnityEngine;
using System.Collections;

// Controla o menu principal do jogo.
// O player começa desativado e é ativado ao clicar em "Jogar".
// TODO: adicionar opções de configuração e tela de créditos.
public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject menuPanel; // Painel visual do menu principal

    [Header("Referências")]
    public ScreenFade screenFade;
    public GameObject player; // Objeto do player (desativado no início)

    void Start()
    {
        // Desativa o player para bloquear qualquer ação antes do menu
        if (player != null)
            player.SetActive(false);

        // Libera o cursor no menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Chamado pelo botão "Jogar" no menu
    public void OnClickPlay()
    {
        StartCoroutine(StartGame());
    }

    // Sequência: fade out → esconde menu → ativa player → fade in
    IEnumerator StartGame()
    {
        yield return StartCoroutine(screenFade.FadeOut());

        menuPanel.SetActive(false);

        if (player != null)
            player.SetActive(true);

        yield return StartCoroutine(screenFade.FadeIn());
    }
}
