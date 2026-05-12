using UnityEngine;
using System.Collections;

// Gerencia os dois modos de ataque do player:
// - Tiro normal (LMB): atira projéteis na direção do mouse
// - Ataque de área (RMB segurado + LMB): pulsa luz e causa dano em área ao redor do player
public class PlayerShoot : MonoBehaviour
{
    [Header("Referências")]
    public Transform shootPoint;
    public GameObject bulletPrefab;
    public Camera cam;
    public PlayerMove playerMove;

    [Header("Áudio")]
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip areaAttackSound;

    [Header("Tiro Normal")]
    public float fireRate = 0.3f;
    public float normalDrainAmount = 15f;

    [Header("Ataque de Área")]
    public float areaRadius = 5f;
    public float areaDamage = 300f;
    public float areaDrainAmount = 80f;
    public float areaCooldown = 1.5f;
    public float areaFlashDuration = 0.2f;
    public GameObject areaAttackEffectPrefab; // Prefab do efeito visual do ataque de área

    private float fireTimer = 0f;
    private float areaTimer = 0f;
    private DroneFollow droneFollow;
    private LightAmmo lightAmmo;
    private Light orbLight;

    void Start()
    {
        droneFollow = shootPoint.GetComponent<DroneFollow>();

        lightAmmo = shootPoint.GetComponent<LightAmmo>();
        if (lightAmmo == null)
            lightAmmo = FindFirstObjectByType<LightAmmo>();

        orbLight = shootPoint.GetComponentInChildren<Light>();
        if (orbLight == null)
            orbLight = FindFirstObjectByType<Light>();
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;
        areaTimer -= Time.deltaTime;

        // Tiro normal — LMB sem RMB
        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireRate;
        }

        // Ataque de área — RMB segurado + LMB pressionado
        if (Input.GetMouseButton(1) && Input.GetMouseButtonDown(0) && areaTimer <= 0f)
        {
            StartCoroutine(AreaAttack());
            areaTimer = areaCooldown;
        }
    }

    void Shoot()
    {
        if (audioSource != null && shootSound != null)
            audioSource.PlayOneShot(shootSound);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

        if (!groundPlane.Raycast(ray, out float distance)) return;

        Vector3 targetPoint = ray.GetPoint(distance);
        Vector3 direction = targetPoint - shootPoint.position;
        direction.y = 0f;
        direction.Normalize();

        if (direction == Vector3.zero) return;

        shootPoint.rotation = Quaternion.LookRotation(direction);

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        BulletProjectile bp = bullet.GetComponent<BulletProjectile>();
        if (bp != null) bp.SetDirection(direction);

        if (lightAmmo != null) lightAmmo.Drain(normalDrainAmount);
    }

    IEnumerator AreaAttack()
    {
        if (audioSource != null && areaAttackSound != null)
            audioSource.PlayOneShot(areaAttackSound);

        if (lightAmmo != null) lightAmmo.Drain(areaDrainAmount);

        // Instancia o efeito visual na posição do player, na altura da cintura
        if (areaAttackEffectPrefab != null)
        {
            Vector3 effectPos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
            GameObject effect = Instantiate(areaAttackEffectPrefab, effectPos, Quaternion.identity);
            AreaAttackEffect aae = effect.GetComponent<AreaAttackEffect>();
            if (aae != null) aae.Init(areaRadius, playerMove);
        }

        // Flash visual na luz
        float originalRange = orbLight != null ? orbLight.range : 0f;
        float originalIntensity = orbLight != null ? orbLight.intensity : 0f;

        if (orbLight != null)
        {
            orbLight.range = areaRadius * 2.5f;
            orbLight.intensity = originalIntensity * 2f;
        }

        // Detecta e mata inimigos em área
        Collider[] hits = Physics.OverlapSphere(transform.position, areaRadius);
        foreach (Collider hit in hits)
        {
            EnemyCapsuleAI enemy = hit.GetComponent<EnemyCapsuleAI>();
            if (enemy == null) enemy = hit.GetComponentInParent<EnemyCapsuleAI>();
            if (enemy != null) enemy.Die();
        }

        yield return new WaitForSeconds(areaFlashDuration);

        if (orbLight != null)
        {
            orbLight.range = originalRange;
            orbLight.intensity = originalIntensity;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, areaRadius);
        Gizmos.color = new Color(1f, 1f, 0f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, areaRadius);
    }
}
