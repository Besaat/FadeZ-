using UnityEngine;

// Faz a câmera seguir o player com suavização.
// NOVIDADE: suporta limites de mapa para impedir que a câmera
// ultrapasse as bordas da fase durante expedições.
public class CameraFollow : MonoBehaviour
{
    [Header("Referências")]
    public Transform target;

    [Header("Configuração")]
    public Vector3 offset = new Vector3(0f, 8f, -6f);
    public float smoothSpeed = 8f;

    // Limites do mapa (definidos pelo MapManager)
    private bool hasBounds = false;
    private Vector3 mapCenter;
    private float mapSize;

    private Quaternion fixedRotation;

    void Start()
    {
        if (target == null) return;
        transform.position = target.position + offset;
        fixedRotation = Quaternion.LookRotation(-offset.normalized);
        transform.rotation = fixedRotation;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        // Limita a posição da câmera às bordas do mapa se os limites estiverem definidos
        if (hasBounds)
        {
            // Margem para não mostrar além da borda (compensa o offset da câmera)
            float margin = 5f;
            float minX = mapCenter.x - mapSize + margin;
            float maxX = mapCenter.x + mapSize - margin;
            float minZ = mapCenter.z - mapSize + margin;
            float maxZ = mapCenter.z + mapSize - margin;

            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.z = Mathf.Clamp(desiredPosition.z, minZ, maxZ);
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.rotation = fixedRotation;
    }

    // Chamado pelo MapManager ao gerar um novo mapa
    public void SetMapBounds(Vector3 center, float size)
    {
        mapCenter = center;
        mapSize = size;
        hasBounds = true;
    }

    // Chamado pelo MapManager ao voltar para a base (sem limites)
    public void ClearMapBounds()
    {
        hasBounds = false;
    }
}
