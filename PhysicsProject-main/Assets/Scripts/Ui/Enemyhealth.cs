using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gere a vida e o estado de integridade dos tanques inimigos no jogo.
/// </summary>
public class EnemyHealth : MonoBehaviour
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
    /// Aplica dano ao inimigo, atualiza a barra de vida gráfica e 
    /// aciona a destruição caso a vida chegue a zero.
    /// </summary>
    /// <param name="amount">Quantidade de dano recebido.</param>
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
    /// Atualiza visualmente o preenchimento da barra de vida na interface.
    /// </summary>
    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    /// <summary>
    /// Regista a destruição do inimigo no sistema de jogo e remove o GameObject da cena.
    /// </summary>
    private void Die()
    {
        StateGameplay.Instance.IncreadeDeathTanks(1);
        Destroy(gameObject);
    }
}