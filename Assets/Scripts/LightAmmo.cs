using UnityEngine;
using UnityEngine.SceneManagement;

public class LightAmmo : MonoBehaviour
{
    [Header("Config")]
    public Light sphereLight;
    public float maxIntensity = 750f;
    public float currentIntensity;
    public float drainPerShot = 15f;
    public float rechargeRate = 2f;
    public float deathThreshold = 2f;
    public float pickupRestoreAmount = 100f;

    private bool isDead = false;

    // Static resource counter
    public static int lightBallsCollected = 0;

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
        sphereLight.intensity = currentIntensity;
        if (currentIntensity <= deathThreshold)
        {
            isDead = true;
            PlayerDeath.Die();
        }
    }

   public void Drain()
{
    currentIntensity -= drainPerShot;
}

// Add this:
public void Drain(float amount)
{
    currentIntensity -= amount;
}

public void Restore(float amount)
{
    currentIntensity += amount;
    currentIntensity = Mathf.Clamp(currentIntensity, 0f, maxIntensity);
    lightBallsCollected++;
}
}