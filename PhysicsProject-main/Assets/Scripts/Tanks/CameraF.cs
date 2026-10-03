using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Alvo")]
    [Tooltip("Arraste o Tank/Player aqui")]
    public Transform target; 

    [Header("Settign")]
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Distância da câmera em relação ao tanque
    public float smoothSpeed = 5f; // Velocidade de suavização do movimento

    private void LateUpdate()
    {
        if (target == null) return;

        // Calcula a posição desejada baseada na posição do tanque e na rotação dele para acompanhar as curvas
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);

        // Move suavemente a câmera da posição atual para a posição desejada
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Faz a câmera olhar sempre para o tanque
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}