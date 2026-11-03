using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controla a barra de vida do jogador, danos sofridos e o fluxo de derrota no jogo.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Componente de UI")]
    public Image healthBarFill;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    /// <summary>
    /// Aplica dano ao jogador, atualiza a interface e verifica a condição de derrota.
    /// </summary>
    /// <param name="amount">Quantidade de dano a aplicar.</param>
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthBar();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Atualiza o preenchimento da barra de vida visual do jogador.
    /// </summary>
    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    /// <summary>
    /// Executa a rotina de derrota: calcula os pontos parciais obtidos, 
    /// envia o registo da partida ao middleware (venceu = false) e transiciona para a cena de derrota.
    /// </summary>
    private void Die()
    {
        // Calcula os pontos obtidos com base nos inimigos que ele conseguiu matar antes de morrer
        int pontosIncompletos = 0;
        if (StateGameplay.Instance != null)
        {
            pontosIncompletos = StateGameplay.Instance.TotalDeathTanks * 10;
        }

        // O jogador perdeu! Envia os dados para o PHP (venceu = false)
        if (FindObjectOfType<MatchManager>() != null)
        {
            FindObjectOfType<MatchManager>().FinalizarPartida(pontosIncompletos, false);
        }

        SceneManager.LoadScene("Derrota");
    }
}