using UnityEngine;
using System.Collections;

// Controla o painel de mapa/expedição aberto com Tab.
// Permite ao jogador iniciar uma expedição ou cancelar.
// Gerencia fade de tela e música de fundo durante a transição.
public class MapMenuUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject mapPanel; // Painel do menu de expedição

    [Header("Referências")]
    public MapManager mapManager;
    public ScreenFade screenFade;
    public PlayerMove playerMove;

    [Header("Música de Fundo")]
    public AudioSource bgMusic; // Volume controlado diretamente no Audio Source (Inspector)

    void Start()
    {
        // Música toca com o volume configurado no Inspector do Audio Source
        if (bgMusic != null && !bgMusic.isPlaying)
            bgMusic.Play();
    }

    void Update()
    {
        // Tab abre/fecha o painel e pausa/retoma o movimento do player
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool isOpening = !mapPanel.activeSelf;
            mapPanel.SetActive(isOpening);

            if (playerMove != null)
                playerMove.enabled = !isOpening;

            // Controle de cursor: livre no menu, oculto durante gameplay
            Cursor.lockState = isOpening ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isOpening;
        }
    }

    // Botão "Sim" — inicia a expedição com fade e geração de mapa
    public void OnClickSim()
    {
        mapPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(DoTransition());
    }

    // Botão "Não" — fecha o painel e devolve o controle ao player
    public void OnClickNao()
    {
        mapPanel.SetActive(false);

        if (playerMove != null)
            playerMove.enabled = true;
    }

    // Sequência de transição: fade out → gera mapa → fade in → libera controle
    IEnumerator DoTransition()
    {
        yield return StartCoroutine(screenFade.FadeOut());
        mapManager.GenerateNewMap();
        yield return StartCoroutine(screenFade.FadeIn());
        mapPanel.SetActive(false);

        if (playerMove != null)
            playerMove.enabled = true;
    }
}
