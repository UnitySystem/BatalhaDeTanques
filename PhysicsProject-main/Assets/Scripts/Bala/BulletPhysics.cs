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

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (body != null && body.linearVelocity != Vector3.zero)
        {
            transform.forward = body.linearVelocity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 1. Verifica se a camada (Layer) do objeto colidido está selecionada na impactLayer
        bool isImpactLayer = ((1 << collision.gameObject.layer) & impactLayer) != 0;

        bool isImpactTag = collision.gameObject.CompareTag("tank");

        if (isImpactLayer || isImpactTag)
        {
            Explode();
        }
    }

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

        // Destrói a bala após o impacto
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}