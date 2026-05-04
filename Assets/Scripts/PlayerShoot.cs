using UnityEngine;

// Gerencia o disparo do player.
// O tiro parte do ponto onde o orbe está (shootPoint) em direção ao cursor do mouse.
// Cada tiro consome luz via LightAmmo.
public class PlayerShoot : MonoBehaviour
{
    [Header("Referências")]
    public Transform shootPoint;     // Transform do orbe (origem do tiro)
    public GameObject bulletPrefab;  // Prefab do projétil
    public Camera cam;               // Câmera principal

    [Header("Áudio")]
    public AudioSource audioSource;
    public AudioClip shootSound;

    [Header("Configuração")]
    public float fireRate = 0.3f;    // Intervalo mínimo entre tiros (segundos)

    private float fireTimer = 0f;    // Contador regressivo até o próximo tiro permitido
    private DroneFollow droneFollow; // Referência ao orbe para sinalizar estado de disparo
    private LightAmmo lightAmmo;     // Referência ao sistema de munição/luz

    void Start()
    {
        // Busca os componentes no próprio objeto do orbe
        droneFollow = shootPoint.GetComponent<DroneFollow>();
        lightAmmo = shootPoint.GetComponent<LightAmmo>();
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        // Informa ao orbe se está atirando (para ajustar a altura)
        if (droneFollow != null)
            droneFollow.isShooting = Input.GetMouseButton(0);

        // Dispara se botão esquerdo pressionado e cooldown zerado
        if (Input.GetMouseButton(0) && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }

    void Shoot()
    {
        // Toca o som de disparo sem interromper sons anteriores
        if (audioSource != null && shootSound != null)
            audioSource.PlayOneShot(shootSound);

        // Projeta um raio da câmera até o plano do chão na altura do player
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

        if (!groundPlane.Raycast(ray, out float distance)) return;

        Vector3 targetPoint = ray.GetPoint(distance);
        Vector3 direction = targetPoint - shootPoint.position;
        direction.y = 0f;
        direction.Normalize();

        if (direction == Vector3.zero) return;

        // Rotaciona o orbe para apontar na direção do tiro
        shootPoint.rotation = Quaternion.LookRotation(direction);

        // Instancia o projétil na posição do orbe
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        BulletProjectile bp = bullet.GetComponent<BulletProjectile>();
        if (bp != null) bp.SetDirection(direction);

        // Consome luz como munição
        if (lightAmmo != null) lightAmmo.Drain();
    }
}
