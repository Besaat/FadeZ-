using UnityEngine;

// Gerencia a luz do player como recurso único:
// munição (tiros drenam luz), vida (sem luz = morte) e recurso coletável.
// Controla tanto a Intensity quanto o Range da luz para tornar
// a redução muito mais perceptível visualmente.
public class LightAmmo : MonoBehaviour
{
    [Header("Referências")]
    public Light sphereLight;
    public QuadAnimator playerAnim; // Para tocar a animação de hit ao tomar dano

    [Header("Intensidade")]
    public float maxIntensity = 750f;
    public float drainPerShot = 15f;         // Drain padrão (tiro normal)
    public float rechargeRate = 20f;         // Recarga por segundo
    public float deathThreshold = 2f;

    [Header("Range da luz")]
    public float maxRange = 10f;
    public float minRange = 1.5f;

    [Header("Pickup")]
    public float pickupRestoreAmount = 100f;

    public static int lightBallsCollected = 0;
    [HideInInspector] public float currentIntensity;

    private bool isDead = false;

    void Start()
    {
        currentIntensity = maxIntensity;
        lightBallsCollected = 0;
        if (sphereLight == null)
            sphereLight = GetComponentInChildren<Light>();
    }

    void Update()
    {
        if (isDead) return;

        currentIntensity += rechargeRate * Time.deltaTime;
        currentIntensity = Mathf.Clamp(currentIntensity, 0f, maxIntensity);

        UpdateLight();

        if (currentIntensity <= deathThreshold)
        {
            isDead = true;
            PlayerDeath.Die();
        }
    }

    void UpdateLight()
    {
        if (sphereLight == null) return;
        float proportion = currentIntensity / maxIntensity;
        sphereLight.intensity = currentIntensity;
        sphereLight.range = Mathf.Lerp(minRange, maxRange, proportion);
    }

    // Drain com quantidade padrão (tiro normal)
    public void Drain()
    {
        currentIntensity -= drainPerShot;
    }

    // Drain com quantidade específica (ataque de área, dano de inimigo)
    // Toca a animação de hit se o playerAnim estiver atribuído
    public void Drain(float amount)
    {
        currentIntensity -= amount;

        // Toca animação de hit ao tomar dano (não ao atirar)
        if (playerAnim != null)
            StartCoroutine(PlayHitAnimation());
    }

    // Toca hit por um curto período e volta para idle/walk
    System.Collections.IEnumerator PlayHitAnimation()
    {
        playerAnim.PlayHit();
        yield return new WaitForSeconds(0.4f); // Duração da animação de hit
        // Não precisa forçar idle — o PlayerMove vai trocar automaticamente no próximo frame
    }

    public void Restore(float amount)
    {
        currentIntensity = Mathf.Clamp(currentIntensity + amount, 0f, maxIntensity);
        lightBallsCollected++;
    }
}
