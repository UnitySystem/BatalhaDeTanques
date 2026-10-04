using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Componente de UI")]
    public Image healthBarFill;
    public GameObject explosion;

    /// <summary>
    /// Inicializa a vida atual com o valor máximo e atualiza a barra de vida da interface
    /// </summary>
    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    /// <summary>
    /// Subtrai o valor do dano da vida atual, limita os valores e verifica se a unidade deve morrer
    /// </summary>
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
    /// Atualiza o preenchimento da barra de vida na interface de usuário proporcionalmente à vida atual
    /// </summary>
    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    /// <summary>
    /// Instancia o efeito de explosão e destrói o objeto do inimigo
    /// </summary>
    private void Die()
    {
        GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(exp, 1.5f);
        Destroy(gameObject);
    }
}