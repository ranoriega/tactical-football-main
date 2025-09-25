using UnityEngine;

public class Goalkeeper : MonoBehaviour
{
    [Header("Referencias")]
    public Transform ball;          // arrastra aquí el balón (Ball)

    [Header("Ajustes de movimiento")]
    public float moveSpeed = 3f;    // velocidad lateral del arquero
    public float rangeX = 2f;       // cuánto se mueve de izquierda a derecha

    [Header("Ajustes de atajada")]
    public float saveForce = 5f;    // fuerza con la que despeja la pelota

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (ball == null) return;

        // Seguir al balón solo en el eje X, pero limitado
        float targetX = Mathf.Clamp(ball.position.x, startPos.x - rangeX, startPos.x + rangeX);

        Vector3 targetPos = new Vector3(targetX, transform.position.y, startPos.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("¡Atajada del arquero!");
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(Vector3.up * saveForce + -transform.forward * 3f, ForceMode.Impulse);
            }
        }
    }
}
