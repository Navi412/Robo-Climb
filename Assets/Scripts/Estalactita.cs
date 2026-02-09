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
        // Aplicamos la velocidad UNA vez y dejamos que el motor de física haga el resto
        // Nota: Si usas una versión anterior a Unity 6, cambia 'linearVelocity' por 'velocity'
        rb.linearVelocity = Vector2.down * velocidadCaida;
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            // Asumiendo que tu script de vida se llama Health
            otro.GetComponent<Health>()?.TakeDamage(danio);
            Destroy(gameObject);
        }
        else if (otro.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}