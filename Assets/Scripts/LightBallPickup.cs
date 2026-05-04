using UnityEngine;

// Orbe de luz dropado por inimigos ao morrer.
// Flutua e gira para ser visível no chão.
// Ao contato com o player, restaura luz e se auto-destrói.
public class LightBallPickup : MonoBehaviour
{
    [Header("Animação")]
    public float bobSpeed = 2f;       // Velocidade da flutuação vertical
    public float bobHeight = 0.3f;    // Amplitude da flutuação
    public float rotateSpeed = 90f;   // Velocidade de rotação (graus/segundo)

    [Header("Áudio")]
    public AudioSource pickupSound;

    private Vector3 startPos;
    private LightAmmo playerLightAmmo;

    void Start()
    {
        startPos = transform.position;
        playerLightAmmo = FindFirstObjectByType<LightAmmo>();

        if (pickupSound == null)
            pickupSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Flutuação senoidal no eixo Y
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotação contínua no eixo Y
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Restaura luz do player
        if (playerLightAmmo != null)
            playerLightAmmo.Restore(playerLightAmmo.pickupRestoreAmount);

        // Toca o som na posição do pickup (independente do objeto, que será destruído)
        if (pickupSound != null && pickupSound.clip != null)
            AudioSource.PlayClipAtPoint(pickupSound.clip, transform.position);

        Destroy(gameObject);
    }
}
