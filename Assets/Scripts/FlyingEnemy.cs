using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Configuración Vuelo")]
    public float speed = 3f;
    public float chaseRange = 10f;
    public float bounceForce = 10f;

    [Header("Inteligencia (Esquivar)")]
    public LayerMask obstacleLayer;
    public float avoidForce = 2f;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Sonido Vuelo")]
    public AudioClip sfxVuelo;
    [Range(0f, 1f)] public float volumenVuelo = 0.5f;
    [Range(0.1f, 3f)] public float pitchVuelo = 1f; // <--- ¡NUEVA BARRA! Controla la velocidad

    [Header("Sonido Muerte")]
    public AudioClip sfxMuerte;
    [Range(0f, 1f)] public float volumenMuerte = 1f;

    [Header("Referencias")]
    private Transform player;
    private SpriteRenderer sr;
    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (isDead || player == null || audioSource == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < chaseRange)
        {
            MoverMurcielago();
            GestionarSonidoVuelo(true);
        }
        else
        {
            GestionarSonidoVuelo(false);
        }
    }

    void MoverMurcielago()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1.5f, obstacleLayer);

        if (hit.collider != null)
        {
            direction += Vector2.up * avoidForce;
            direction.Normalize();
        }

        transform.Translate(direction * speed * Time.deltaTime);

        if (player.position.x > transform.position.x) sr.flipX = false;
        else sr.flipX = true;
    }

    void GestionarSonidoVuelo(bool debeVolar)
    {
        if (audioSource == null) return;

        if (debeVolar)
        {
            // Solo iniciamos si no está sonando ya el aleteo
            if (audioSource.clip != sfxVuelo || !audioSource.isPlaying)
            {
                audioSource.clip = sfxVuelo;
                audioSource.volume = volumenVuelo;
                audioSource.pitch = pitchVuelo; // <--- APLICAMOS LA VELOCIDAD AQUÍ
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.clip == sfxVuelo && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D point in collision.contacts)
            {
                if (point.normal.y < -0.5f)
                {
                    Die();
                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0);
                        playerRb.linearVelocity += Vector2.up * bounceForce;
                    }
                }
                else
                {
                    Health playerHealth = collision.gameObject.GetComponent<Health>();
                    if (playerHealth != null) playerHealth.TakeDamage(1);
                }
            }
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (audioSource != null)
        {
            audioSource.Stop();

            // Ajustes para la muerte:
            audioSource.pitch = 1f; // <--- RESTAURAMOS VELOCIDAD NORMAL
            audioSource.volume = volumenMuerte;
            audioSource.loop = false;

            if (sfxMuerte != null)
            {
                audioSource.PlayOneShot(sfxMuerte);
            }
        }

        anim.SetTrigger("die");
        GetComponent<Collider2D>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 3;
            rb.linearVelocity = new Vector2(0, 5);
        }

        float tiempoEspera = 1f;
        if (sfxMuerte != null && sfxMuerte.length > tiempoEspera)
        {
            tiempoEspera = sfxMuerte.length;
        }

        Destroy(gameObject, tiempoEspera);
    }
}