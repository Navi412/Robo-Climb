using UnityEngine;

public class StaticBat : MonoBehaviour
{
    [Header("Configuración Flotación")]
    public float amplitud = 0.5f;   
    public float velocidad = 2f;    

    [Header("Configuración Rebote")]
    public float bounceForce = 10f; 

    private Vector3 posInicial;
    private float tiempoRandom;
    private Animator anim;
    private bool isDead = false;
    private Rigidbody2D rb;

    void Start()
    {
        posInicial = transform.position;
        tiempoRandom = Random.Range(0f, 10f); 
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if(rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        // CHIVATO 1: Comprobar si encontró el Animator al arrancar
        if (anim == null) Debug.LogError("❌ ERROR CRÍTICO: Este murciélago NO tiene componente Animator.");
        else Debug.Log("✅ Animator encontrado correctamente.");
    }

    void Update()
    {
        if (isDead) return; 

        float nuevoY = posInicial.y + Mathf.Sin((Time.time + tiempoRandom) * velocidad) * amplitud;
        transform.position = new Vector3(posInicial.x, nuevoY, posInicial.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        // CHIVATO 2: Ver con qué estamos chocando
        // Debug.Log("Choque con: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D point in collision.contacts)
            {
                // CHIVATO 3: Ver la dirección exacta del golpe
                // Si sale positivo (0.X) es golpe por abajo/lado. Si sale negativo (-0.X) es por arriba.
                Debug.Log("Normal del choque Y: " + point.normal.y);

                if (point.normal.y < -0.4f) 
                {
                    Debug.Log("✅ ¡PISOTÓN DETECTADO! Ejecutando muerte...");
                    
                    Die(); 

                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    if(playerRb != null)
                    {
                        Debug.Log("🦘 Impulsando al jugador...");
                        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0); 
                        playerRb.linearVelocity += Vector2.up * bounceForce;
                    }
                    
                    return; 
                }
                else
                {
                    Debug.Log("❌ Choque detectado pero NO fue desde arriba (Normal incorrecta)");
                }
            }
        }
    }

    void Die()
    {
        Debug.Log("💀 Entrando en función Die()");

        if (isDead) return;
        isDead = true;

        if(anim != null) 
        {
            Debug.Log("🎬 Enviando Trigger 'die' al Animator");
            anim.SetTrigger("die");
        }

        GetComponent<Collider2D>().enabled = false;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; 
            rb.gravityScale = 3; 
            rb.linearVelocity = new Vector2(0, 5); 
        }

        Destroy(gameObject, 2f);
    }
}