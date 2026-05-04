using UnityEngine;

// Controla o orbe de luz que orbita ao redor do player seguindo o mouse.
// O orbe ajusta sua altura com base no estado do player (normal, atirando, mirando).
// Este script está no objeto do orbe (ShootPoint), não no player.
public class DroneFollow : MonoBehaviour
{
    [Header("Referências")]
    public Transform player; // Transform do player
    public Camera cam;       // Câmera principal

    [Header("Órbita")]
    public float orbitRadius = 1.5f;      // Distância do orbe ao player
    public float followSpeed = 8f;         // Velocidade de interpolação do orbe

    [Header("Alturas")]
    public float height = 1.5f;           // Altura padrão (sem ação)
    public float shootHeight = 0.5f;      // Altura ao atirar (botão esquerdo)
    public float aimHeight = 0.2f;        // Altura ao mirar (botão direito)

    [Header("Correção de ângulo")]
    [Tooltip("Quando o mouse está acima deste % da tela, o orbe vai para o lado oposto")]
    public float screenThreshold = 0.6f;

    // Definido externamente pelo PlayerShoot para indicar que está atirando
    [HideInInspector] public bool isShooting = false;

    void Update()
    {
        // Projeta o mouse no plano do chão na altura do player
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, player.position);

        if (!groundPlane.Raycast(ray, out float distance)) return;

        Vector3 mouseWorldPos = ray.GetPoint(distance);
        Vector3 dir = (mouseWorldPos - player.position);
        dir.y = 0f;

        if (dir == Vector3.zero) return;
        dir.Normalize();

        // Inverte a direção quando o mouse está muito alto na tela
        // para evitar que o orbe fique entre a câmera e o player
        float screenY = Input.mousePosition.y / Screen.height;
        if (screenY > screenThreshold)
            dir = -dir;

        // Calcula posição alvo do orbe
        Vector3 targetPos = player.position + dir * orbitRadius;

        // Determina altura com base na ação atual
        if (Input.GetMouseButton(1))
            targetPos.y = player.position.y + aimHeight;
        else if (isShooting)
            targetPos.y = player.position.y + shootHeight;
        else
            targetPos.y = player.position.y + height;

        // Move suavemente até a posição alvo
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}
