using UnityEngine;

public class Estalactita : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadCaida = 10f;
    public int danio = 1;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Aplicamos velocidad inicial constante hacia abajo
        rb.linearVelocity = Vector2.down * velocidadCaida;
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            otro.GetComponent<Health>()?.TakeDamage(danio);
            Destroy(gameObject);
        }
        else if (otro.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}