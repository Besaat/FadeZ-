using UnityEngine;

// Gerencia a luz do player como recurso único.
public class LightAmmo : MonoBehaviour
{
    [Header("Referências")]
    public Light sphereLight;
    public QuadAnimator playerAnim;

    [Header("Intensidade")]
    public float maxIntensity = 750f;
    public float drainPerShot = 15f;
    public float rechargeRate = 20f;
    public float deathThreshold = 2f;

    [Header("Range da luz")]
    public float maxRange = 10f;
    public float minRange = 1.5f;

    [Header("Pickup")]
    public float pickupRestoreAmount = 100f;

    // Valores base para reset ao morrer
    [HideInInspector] public float baseMaxIntensity = 750f;
    [HideInInspector] public float baseRechargeRate = 20f;
    [HideInInspector] public float basePickupRestoreAmount = 100f;

    public static int lightBallsCollected = 0;
    [HideInInspector] public float currentIntensity;

    private bool isDead = false;

    // Referência ao DroneFollow para detectar RMB (upgrade 4)
    private DroneFollow droneFollow;

    void Start()
    {
        currentIntensity = maxIntensity;
        lightBallsCollected = 0;

        if (sphereLight == null)
            sphereLight = GetComponentInChildren<Light>();

        droneFollow = FindFirstObjectByType<DroneFollow>();

        // Salva valores base para reset
        baseMaxIntensity = maxIntensity;
        baseRechargeRate = rechargeRate;
        basePickupRestoreAmount = pickupRestoreAmount;
    }

    void Update()
    {
        if (isDead) return;

        // Recarga passiva base
        float currentRecharge = rechargeRate;

        // Upgrade 4: bônus de recarga quando RMB está pressionado (esfera erguida)
        if (droneFollow != null && droneFollow.currentMode == DroneFollow.OrbMode.AreaCharge)
        {
            if (UpgradeManager.instance != null)
                currentRecharge *= UpgradeManager.instance.GetPassiveRechargeBonusMultiplier();
        }

        currentIntensity += currentRecharge * Time.deltaTime;
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

    public void Drain()
    {
        currentIntensity -= drainPerShot;
    }

    public void Drain(float amount)
    {
        currentIntensity -= amount;

        if (playerAnim != null)
            StartCoroutine(PlayHitAnimation());
    }

    System.Collections.IEnumerator PlayHitAnimation()
    {
        playerAnim.PlayHit();
        yield return new WaitForSeconds(0.4f);
    }

    public void Restore(float amount)
    {
        currentIntensity = Mathf.Clamp(currentIntensity + amount, 0f, maxIntensity);
        lightBallsCollected++;
    }
}
