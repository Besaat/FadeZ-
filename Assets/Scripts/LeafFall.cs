using UnityEngine;

public class LeafFall : MonoBehaviour
{
    public float fallSpeed = 1.5f;
    public float swayAmount = 0.5f;
    public float swaySpeed = 2f;
    public float rotateSpeed = 90f;

    float offset;

    void Start()
    {
        offset = Random.Range(0f, 100f);
        Destroy(gameObject, 5f); // destrói depois de um tempo
    }

    void Update()
    {
        // movimento de queda
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // leve balanço lateral
        float sway = Mathf.Sin(Time.time * swaySpeed + offset) * swayAmount;
        transform.position += new Vector3(sway * Time.deltaTime, 0, 0);

        // rotação
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }
}
