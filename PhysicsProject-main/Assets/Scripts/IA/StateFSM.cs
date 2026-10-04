using UnityEngine;

public class StateFSM
{
    public enum STATE
    {
        Patrol, Chase, Attack
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

    /// <summary>
    /// Inicializa uma nova instância da classe base de estado definindo o agente, alvo e estágio inicial
    /// </summary>
    public StateFSM(GameObject _npc, Transform _player)
    {
        npc = _npc;
        player = _player;
        stage = EVENT.ENTER;
    }

    /// <summary>
    /// Transiciona o estágio do estado para atualização contínua ao entrar no estado
    /// </summary>
    public virtual void Enter() { stage = EVENT.UPDATE; }

    /// <summary>
    /// Executa a rotina de atualização contínua pertencente ao estado
    /// </summary>
    public virtual void Update() { stage = EVENT.UPDATE; }

    /// <summary>
    /// Define o estágio de saída antes de transicionar para um novo estado
    /// </summary>
    public virtual void Exit() { stage = EVENT.EXIT; }

    /// <summary>
    /// Controla o ciclo de vida do estado atual e determina quando transicionar para o próximo estado
    /// </summary>
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

    /// <summary>
    /// Lança um raio para checar se a linha de visão do agente até o jogador está livre de obstáculos
    /// </summary>
    public bool CanSeePlayer(AIComponent aiTank)
    {
        Vector3 origin = npc.transform.position + Vector3.up * 1.5f;
        Vector3 target = player.position + Vector3.up * 1.0f;
        Vector3 direction = (target - origin).normalized;
        float distance = Vector3.Distance(origin, target);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            if (hit.transform == player || hit.transform.root == player)
            {
                return true;
            }
        }

        return false;
    }
}