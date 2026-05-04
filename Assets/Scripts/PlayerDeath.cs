using UnityEngine;
using UnityEngine.SceneManagement;

// Gerencia a morte do player.
// É ativado pelo LightAmmo quando a luz zera.
// Usa padrão Singleton estático para ser chamado de qualquer lugar com PlayerDeath.Die().
public class PlayerDeath : MonoBehaviour
{
    [Header("Referências")]
    public QuadAnimator anim;       // Animador de sprite (para animação de morte)
    public PlayerMove playerMove;   // Referência ao movimento para desativá-lo

    private static PlayerDeath instance; // Referência estática para chamada global
    private bool isDead = false;

    void Awake()
    {
        instance = this;
    }

    // Método estático chamado pelo LightAmmo quando a luz chega ao limite mínimo
    public static void Die()
    {
        if (instance != null)
            instance.StartDeath();
    }

    void StartDeath()
    {
        if (isDead) return;
        isDead = true;

        // Desativa o movimento imediatamente
        if (playerMove != null)
            playerMove.enabled = false;

        // Toca a animação de morte e reinicia a cena ao terminar
        if (anim != null)
        {
            anim.onDeathComplete = () =>
            {
                // TODO: Futuramente carregar a tela de derrota em vez de reiniciar diretamente
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
