using UnityEngine;

public class SkateboardController : MonoBehaviour
{
    [Header("Configurações de movimento")]
    public float walkSpeed = 2f;        // Velocidade normal
    public float runSpeed = 5f;         // Velocidade ao correr (Shift)
    public float rotationSpeed = 100f;  // Velocidade da rotação
    public float acceleration = 5f;     // Quão rápido alcança a velocidade desejada

    [Header("Referências")]
    public Animator animator;           // Animator do personagem

    private float currentSpeed = 0f;
    private float targetSpeed = 0f;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Determina a velocidade alvo (aceleração gradual)
        targetSpeed = (isRunning ? runSpeed : walkSpeed) * moveInput;

        // Interpola suavemente para a velocidade atual
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

        // Movimento frontal/trás
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // Rotação e tilt apenas se estiver se movendo
        if (Mathf.Abs(currentSpeed) > 0.01f)
        {
            // Rotação
            transform.Rotate(Vector3.up * turnInput * rotationSpeed * Time.deltaTime);

            // Inclinação ao virar (tilt)
            float tilt = turnInput * 15f;
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, -tilt);
        }
        else
        {
            // Quando parado, tilt é 0 (sem interpolação)
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
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
}
