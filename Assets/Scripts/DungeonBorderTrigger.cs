using UnityEngine;

public class DungeonBorderTrigger : MonoBehaviour
{
    private bool onCooldown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (onCooldown) return;
        if (other.CompareTag("Player"))
        {
            onCooldown = true;
            FindFirstObjectByType<MapManager>().GenerateNewMap();
        }
    }
}