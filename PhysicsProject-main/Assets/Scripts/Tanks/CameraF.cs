using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Alvo")]
    [Tooltip("Arraste o Tank/Player aqui")]
    public Transform target;

    [Header("Settign")]
    public Vector3 offset = new Vector3(0f, 5f, -10f);
    public float smoothSpeed = 5f;

    /// <summary>
    /// Atualiza a posição e rotação da câmera após as atualizações de movimento do alvo
    /// </summary>
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + target.TransformDirection(offset);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}