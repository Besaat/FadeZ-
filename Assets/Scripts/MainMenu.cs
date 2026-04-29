using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public ScreenFade screenFade;

    public GameObject player; // ou o que você quer ativar

    void Start()
    {
        // trava o player no início
        if (player != null)
            player.SetActive(false);

        // mouse livre
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnClickPlay()
    {
        Debug.Log("PLAY FUNCIONOU");
        Debug.Log("CLIQUEI NO PLAY");
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        // 🔥 Fade OUT
        yield return StartCoroutine(screenFade.FadeOut());

        // ❌ esconde menu
        menuPanel.SetActive(false);

        // ✅ ativa jogador/jogo
        if (player != null)
            player.SetActive(true);

        // 🔥 Fade IN
        yield return StartCoroutine(screenFade.FadeIn());

        
    }
}