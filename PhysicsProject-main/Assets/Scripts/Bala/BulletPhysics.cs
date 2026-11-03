using UnityEngine;

/// <summary>
/// Controla o comportamento físico dos projéteis, tratamento de colisões 
/// e cálculo de dano em área (splash damage) sobre alvos.
/// </summary>
public class BulletPhysics : MonoBehaviour
{
    public GameObject explosion;
    public float lifetime = 5f;

    [Header("Configurações de Explosão e Dano em Área")]
    public float explosionRadius = 5f;
    public float maxDamage = 50f;
    public LayerMask tankMask;

    [Header("Camadas de Impacto (Paredes, Chão, Jogador, Obstáculos)")]
    public LayerMask impactLayer;

    private Rigidbody body;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        // Destrói o projétil automaticamente após o tempo limite de vida
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Alinha a rotação frontal da bala com a sua velocidade direcional
        if (body != null && body.linearVelocity != Vector3.zero)
        {
            transform.forward = body.linearVelocity;
        }
    }

    /// <summary>
    /// Deteta colisões físicas com o ambiente ou alvos e dispara o evento de explosão.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        bool isImpactLayer = ((1 << collision.gameObject.layer) & impactLayer) != 0;
        bool isImpactTag = collision.gameObject.CompareTag("tank");

        if (isImpactLayer || isImpactTag)
        {
            Explode();
        }
    }

    /// <summary>
    /// Instancia o efeito visual de explosão e aplica dano proporcional por proximidade 
    /// a todos os tanques dentro do raio de alcance.
    /// </summary>
    private void Explode()
    {
        GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(exp, 1.5f);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, tankMask);

        foreach (Collider hit in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            float damageMultiplier = Mathf.Clamp01(1f - (distance / explosionRadius));
            float calculatedDamage = maxDamage * damageMultiplier;

            if (hit.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(calculatedDamage);
            }
            else if (hit.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(calculatedDamage);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}