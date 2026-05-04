using UnityEngine;

// Faz a câmera seguir o player com suavização.
// CORREÇÃO DO JITTER: A câmera não usa mais LookAt dinâmico — a rotação é fixa,
// definida pelo offset. Isso elimina o tremido causado pelo LookAt reagindo
// ao movimento de pixel-a-pixel do player.
public class CameraFollow : MonoBehaviour
{
    [Header("Referências")]
    public Transform target; // Transform do player

    [Header("Configuração")]
    public Vector3 offset = new Vector3(0f, 8f, -6f); // Posição relativa ao player
    public float smoothSpeed = 8f; // Velocidade de suavização (maior = mais rápido)

    private Quaternion fixedRotation; // Rotação da câmera calculada uma vez no início

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: nenhum target definido.");
            return;
        }

        // Posiciona a câmera já no offset correto para evitar interpolação inicial brusca
        transform.position = target.position + offset;

        // Calcula a rotação uma única vez com base no offset
        // Isso garante que a câmera sempre aponte para onde o offset "olha"
        fixedRotation = Quaternion.LookRotation(-offset.normalized);
        transform.rotation = fixedRotation;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Interpola suavemente até a posição desejada
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Rotação permanece fixa — sem LookAt dinâmico para evitar jitter
        transform.rotation = fixedRotation;
    }
}
