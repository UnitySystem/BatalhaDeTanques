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

    public bool CanSeePlayer(AIComponent aiTank)
    {
        Vector3 origin = npc.transform.position + Vector3.up * 1.5f; // Altura do canhão/visão
        Vector3 target = player.position + Vector3.up * 1.0f;
        Vector3 direction = (target - origin).normalized;
        float distance = Vector3.Distance(origin, target);

        // Lança o raio até a distância do jogador
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            // Se colidiu diretamente com o jogador (ou com algo cujo root seja o jogador)
            if (hit.transform == player || hit.transform.root == player)
            {
                return true; // Visão limpa!
            }
        }

        return false; // Existe um obstáculo bloqueando a visão
    }
}