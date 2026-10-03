using UnityEngine;

public class AITank : MonoBehaviour
{
    [Header("Objects")]
    public GameObject bulletPrefab;
    public GameObject bulletSpawn;
    public GameObject enemy;
    public Transform cannon;
    [Space(10)]
    public float rotationSpeed = 2.0f;
    public float fireRate = 1.0f;
    public float attackRange = 10.0f;
    public float distance;

    [Header("Limites de Inclinação do Cano")]
    public float minAngle = -5f;
    public float maxAngle = 25f;

    float speed = 15.0f;
    float nextFireTime = 0f;

    void Update()
    {
        float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
        distance = distanceToEnemy;

        if (distanceToEnemy <= attackRange)
        {
            float? angle = RotateCannon();

            if (angle != null)
            {
                if ((float)angle >= minAngle && (float)angle <= maxAngle)
                {
                    if (Time.time >= nextFireTime)
                    {
                        CreateBullet();
                        nextFireTime = Time.time + fireRate;
                    }
                }
            }
        }
    }

    void CreateBullet()
    {
        GameObject shell = Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        shell.GetComponent<Rigidbody>().linearVelocity = speed * cannon.forward;
    }

    float? RotateCannon()
    {
        float? angle = CalculateAngle(true);
        if (angle != null)
        {
            float clampedAngle = Mathf.Clamp((float)angle, minAngle, maxAngle);

            Vector3 localTargetPos = transform.InverseTransformPoint(enemy.transform.position);
            localTargetPos.y = 0f;

            Quaternion lookRot = Quaternion.LookRotation(localTargetPos);
            cannon.localRotation = lookRot * Quaternion.Euler(-clampedAngle, 0f, 0f);
        }
        return angle;
    }

    float? CalculateAngle(bool low)
    {
        float angle = 0.0f;
        Vector3 targetDir = enemy.transform.position - transform.position;
        float y = targetDir.y;
        targetDir.y = 0;
        float x = targetDir.magnitude;
        float gravity = 9.81f;
        float sSqr = speed * speed;
        float underTheRoot = sSqr * sSqr - gravity * (gravity * x * x + 2 * y * sSqr);

        if (underTheRoot >= 0)
        {
            float root = Mathf.Sqrt(underTheRoot);
            float highAngle = sSqr + root;
            float lowAngle = sSqr - root;

            if (low)
            {
                angle = Mathf.Atan2(lowAngle, gravity * x) * Mathf.Rad2Deg;
                return angle;
            }
            else
            {
                angle = Mathf.Atan2(highAngle, gravity * x) * Mathf.Rad2Deg;
                return angle;
            }
        }
        else
        {
            return null;
        }
    }
}