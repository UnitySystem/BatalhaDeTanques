using UnityEngine;

/// <summary>
/// Controla o seguimento suave da câmara em relação à posição do tanque do jogador.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Alvo")]
    [Tooltip("Arraste o Tank/Player aqui")]
    public Transform target;

    [Header("Settign")]
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Distância da câmara em relação ao tanque
    public float smoothSpeed = 5f; // Velocidade de suavização do movimento

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}