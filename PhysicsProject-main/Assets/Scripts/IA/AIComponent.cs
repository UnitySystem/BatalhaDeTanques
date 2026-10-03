using UnityEngine;

public class AIComponent : MonoBehaviour
{
    [Header("Objetos")]
    public GameObject bulletPrefab;
    public GameObject bulletSpawn;
    public GameObject enemy;
    public Transform cannon;

    [Header("Configurações de Combate")]
    public float rotationSpeed = 2.0f;
    public float fireRate = 1.0f;
    public float attackRange = 10.0f;
    public float shellSpeed = 15.0f;

    [Header("Limites de Inclinação do Cano")]
    public float minAngle = -5f;
    public float maxAngle = 25f;

    [Header("Movimentação A*")]
    public float moveSpeed = 5.0f;
    public float nodeReachDistance = 0.5f;

    [SerializeField] private AStarPathfinding pathfinding;

    private StateFSM currentState;

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

    private void Update()
    {
        if (enemy == null || currentState == null) return;

        currentState = currentState.Process();
    }
}