using UnityEngine;
public class DroneFollow : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public Camera cam;
    [Header("Config")]
    public float orbitRadius = 1.5f;
    public float height = 1.5f;
    public float shootHeight = 0.5f;
    public float aimHeight = 0.2f; // height when aiming with right click
    public float followSpeed = 8f;
    public float screenThreshold = 0.6f;

    [HideInInspector] public bool isShooting = false;

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, player.position);
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);
            Vector3 dir = (targetPoint - player.position);
            dir.y = 0;
            if (dir == Vector3.zero) return;
            dir.Normalize();

            float screenY = Input.mousePosition.y / Screen.height;
            if (screenY > screenThreshold)
                dir = -dir;

            Vector3 targetPos = player.position + dir * orbitRadius;

            // Priority: aiming > shooting > normal
            if (Input.GetMouseButton(1))
                targetPos.y = player.position.y + aimHeight;
            else if (isShooting)
                targetPos.y = player.position.y + shootHeight;
            else
                targetPos.y = player.position.y + height;

            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        }
    }
}