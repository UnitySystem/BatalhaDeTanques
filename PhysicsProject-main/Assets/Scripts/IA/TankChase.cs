using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Estado de perseguição da FSM: utiliza o algoritmo A* para guiar o NPC 
/// em direção ao jogador em tempo real.
/// </summary>
public class TankChase : StateFSM
{
    private AIComponent aiTank;
    private AStarPathfinding pathfinding;
    private List<Node> currentPath;
    private int currentPathIndex = 0;

    private float lostVisionTimer = 0f;
    private float maxLostVisionTime = 2.0f;

    /// <summary>
    /// Construtor do estado de perseguição.
    /// </summary>
    public TankChase(GameObject _npc, Transform _player, AIComponent _aiTank, AStarPathfinding _pathfinding)
        : base(_npc, _player)
    {
        state = STATE.Chase;
        aiTank = _aiTank;
        pathfinding = _pathfinding;
    }

    /// <summary>
    /// Inicializa o temporizador de visão e solicita o primeiro caminho via A*.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
        lostVisionTimer = 0f;
        RequestNewPath();
    }

    /// <summary>
    /// Atualiza a lógica de perseguição, checa transições para ataque ou patrulha.
    /// </summary>
    public override void Update()
    {
        base.Update();

        float distanceToPlayer = Vector3.Distance(npc.transform.position, player.position);

        if (distanceToPlayer <= aiTank.attackRange)
        {
            nextState = new TankAttack(npc, player, aiTank, pathfinding);
            stage = EVENT.EXIT;
            return;
        }

        if (!CanSeePlayer(aiTank))
        {
            lostVisionTimer += Time.deltaTime;

            if (lostVisionTimer >= maxLostVisionTime)
            {
                nextState = new TankPatrol(npc, player, aiTank, pathfinding);
                stage = EVENT.EXIT;
                return;
            }
        }
        else
        {
            lostVisionTimer = 0f;
        }

        MoveAlongPath();
    }

    /// <summary>Solicita o cálculo de um novo caminho ao sistema A* até à posição atual do jogador.</summary>
    private void RequestNewPath()
    {
        currentPath = pathfinding.FindPath(npc.transform.position, player.position);
        currentPathIndex = 0;
    }

    /// <summary>Move o NPC sequencialmente ao longo dos nós calculados pelo A*.</summary>
    private void MoveAlongPath()
    {
        if (currentPath == null || currentPathIndex >= currentPath.Count)
        {
            RequestNewPath();
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
        }
    }

    /// <summary>Ações executadas ao sair do estado de perseguição.</summary>
    public override void Exit()
    {
        base.Exit();
    }
}