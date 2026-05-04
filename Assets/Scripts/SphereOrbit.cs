using UnityEngine;

// Controla o orbe de luz que orbita ao redor do player seguindo o mouse.
// Modos:
// - Normal: orbe segue o mouse ao redor do player em altura média
// - Atirar (LMB): orbe desce para a posição de mira antes de atirar
// - Ataque de Área (RMB): orbe sobe acima da cabeça do player
public class DroneFollow : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public Camera cam;

    [Header("Órbita")]
    public float orbitRadius = 1.5f;
    public float followSpeed = 8f;

    [Header("Alturas")]
    public float height = 1.5f;         // Altura padrão
    public float shootHeight = 0.5f;    // Altura ao atirar (LMB) — posição de mira
    public float areaHeight = 2.5f;     // Altura ao carregar ataque de área (RMB) — acima da cabeça

    [Header("Correção de ângulo")]
    [Tooltip("Quando o mouse está acima deste % da tela, o orbe vai para o lado oposto")]
    public float screenThreshold = 0.6f;

    // Estado atual do orbe — lido pelo PlayerShoot
    public enum OrbMode { Normal, Shooting, AreaCharge }
    [HideInInspector] public OrbMode currentMode = OrbMode.Normal;

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, player.position);

        if (!groundPlane.Raycast(ray, out float distance)) return;

        Vector3 mouseWorldPos = ray.GetPoint(distance);
        Vector3 dir = (mouseWorldPos - player.position);
        dir.y = 0f;

        if (dir == Vector3.zero) return;
        dir.Normalize();

        float screenY = Input.mousePosition.y / Screen.height;
        if (screenY > screenThreshold)
            dir = -dir;

        Vector3 targetPos = player.position + dir * orbitRadius;

        // Define modo e altura conforme input
        if (Input.GetMouseButton(1))
        {
            // RMB — modo de ataque de área: orbe sobe acima da cabeça
            currentMode = OrbMode.AreaCharge;
            targetPos = player.position; // Centraliza sobre o player
            targetPos.y = player.position.y + areaHeight;
        }
        else if (Input.GetMouseButton(0))
        {
            // LMB — modo de tiro: orbe desce para posição de mira
            currentMode = OrbMode.Shooting;
            targetPos.y = player.position.y + shootHeight;
        }
        else
        {
            currentMode = OrbMode.Normal;
            targetPos.y = player.position.y + height;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}
