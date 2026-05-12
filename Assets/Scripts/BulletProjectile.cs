using UnityEngine;

// Projétil disparado pelo player.
public class BulletProjectile : MonoBehaviour
{
    [Header("Configuração")]
    public float speed = 20f;
    public float lifetime = 3f;
    public bool pierceEnabled = false; // Upgrade 5: atravessa inimigos

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

            // Se pierce não está ativo, destrói o projétil ao atingir inimigo
            if (!pierceEnabled)
                Destroy(gameObject);
        }
        else
        {
            // Destrói ao bater em obstáculos independente do pierce
            Destroy(gameObject);
        }
    }
}
