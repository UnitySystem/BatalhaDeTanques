using UnityEngine;

public class BulletPhysics : MonoBehaviour
{
    public GameObject explosion;
    public float lifetime;
    private Rigidbody body;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (body.linearVelocity != Vector3.zero)
        {
            transform.forward = body.linearVelocity;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("tank"))
        {
            if (col.gameObject.TryGetComponent<EnemyHealth>(out EnemyHealth component))
            {
                Debug.Log("machucou");
                component.TakeDamage(0.2f);
            }
            GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(exp, 0.5f);
            Destroy(gameObject);
        }
    }
}