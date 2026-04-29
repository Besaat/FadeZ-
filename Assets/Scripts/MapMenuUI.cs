using UnityEngine;
using System.Collections;

public class MapMenuUI : MonoBehaviour
{
    public GameObject mapPanel;
    public MapManager mapManager;
    public ScreenFade screenFade;

    public PlayerMove playerMove;

    [Header("Música de Fundo")]
    public AudioSource bgMusic;
    public float audibleVolume = 0.650f;
    public float musicDelay = 1.5f;

    void Start()
    {
        if (bgMusic != null)
        {
            bgMusic.volume = 0f;

            if (!bgMusic.isPlaying)
                bgMusic.Play();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool isOpen = !mapPanel.activeSelf;

            mapPanel.SetActive(isOpen);

            if (playerMove != null)
                playerMove.enabled = !isOpen;

            if (isOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void OnClickSim()
    {
        // fecha mapa imediatamente
        mapPanel.SetActive(false);

        // mantém mouse livre
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // inicia música com atraso
        if (bgMusic != null)
        {
            StartCoroutine(PlayMusicWithDelay());
        }

        // inicia transição
        StartCoroutine(DoTransition());
    }

    IEnumerator PlayMusicWithDelay()
    {
        yield return new WaitForSeconds(musicDelay);

        bgMusic.Stop();
        bgMusic.time = 0f;
        bgMusic.volume = audibleVolume;
        bgMusic.Play();
    }

    IEnumerator DoTransition()
    {
        yield return StartCoroutine(screenFade.FadeOut());

        mapManager.GenerateNewMap();

        yield return StartCoroutine(screenFade.FadeIn());

        mapPanel.SetActive(false);

        if (playerMove != null)
            playerMove.enabled = true;
    }

    public void OnClickNao()
    {
        mapPanel.SetActive(false);

        if (playerMove != null)
            playerMove.enabled = true;
    }
}