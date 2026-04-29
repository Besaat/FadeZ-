using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera cam;

    void LateUpdate()
    {
        transform.forward = cam.transform.forward;
    }
}
