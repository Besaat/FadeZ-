using UnityEngine;

// Toca uma animação de ataque em área usando uma spritesheet fatiada.
// Instanciado pelo PlayerShoot no momento do ataque de área.
// A animação toca uma vez e o objeto se destrói ao terminar.
// O sprite é rotacionado para ficar deitado (visão isométrica).
// O movimento do player fica travado durante a animação via isAttacking.
public class AreaAttackEffect : MonoBehaviour
{
    [Header("Spritesheet")]
    public Sprite[] frames;
    public float fps = 12f;

    [Header("Halo de luz")]
    public float lightIntensity = 300f;
    public Color lightColor = new Color(1f, 0.9f, 0.5f, 1f);

    private SpriteRenderer sr;
    private int currentFrame = 0;
    private float timer = 0f;
    private bool finished = false;
    private PlayerMove playerMove;

    public void Init(float areaRadius, PlayerMove pm)
    {
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        float size = areaRadius * 4f;
        transform.localScale = new Vector3(size, size, size);

        // Trava o movimento via booleano — não desativa o componente
        playerMove = pm;
        if (playerMove != null)
            playerMove.isAttacking = true;

        // Cria o halo de luz na posição do efeito
        GameObject lightObj = new GameObject("AreaHalo");
        lightObj.transform.SetParent(transform);
        lightObj.transform.localPosition = Vector3.zero;
        lightObj.transform.localRotation = Quaternion.identity;
        lightObj.transform.localScale = Vector3.one;

        Light halo = lightObj.AddComponent<Light>();
        halo.type = LightType.Point;
        halo.color = lightColor;
        halo.intensity = lightIntensity;
        halo.range = areaRadius * 3f;
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();

        sr.color = new Color(1f, 1f, 1f, 0.4f);

        if (frames != null && frames.Length > 0)
            sr.sprite = frames[0];
    }

    void Update()
    {
        if (finished || frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;

        if (timer >= 1f / fps)
        {
            timer = 0f;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                finished = true;
                if (playerMove != null)
                    playerMove.isAttacking = false;
                Destroy(gameObject);
                return;
            }

            sr.sprite = frames[currentFrame];
        }
    }

    void OnDestroy()
    {
        // Garante que o movimento seja destravado mesmo se destruído externamente
        if (playerMove != null)
            playerMove.isAttacking = false;
    }
}

