using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

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