using UnityEngine;

// Gerencia a luz do player, que funciona como recurso único:
// é munição (cada tiro drena luz), vida (sem luz o player morre)
// e pode ser restaurada coletando orbes dropados por inimigos.
public class LightAmmo : MonoBehaviour
{
    [Header("Referências")]
    public Light sphereLight; // A luz 3D acoplada ao orbe do player

    [Header("Configuração")]
    public float maxIntensity = 750f;      // Intensidade máxima da luz
    public float drainPerShot = 15f;       // Luz consumida por tiro
    public float rechargeRate = 2f;        // Luz regenerada por segundo (passiva)
    public float deathThreshold = 2f;      // Intensidade abaixo da qual o player morre
    public float pickupRestoreAmount = 100f; // Luz restaurada ao coletar um orbe

    // Contador de orbes coletados nessa expedição (recurso que leva de volta à base)
    public static int lightBallsCollected = 0;

    [HideInInspector] public float currentIntensity; // Valor atual da luz

    private bool isDead = false;

    void Start()
    {
        currentIntensity = maxIntensity;
        lightBallsCollected = 0;

        // Busca o componente Light no filho se não atribuído no Inspector
        if (sphereLight == null)
            sphereLight = GetComponentInChildren<Light>();
    }

    void Update()
    {
        if (isDead) return;

        // Regeneração passiva de luz
        currentIntensity += rechargeRate * Time.deltaTime;
        currentIntensity = Mathf.Clamp(currentIntensity, 0f, maxIntensity);

        // Atualiza intensidade visual da luz 3D
        if (sphereLight != null)
            sphereLight.intensity = currentIntensity;

        // Verifica condição de morte
        if (currentIntensity <= deathThreshold)
        {
            isDead = true;
            PlayerDeath.Die();
        }
    }

    // Consome a quantidade padrão de luz (chamado ao atirar)
    public void Drain()
    {
        currentIntensity -= drainPerShot;
    }

    // Consome uma quantidade específica de luz (chamado por dano de inimigo)
    public void Drain(float amount)
    {
        currentIntensity -= amount;
    }

    // Restaura luz ao coletar pickup e registra o recurso coletado
    public void Restore(float amount)
    {
        currentIntensity = Mathf.Clamp(currentIntensity + amount, 0f, maxIntensity);
        lightBallsCollected++;
    }
}
