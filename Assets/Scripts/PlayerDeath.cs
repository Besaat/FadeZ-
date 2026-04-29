using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
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

        // Disable movement
        if (playerMove != null)
            playerMove.enabled = false;

        // Play death animation then restart
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