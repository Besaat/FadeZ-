using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f;
    private Vector3 direction;
    private bool isReady = false;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        isReady = true;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!isReady) return;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Sphere") return;

        if (other.CompareTag("Enemy"))
        {
            EnemyCapsuleAI enemy = other.GetComponent<EnemyCapsuleAI>();
            if (enemy != null) enemy.Die();
            Destroy(gameObject);
        }
    }
}