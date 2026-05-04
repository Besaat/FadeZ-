using UnityEngine;

// Simula a queda de uma folha de árvore com movimento senoidal lateral.
// Instanciado pelo TreeLeafSpawner em intervalos aleatórios.
// Auto-destrói após alguns segundos para não acumular objetos na cena.
public class LeafFall : MonoBehaviour
{
    [Header("Movimento")]
    public float fallSpeed = 1.5f;   // Velocidade de descida
    public float swayAmount = 0.5f;  // Amplitude do balanço lateral
    public float swaySpeed = 2f;     // Velocidade do balanço lateral
    public float rotateSpeed = 90f;  // Velocidade de rotação (graus/segundo)

    private float timeOffset; // Offset aleatório para evitar que todas as folhas sincronizem

    void Start()
    {
        timeOffset = Random.Range(0f, 100f);
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // Queda vertical
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // Balanço lateral senoidal
        float sway = Mathf.Sin(Time.time * swaySpeed + timeOffset) * swayAmount;
        transform.position += new Vector3(sway * Time.deltaTime, 0f, 0f);

        // Rotação contínua para efeito visual
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }
}
