using UnityEngine;

public class LightBallPickup : MonoBehaviour
{
    public float bobSpeed = 2f;
    public float bobHeight = 0.3f;
    public float rotateSpeed = 90f;
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
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        if (playerLightAmmo != null)
            playerLightAmmo.Restore(playerLightAmmo.pickupRestoreAmount);

        // Play sound at position independently of the object
        if (pickupSound != null && pickupSound.clip != null)
            AudioSource.PlayClipAtPoint(pickupSound.clip, transform.position);

        Destroy(gameObject);
    }
}
}