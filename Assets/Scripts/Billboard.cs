using UnityEngine;

// Faz o objeto sempre encarar a câmera (efeito billboard/sprite 2D em mundo 3D).
// Usado em árvores, inimigos e qualquer sprite quad para simular profundidade.
public class Billboard : MonoBehaviour
{
    public Camera cam;

    void LateUpdate()
    {
        // Alinha o forward do objeto com o forward da câmera
        transform.forward = cam.transform.forward;
    }
}
