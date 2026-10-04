using UnityEngine;

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

    /// <summary>
    /// Inicializa a referência do Rigidbody e agenda a destruição automática do projétil
    /// </summary>
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Orienta a rotação do projétil na direção do seu vetor de velocidade a cada frame
    /// </summary>
    private void Update()
    {
        if (body != null && body.linearVelocity != Vector3.zero)
        {
            transform.forward = body.linearVelocity;
        }
    }

    /// <summary>
    /// Detecta a colisão do projétil e aciona a explosão se atingir uma camada ou tag válida
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
    /// Instancia os efeitos da explosão, calcula e aplica o dano em área nas unidades atingidas
    /// </summary>
    private void Explode()
    {
        GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(exp, 1.5f);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, tankMask);

        foreach (Collider hit in hitColliders)
        {
            EnemyHealth health = hit.GetComponentInParent<EnemyHealth>();

            if (health != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float damageMultiplier = Mathf.Clamp01(1f - (distance / explosionRadius));
                float calculatedDamage = maxDamage * damageMultiplier;

                health.TakeDamage(calculatedDamage);
            }
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Exibe o raio de explosão do projétil na janela de cena quando o objeto está selecionado
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}