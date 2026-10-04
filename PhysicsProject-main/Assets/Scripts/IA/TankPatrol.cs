using System.Collections.Generic;
using UnityEngine;

public class TankPatrol : StateFSM
{
    private AIComponent aiTank;
    private AStarPathfinding pathfinding;
    private List<Node> currentPath;
    private int currentPathIndex = 0;
    private int currentWaypointIndex = 0;

    /// <summary>
    /// Construtor que inicializa o estado de patrulha com os nós e pontos de patrulha cadastrados
    /// </summary>
    public TankPatrol(GameObject _npc, Transform _player, AIComponent _aiTank, AStarPathfinding _pathfinding)
        : base(_npc, _player)
    {
        state = STATE.Patrol;
        aiTank = _aiTank;
        pathfinding = _pathfinding;
    }

    /// <summary>
    /// Inicializa a rotina de patrulha solicitando a primeira rota até o ponto de interesse
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        RequestNewPathToWaypoint();
    }

    /// <summary>
    /// Controla a movimentação de patrulha e verifica a presença do jogador para realizar transições
    /// </summary>
    public override void Update()
    {
        base.Update();

        if (CanSeePlayer(aiTank))
        {
            float distanceToPlayer = Vector3.Distance(npc.transform.position, player.position);

            if (distanceToPlayer <= aiTank.attackRange)
            {
                nextState = new TankAttack(npc, player, aiTank, pathfinding);
            }
            else
            {
                nextState = new TankChase(npc, player, aiTank, pathfinding);
            }

            stage = EVENT.EXIT;
            return;
        }

        MoveAlongPath();
    }

    /// <summary>
    /// Calcula um caminho via A* da posição do agente até o próximo ponto de patrulha
    /// </summary>
    private void RequestNewPathToWaypoint()
    {
        if (aiTank.patrolPoints == null || aiTank.patrolPoints.Length == 0) return;

        Transform targetPoint = aiTank.patrolPoints[currentWaypointIndex];
        if (pathfinding != null && targetPoint != null)
        {
            currentPath = pathfinding.FindPath(npc.transform.position, targetPoint.position);
            currentPathIndex = 0;
        }
    }

    /// <summary>
    /// Conduz a movimentação e rotação do tanque ao longo dos nós da rota de patrulha
    /// </summary>
    private void MoveAlongPath()
    {
        if (currentPath == null || currentPathIndex >= currentPath.Count)
        {
            AdvanceToNextWaypoint();
            return;
        }

        Vector3 targetPosition = currentPath[currentPathIndex].WorldPosition;
        targetPosition.y = npc.transform.position.y;

        Vector3 moveDir = (targetPosition - npc.transform.position).normalized;

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, targetRotation, Time.deltaTime * aiTank.rotationSpeed * 2f);
        }

        npc.transform.position = Vector3.MoveTowards(npc.transform.position, targetPosition, aiTank.moveSpeed * Time.deltaTime);

        if (Vector3.Distance(npc.transform.position, targetPosition) <= aiTank.nodeReachDistance)
        {
            currentPathIndex++;

            if (currentPathIndex >= currentPath.Count)
            {
                AdvanceToNextWaypoint();
            }
        }
    }

    /// <summary>
    /// Atualiza o índice para o próximo ponto da lista de patrulha e solicita uma nova rota
    /// </summary>
    private void AdvanceToNextWaypoint()
    {
        if (aiTank.patrolPoints == null || aiTank.patrolPoints.Length == 0) return;

        currentWaypointIndex = (currentWaypointIndex + 1) % aiTank.patrolPoints.Length;
        RequestNewPathToWaypoint();
    }

    /// <summary>
    /// Executa os procedimentos de finalização ao sair do estado de patrulha
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }
}