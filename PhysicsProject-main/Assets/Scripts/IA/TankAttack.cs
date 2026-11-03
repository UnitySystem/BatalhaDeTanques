using UnityEngine;

/// <summary>
/// Estado de ataque da FSM: gere o alinhamento balístico do canhão, 
/// cálculo de trajetória parabólica e disparos contra o jogador.
/// </summary>
public class TankAttack : StateFSM
{
    private AIComponent aiTank;
    private AStarPathfinding pathfinding;
    private float nextFireTime = 0f;

    /// <summary>
    /// Construtor do estado de ataque.
    /// </summary>
    public TankAttack(GameObject _npc, Transform _player, AIComponent _aiTank, AStarPathfinding _pathfinding)
        : base(_npc, _player)
    {
        state = STATE.Attack;
        aiTank = _aiTank;
        pathfinding = _pathfinding;
    }

    /// <summary>
    /// Ações executadas ao entrar no estado de ataque.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
    }

    /// <summary>
    /// Atualiza a lógica de combate, valida distâncias, linha de visão e cadência de tiro.
    /// </summary>
    public override void Update()
    {
        base.Update();

        float distanceToPlayer = Vector3.Distance(npc.transform.position, player.position);

        if (distanceToPlayer > aiTank.attackRange)
        {
            nextState = new TankChase(npc, player, aiTank, pathfinding);
            stage = EVENT.EXIT;
            return;
        }

        if (!CanSeePlayer(aiTank))
        {
            nextState = new TankChase(npc, player, aiTank, pathfinding);
            stage = EVENT.EXIT;
            return;
        }

        float? angle = RotateCannon();

        if (angle != null)
        {
            if ((float)angle >= aiTank.minAngle && (float)angle <= aiTank.maxAngle)
            {
                if (Time.time >= nextFireTime)
                {
                    CreateBullet();
                    nextFireTime = Time.time + aiTank.fireRate;
                }
            }
        }
    }

    /// <summary>Instancia e dispara o projétil balístico do inimigo.</summary>
    private void CreateBullet()
    {
        GameObject shell = Object.Instantiate(aiTank.bulletPrefab, aiTank.bulletSpawn.transform.position, aiTank.bulletSpawn.transform.rotation);
        shell.GetComponent<Rigidbody>().linearVelocity = aiTank.shellSpeed * aiTank.cannon.forward;
    }

    /// <summary>Calcula e aplica a rotação necessária ao canhão para acertar o alvo.</summary>
    private float? RotateCannon()
    {
        float? angle = CalculateAngle(true);
        if (angle != null)
        {
            float clampedAngle = Mathf.Clamp((float)angle, aiTank.minAngle, aiTank.maxAngle);

            Vector3 localTargetPos = npc.transform.InverseTransformPoint(player.position);
            localTargetPos.y = 0f;

            Quaternion lookRot = Quaternion.LookRotation(localTargetPos);
            aiTank.cannon.localRotation = lookRot * Quaternion.Euler(-clampedAngle, 0f, 0f);
        }
        return angle;
    }

    /// <summary>Fórmula balística para calcular o ângulo de lançamento do projétil.</summary>
    private float? CalculateAngle(bool low)
    {
        Vector3 targetDir = player.position - npc.transform.position;
        float y = targetDir.y;
        targetDir.y = 0;
        float x = targetDir.magnitude;
        float gravity = 9.81f;
        float sSqr = aiTank.shellSpeed * aiTank.shellSpeed;
        float underTheRoot = sSqr * sSqr - gravity * (gravity * x * x + 2 * y * sSqr);

        if (underTheRoot >= 0)
        {
            float root = Mathf.Sqrt(underTheRoot);
            float highAngle = sSqr + root;
            float lowAngle = sSqr - root;

            return Mathf.Atan2(low ? lowAngle : highAngle, gravity * x) * Mathf.Rad2Deg;
        }

        return null;
    }

    /// <summary>Ações executadas ao sair do estado de ataque.</summary>
    public override void Exit()
    {
        base.Exit();
    }
}