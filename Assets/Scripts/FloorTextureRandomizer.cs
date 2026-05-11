using UnityEngine;

// Troca o material do chão da dungeon aleatoriamente a cada nova fase.
// Chamado pelo MapManager em GenerateNewMap().
// Adicione este script ao objeto do chão da dungeon e atribua os materiais no Inspector.
public class FloorTextureRandomizer : MonoBehaviour
{
    [Header("Materiais")]
    public Material[] floorMaterials;

    private Renderer rend;
    private int lastIndex = -1;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    // Chamado pelo MapManager a cada nova fase
    public void RandomizeTexture()
    {
        if (floorMaterials == null || floorMaterials.Length == 0 || rend == null) return;

        // Evita repetir o mesmo material duas vezes seguidas
        int newIndex;
        do
        {
            newIndex = Random.Range(0, floorMaterials.Length);
        } while (floorMaterials.Length > 1 && newIndex == lastIndex);

        lastIndex = newIndex;
        rend.material = floorMaterials[newIndex];
    }
}
