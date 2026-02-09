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

        // Lo ponemos Kinematic para controlarlo nosotros sin que le afecte la física aún
        if(rb != null) rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        if (isDead) return; 

        // Movimiento oscilante simple (Seno) para que flote en el sitio
        float nuevoY = posInicial.y + Mathf.Sin((Time.time + tiempoRandom) * velocidad) * amplitud;
        transform.position = new Vector3(posInicial.x, nuevoY, posInicial.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D point in collision.contacts)
            {
                // Miramos si el golpe viene desde arriba (pisotón)
                if (point.normal.y < -0.4f) 
                {
                    Die(); 

                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    if(playerRb != null)
                    {
                        // Frenamos su caída y le damos el impulso hacia arriba
                        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0); 
                        playerRb.linearVelocity += Vector2.up * bounceForce;
                    }
                    
                    return; 
                }
            }
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if(anim != null) 
        {
            anim.SetTrigger("die");
        }

        // Desactivamos choque para que no vuelva a rebotar
        GetComponent<Collider2D>().enabled = false;

        if (rb != null)
        {
            // Activamos la física real para que se caiga por gravedad
            rb.bodyType = RigidbodyType2D.Dynamic; 
            rb.gravityScale = 3; 
            rb.linearVelocity = new Vector2(0, 5); 
        }

        Destroy(gameObject, 2f);
    }
}