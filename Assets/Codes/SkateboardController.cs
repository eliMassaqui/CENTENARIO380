using UnityEngine;

public class SkateboardController : MonoBehaviour
{
    [Header("Configurações de movimento")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float rotationSpeed = 100f;
    public float acceleration = 5f;

    [Header("Configurações de pulo")]
    public float jumpHeight = 0.5f;        // Altura do pulo
    public float jumpDuration = 0.5f;      // Duração do pulo (subida + descida)
    public float groundCheckDistance = 1f; // Distância do Raycast para medir altura do chão
    public LayerMask groundLayer;          // Layer do chão

    [Header("Referências")]
    public Animator animator;

    private float currentSpeed = 0f;
    private float targetSpeed = 0f;

    // Pulo
    private bool isJumping = false;
    private float jumpProgress = 0f;
    private float offsetY = 0f;            // Offset vertical do pulo

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.freezeRotation = true;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Velocidade alvo
        targetSpeed = (isRunning ? runSpeed : walkSpeed) * moveInput;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

        // Movimento horizontal
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // Rotação e tilt
        if (Mathf.Abs(currentSpeed) > 0.01f)
        {
            transform.Rotate(Vector3.up * turnInput * rotationSpeed * Time.deltaTime);
            float tilt = turnInput * 15f;
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, -tilt);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
        }

        // Medir altura do chão usando Raycast
        float groundHeight = GetGroundHeight();

        // Pulo: apenas para frente (moveInput > 0)
        if (!isJumping && moveInput > 0f)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isJumping = true;
                jumpProgress = 0f;
                animator.SetTrigger("Jump");
            }
        }

        // Atualiza pulo
        if (isJumping)
        {
            jumpProgress += Time.deltaTime / jumpDuration;
            offsetY = Mathf.Sin(jumpProgress * Mathf.PI) * jumpHeight;

            Vector3 pos = transform.position;
            pos.y = groundHeight + offsetY;
            transform.position = pos;

            if (jumpProgress >= 1f)
            {
                isJumping = false;
                offsetY = 0f;
                Vector3 resetPos = transform.position;
                resetPos.y = groundHeight;
                transform.position = resetPos;
            }
        }
        else
        {
            // Garante que o personagem siga a altura do chão quando não estiver pulando
            Vector3 pos = transform.position;
            pos.y = groundHeight;
            transform.position = pos;
        }

        // Atualiza animações
        if (Mathf.Abs(currentSpeed) > 0.01f)
        {
            if (isRunning)
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isScooterIdle", false);
                animator.SetBool("isIdle", false);
            }
            else
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isScooterIdle", true);
                animator.SetBool("isIdle", false);
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isScooterIdle", false);
            animator.SetBool("isIdle", true);
        }
    }

    // Retorna a altura do chão abaixo do personagem
    float GetGroundHeight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            return hit.point.y;
        }
        else
        {
            // Se não detectar chão, retorna altura atual
            return transform.position.y;
        }
    }
}
