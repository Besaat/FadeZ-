using UnityEngine;
using System.Collections.Generic;

// Gerencia todos os aprimoramentos do jogo.
// Os efeitos acumulam entre fases e resetam ao morrer.
// O aprimoramento 5 (pierce) só pode ser escolhido uma vez por run.
// Nunca aparece o mesmo upgrade duas vezes na mesma seleção de cartas.
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [Header("Referências")]
    public PlayerMove playerMove;
    public PlayerShoot playerShoot;
    public LightAmmo lightAmmo;

    // Contadores de quantas vezes cada upgrade foi aplicado
    private int[] upgradeCounts = new int[7];

    // Controla se o pierce já foi escolhido (upgrade 5 — índice 4)
    private bool pierceUnlocked = false;

    // Nomes dos upgrades para exibir nas cartas
    public static readonly string[] upgradeNames = new string[]
    {
        "Velocidade de Movimento +15%",
        "Velocidade de Ataque +15%",
        "Orbes recuperam +20% de luz",
        "Recarga passiva +30% com esfera erguida",
        "Disparos atravessam inimigos",
        "Ataque em área consome -40% de luz",
        "Luz máxima e inicial +30%"
    };

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Retorna 3 índices de upgrades aleatórios sem repetição para a seleção de cartas
    public int[] GetRandomUpgradeOptions()
    {
        List<int> available = new List<int>();

        for (int i = 0; i < upgradeNames.Length; i++)
        {
            // Upgrade 5 (pierce, índice 4) só aparece se ainda não foi escolhido
            if (i == 4 && pierceUnlocked) continue;
            available.Add(i);
        }

        // Embaralha a lista
        for (int i = available.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = available[i];
            available[i] = available[j];
            available[j] = temp;
        }

        // Retorna os 3 primeiros
        int count = Mathf.Min(3, available.Count);
        int[] result = new int[count];
        for (int i = 0; i < count; i++)
            result[i] = available[i];

        return result;
    }

    // Aplica o upgrade escolhido pelo índice
    public void ApplyUpgrade(int upgradeIndex)
    {
        upgradeCounts[upgradeIndex]++;

        switch (upgradeIndex)
        {
            case 0: ApplyMoveSpeed(); break;
            case 1: ApplyAttackSpeed(); break;
            case 2: ApplyOrbRestore(); break;
            case 3: ApplyPassiveRecharge(); break;
            case 4: ApplyPierce(); break;
            case 5: ApplyAreaCost(); break;
            case 6: ApplyMaxLight(); break;
        }
    }

    // Reseta todos os upgrades ao morrer
    public void ResetUpgrades()
    {
        upgradeCounts = new int[7];
        pierceUnlocked = false;

        // Reaplica os valores base em todos os scripts
        if (playerMove != null)
        {
            playerMove.walkSpeed = playerMove.baseWalkSpeed;
            playerMove.runSpeed = playerMove.baseRunSpeed;
        }

        if (playerShoot != null)
        {
            playerShoot.fireRate = playerShoot.baseFireRate;
            playerShoot.areaDrainAmount = playerShoot.baseAreaDrainAmount;
            playerShoot.pierceEnabled = false;
        }

        if (lightAmmo != null)
        {
            lightAmmo.pickupRestoreAmount = lightAmmo.basePickupRestoreAmount;
            lightAmmo.rechargeRate = lightAmmo.baseRechargeRate;
            lightAmmo.maxIntensity = lightAmmo.baseMaxIntensity;
            lightAmmo.currentIntensity = lightAmmo.baseMaxIntensity;
        }
    }

    // --- Implementação de cada upgrade ---

    void ApplyMoveSpeed()
    {
        if (playerMove == null) return;
        playerMove.walkSpeed *= 1.15f;
        playerMove.runSpeed *= 1.15f;
    }

    void ApplyAttackSpeed()
    {
        if (playerShoot == null) return;
        playerShoot.fireRate *= 0.85f; // Menor fireRate = mais rápido
    }

    void ApplyOrbRestore()
    {
        if (lightAmmo == null) return;
        lightAmmo.pickupRestoreAmount *= 1.20f;
    }

    void ApplyPassiveRecharge()
    {
        if (lightAmmo == null) return;
        // O bônus de recarga com RMB é controlado pelo LightAmmo
        // Apenas registra o upgrade — LightAmmo lê upgradeCounts[3]
    }

    void ApplyPierce()
    {
        if (playerShoot == null) return;
        pierceUnlocked = true;
        playerShoot.pierceEnabled = true;
    }

    void ApplyAreaCost()
    {
        if (playerShoot == null) return;
        playerShoot.areaDrainAmount *= 0.60f;
    }

    void ApplyMaxLight()
    {
        if (lightAmmo == null) return;
        float bonus = lightAmmo.baseMaxIntensity * 0.30f;
        lightAmmo.maxIntensity += bonus;
        lightAmmo.currentIntensity = lightAmmo.maxIntensity;
    }

    // Retorna o bônus de recarga passiva acumulado pelo upgrade 3
    public float GetPassiveRechargeBonusMultiplier()
    {
        return 1f + (upgradeCounts[3] * 0.30f);
    }
}
