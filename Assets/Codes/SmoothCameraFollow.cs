using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;                // Arrasta o player
    public Rigidbody playerRb;              // (Opcional) Arrasta o Rigidbody do player pra usar velocidade real

    [Header("Configurações de posição")]
    public Vector3 offset = new Vector3(0, 4, -7);
    public float followSpeed = 5f;          // Velocidade de acompanhamento
    public float rotationSpeed = 5f;        // Suavidade da rotação

    [Header("Efeito de balanço")]
    public float swayAmount = 0.2f;         // Força do balanço lateral
    public float swaySpeed = 4f;            // Velocidade do balanço
    public float bobAmount = 0.1f;          // Balanço vertical (andar/correr)
    public float bobSpeed = 6f;             // Frequência do movimento vertical

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (player == null) return;

        // Posição base desejada
        Vector3 targetPos = player.position + offset;

        // Efeito de balanço dinâmico
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount;

        // Aplica o balanço na posição (em função do movimento)
        Vector3 dynamicOffset = new Vector3(sway, bob, 0);
        targetPos += transform.TransformDirection(dynamicOffset);

        // Movimento suave até a posição desejada
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1f / followSpeed);

        // Rotação suave para olhar o jogador
        Quaternion targetRot = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }
}
