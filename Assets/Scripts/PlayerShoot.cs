using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Referências")]
    public Transform shootPoint;
    public GameObject bulletPrefab;
    public Camera cam;

    [Header("Áudio")]
    public AudioSource audioSource;
    public AudioClip shootSound;

    [Header("Config")]
    public float fireRate = 0.3f;
    private float fireTimer = 0f;

    private DroneFollow droneFollow;
    private LightAmmo lightAmmo;

    void Start()
    {
        droneFollow = shootPoint.GetComponent<DroneFollow>();
        lightAmmo = shootPoint.GetComponent<LightAmmo>();
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        if (droneFollow != null)
            droneFollow.isShooting = Input.GetMouseButton(0);

        if (Input.GetMouseButton(0) && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }

    void Shoot()
    {
        // 🔊 TOCA O SOM
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);
            Vector3 direction = (targetPoint - shootPoint.position);
            direction.y = 0;
            direction.Normalize();

            if (direction != Vector3.zero)
                shootPoint.rotation = Quaternion.LookRotation(direction);

            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);

            BulletProjectile bp = bullet.GetComponent<BulletProjectile>();
            bp.SetDirection(direction);

            if (lightAmmo != null) lightAmmo.Drain();
        }
    }
}