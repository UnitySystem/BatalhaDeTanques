using UnityEngine;

public class StateFSM
{
    public enum STATE
    {
        Patrol,Chase,Attack
    };

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public STATE state;
    protected EVENT stage;
    protected GameObject npc;
    protected Transform player;
    protected StateFSM nextState;

    public StateFSM(GameObject _npc, Transform _player)
    {
        npc = _npc;
        player = _player;
        stage = EVENT.ENTER;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }

    public StateFSM Process()
    {
        if (stage == EVENT.ENTER)
        {
            Enter();
            Debug.Log("Esse é o novo estado: " + state);
        } 
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        
        return this;
    }

    public bool CanSeePlayer(AIComponent aiTank, bool ignoreAngle = false)
    {
        Vector3 direction = player.position - npc.transform.position;

        if (direction.magnitude > aiTank.visionDistance)
        {
            return false;
        }

        if (ignoreAngle)
        {
            return true;
        }

        float angle = Vector3.Angle(direction, npc.transform.forward);
        return angle <= aiTank.visionAngle;
    }
}