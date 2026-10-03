using System.Collections.Generic;
using UnityEngine;

public class TankPatrol : StateFSM
{
    private AIComponent aiTank;
    private AStarPathfinding pathfinding;
    private List<Node> currentPath;
    private int currentPathIndex = 0;

    public TankPatrol(GameObject _npc, Transform _player, AIComponent _aiTank, AStarPathfinding _pathfinding)
        : base(_npc, _player)
    {
        name = STATE.Patrol;
        aiTank = _aiTank;
        pathfinding = _pathfinding;
    }

    public override void Enter()
    {
        base.Enter();
        RequestNewPath();
    }

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

        MoveAlongPath();
    }

    private void RequestNewPath()
    {
        if (pathfinding != null && player != null)
        {
            currentPath = pathfinding.FindPath(npc.transform.position, player.position);
            currentPathIndex = 0;
        }
    }

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

    public override void Exit()
    {
        base.Exit();
    }
}