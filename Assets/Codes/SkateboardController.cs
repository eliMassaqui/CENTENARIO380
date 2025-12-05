using UnityEngine;

public class SkateboardController : MonoBehaviour
{
    [Header("Configurações de movimento")]
    public float walkSpeed = 2f;        // Velocidade normal
    public float runSpeed = 5f;         // Velocidade ao correr (Shift)
    public float rotationSpeed = 100f;  // Velocidade da rotação

    [Header("Referências")]
    public Animator animator;           // Arrasta o Animator do personagem aqui

    private float currentSpeed = 0f;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        float move = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");
        bool isRunning = Input.GetKey(KeyCode.LeftShift); // Shift ativa corrida

        // Calcula a velocidade atual (normal ou corrida)
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Movimento frontal/trás (sem física real)
        if (move != 0)
        {
            transform.Translate(Vector3.forward * move * currentSpeed * Time.deltaTime);

            if (isRunning)
            {
                animator.SetBool("isRunning", true);   // Ativa animação de corrida
                animator.SetBool("isScooterIdle", false);
                animator.SetBool("isIdle", false);
            }
            else
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isScooterIdle", true); // Animação scooter normal
                animator.SetBool("isIdle", false);
            }
        }
        else
        {
            // Sem movimento → animação parada total
            animator.SetBool("isRunning", false);
            animator.SetBool("isScooterIdle", false);
            animator.SetBool("isIdle", true);
        }

        // Rotação do personagem
        transform.Rotate(Vector3.up * turn * rotationSpeed * Time.deltaTime);

        // Inclinação leve ao virar (efeito visual, opcional)
        float tilt = Mathf.Lerp(0, turn * 15f, Time.deltaTime * 5f);
        transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, -tilt);
    }
}
