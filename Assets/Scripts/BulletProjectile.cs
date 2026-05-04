using UnityEngine;

// Projétil disparado pelo player.
// Move em linha reta na direção definida pelo PlayerShoot.
// Destrói a si mesmo ao atingir um inimigo ou após o tempo de vida expirar.
public class BulletProjectile : MonoBehaviour
{
    [Header("Configuração")]
    public float speed = 20f;     // Velocidade do projétil
    public float lifetime = 3f;   // Tempo em segundos até se auto-destruir

    private Vector3 direction;    // Direção normalizada do movimento
    private bool isReady = false; // Só se move após SetDirection ser chamado

    // Chamado pelo PlayerShoot logo após instanciar o projétil
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        isReady = true;
    }

    void Start()
    {
        // Garante destruição automática mesmo se não atingir nada
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!isReady) return;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignora colisão com o próprio player e com o orbe (Sphere)
        if (other.CompareTag("Player") || other.gameObject.name == "Sphere") return;

        if (other.CompareTag("Enemy"))
        {
            EnemyCapsuleAI enemy = other.GetComponent<EnemyCapsuleAI>();
            if (enemy != null) enemy.Die();

            Destroy(gameObject);
        }
        else
        {
            // Destrói ao bater em qualquer outro objeto sólido (árvores, pedras etc)
            // Comente a linha abaixo se quiser que o tiro passe por obstáculos
            Destroy(gameObject);
        }
    }
}
