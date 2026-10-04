using UnityEngine;
using UnityEngine.InputSystem;

public class TankComponent : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 10f;
    public float rotationSpeed = 100f;
    public bool invertRotationWhenBackingUp = true;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference rotateUp;
    public InputActionReference rotateDown;

    [Header("Cannon & Shooting Settings")]
    public Transform cannon;
    public Transform bulletSpawn;
    public GameObject bulletPrefab;
    public float bulletSpeed = 15f;

    [Header("Cannon Limits")]
    public float minCannonAngle = -20f;
    public float maxCannonAngle = 45f;
    private float currentCannonRotationX = 0f;

    /// <summary>
    /// Habilita as ações de entrada de movimento e rotação quando o componente é ativado
    /// </summary>
    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
        if (rotateUp != null) rotateUp.action.Enable();
        if (rotateDown != null) rotateDown.action.Enable();
    }

    /// <summary>
    /// Desabilita as ações de entrada de movimento e rotação quando o componente é desativado
    /// </summary>
    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
        if (rotateUp != null) rotateUp.action.Disable();
        if (rotateDown != null) rotateDown.action.Disable();
    }

    /// <summary>
    /// Processa o controle de movimento do tanque, rotação do canhão e disparos a cada frame
    /// </summary>
    private void Update()
    {
        HandleMovement();
        HandleCannonRotation();
        HandleShooting();
    }

    /// <summary>
    /// Lê as entradas do jogador para realizar a translação e rotação do tanque
    /// </summary>
    private void HandleMovement()
    {
        if (moveAction == null) return;

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        float translation = moveInput.y * speed * Time.deltaTime;
        float rotation = moveInput.x * rotationSpeed * Time.deltaTime;

        if (invertRotationWhenBackingUp && moveInput.y < 0)
        {
            rotation = -rotation;
        }

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }

    /// <summary>
    /// Ajusta a inclinação vertical do canhão com base nas entradas e aplica limites de ângulo
    /// </summary>
    private void HandleCannonRotation()
    {
        if (cannon == null) return;
        float rotateAmount = 0f;

        if (rotateUp != null && rotateUp.action.IsPressed())
        {
            rotateAmount -= rotationSpeed * Time.deltaTime;
        }
        if (rotateDown != null && rotateDown.action.IsPressed())
        {
            rotateAmount += rotationSpeed * Time.deltaTime;
        }
        if (rotateAmount != 0f)
        {
            currentCannonRotationX += rotateAmount;
            currentCannonRotationX = Mathf.Clamp(currentCannonRotationX, minCannonAngle, maxCannonAngle);

            Vector3 currentEuler = cannon.localEulerAngles;
            currentEuler.x = currentCannonRotationX;
            cannon.localEulerAngles = currentEuler;
        }
    }

    /// <summary>
    /// Instancia um projétil e aplica velocidade na direção do ponto de disparo ao clicar no botão esquerdo do mouse
    /// </summary>
    private void HandleShooting()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (bulletPrefab != null && bulletSpawn != null)
            {
                GameObject shell = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
                if (shell.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.linearVelocity = bulletSpawn.forward * bulletSpeed;
                }
            }
        }
    }
}