using System.Collections.Generic;
using UnityEngine;

public class TankPatrol : StateFSM
{
    private AIComponent aiTank;
    private AStarPathfinding pathfinding;
    private List<Node> currentPath;
    private int currentPathIndex = 0;
    private int currentWaypointIndex = 0;

    public TankPatrol(GameObject _npc, Transform _player, AIComponent _aiTank, AStarPathfinding _pathfinding)
        : base(_npc, _player)
    {
        state = STATE.Patrol;
        aiTank = _aiTank;
        pathfinding = _pathfinding;
    }

    public override void Enter()
    {
        base.Enter();
        RequestNewPathToWaypoint();
    }

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

    private void AdvanceToNextWaypoint()
    {
        if (aiTank.patrolPoints == null || aiTank.patrolPoints.Length == 0) return;

        currentWaypointIndex = (currentWaypointIndex + 1) % aiTank.patrolPoints.Length;
        RequestNewPathToWaypoint();
    }

    public override void Exit()
    {
        base.Exit();
    }
}