using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 3f;

    public QuadAnimator anim;

    private Rigidbody rb;
    private Vector3 move;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        move = new Vector3(h, 0, v);

        bool isMoving = move.magnitude > 0.1f;

        // Animação
        if (!isMoving)
        {
            anim.PlayIdle();
        }
        else
        {
            anim.PlayWalk();
        }

        // FLIP
        if (h > 0.1f)
        {
            transform.localScale = new Vector3(3, 3, 3);
        }
        else if (h < -0.1f)
        {
            transform.localScale = new Vector3(-3, 3, 3);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + move.normalized * speed * Time.fixedDeltaTime);
    }
}