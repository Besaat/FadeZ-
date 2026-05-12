using UnityEngine;
using UnityEngine.SceneManagement;

// Gerencia a morte do player.
public class PlayerDeath : MonoBehaviour
{
    [Header("Referências")]
    public QuadAnimator anim;
    public PlayerMove playerMove;

    private static PlayerDeath instance;
    private bool isDead = false;

    void Awake()
    {
        instance = this;
    }

    public static void Die()
    {
        if (instance != null)
            instance.StartDeath();
    }

    void StartDeath()
    {
        if (isDead) return;
        isDead = true;

        if (playerMove != null)
            playerMove.enabled = false;

        // Reseta todos os upgrades ao morrer
        if (UpgradeManager.instance != null)
            UpgradeManager.instance.ResetUpgrades();

        if (anim != null)
        {
            anim.onDeathComplete = () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            };
            anim.PlayDeath();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
