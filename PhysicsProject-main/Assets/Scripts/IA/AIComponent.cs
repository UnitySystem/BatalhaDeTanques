using UnityEngine;

public class AIComponent : MonoBehaviour
{
    [Header("Objetos")]
    public GameObject bulletPrefab;
    public GameObject bulletSpawn;
    public GameObject enemy;
    public Transform cannon;

    [Header("Patrulha")]
    public Transform[] patrolPoints = new Transform[4];

    [Header("Configurações de Combate")]
    public float rotationSpeed = 2.0f;
    public float fireRate = 1.0f;
    public float attackRange = 10.0f;
    public float shellSpeed = 15.0f;

    [Header("Configurações de Visão")]
    public float visionDistance = 15.0f;
    public float visionAngle = 45.0f;

    [Header("Gizmos de Visão")]
    public Color visionLineColor = Color.green;

    [Header("Limites de Inclinação do Cano")]
    public float minAngle = -5f;
    public float maxAngle = 25f;

    [Header("Movimentação A*")]
    public float moveSpeed = 5.0f;
    public float nodeReachDistance = 0.5f;

    [SerializeField] private AStarPathfinding pathfinding;

    [Header("Estado Atual")]
    [SerializeField] private StateFSM currentState;

    /// <summary>
    /// Configura os componentes necessários e inicializa a máquina de estados com o estado de patrulha
    /// </summary>
    private void Start()
    {
        if (pathfinding == null)
        {
            pathfinding = FindAnyObjectByType<AStarPathfinding>();
        }

        if (enemy != null)
        {
            currentState = new TankPatrol(gameObject, enemy.transform, this, pathfinding);
        }
    }

    /// <summary>
    /// Executa e atualiza a lógica da máquina de estados do agente inteligente a cada frame
    /// </summary>
    private void Update()
    {
        currentState = currentState.Process();
    }

    /// <summary>
    /// Desenha o campo de visão e o raio de alcance de ataque na janela de cena
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position;

        Vector3 leftRayDirection = Quaternion.Euler(0, -visionAngle, 0) * transform.forward;
        Vector3 rightRayDirection = Quaternion.Euler(0, visionAngle, 0) * transform.forward;

        Gizmos.color = visionLineColor;
        Gizmos.DrawRay(origin, leftRayDirection * visionDistance);
        Gizmos.DrawRay(origin, rightRayDirection * visionDistance);
        Gizmos.DrawRay(origin, transform.forward * visionDistance);

        int segments = 20;
        Vector3 previousPoint = origin + leftRayDirection * visionDistance;
        float stepAngle = (visionAngle * 2f) / segments;

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = -visionAngle + (stepAngle * i);
            Vector3 nextDir = Quaternion.Euler(0, currentAngle, 0) * transform.forward;
            Vector3 nextPoint = origin + nextDir * visionDistance;

            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, attackRange);
    }
}