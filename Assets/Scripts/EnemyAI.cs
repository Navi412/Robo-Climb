using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Configuración General")]
    public float speed = 2f;
    public float bounceForce = 10f;

    [Header("Inteligencia Artificial")]
    public float detectionRange = 5f;
    public float patrolDistance = 3f;
    public LayerMask obstacleLayer;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Configuración de Pasos")]
    public AudioClip sfxPasos;
    [Range(0f, 1f)] public float volumenPasos = 0.3f;
    [Range(0.1f, 3f)] public float pitchPasos = 0.6f; // <--- ¡NUEVO! Velocidad de pasos

    [Header("Configuración de Muerte")]
    public AudioClip sfxMuerte;
    [Range(0f, 1f)] public float volumenMuerte = 1f;

    [Header("Referencias")]
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private bool isDead = false;

    private Vector3 startPosition;
    private bool movingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        startPosition = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null || isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange && CanSeePlayer())
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        GestionarSonidoPasos();
    }

    void GestionarSonidoPasos()
    {
        // Si se está moviendo
        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            if (!audioSource.isPlaying || audioSource.clip != sfxPasos)
            {
                audioSource.volume = volumenPasos;
                audioSource.pitch = pitchPasos; // <--- APLICAMOS LA VELOCIDAD AQUÍ

                audioSource.clip = sfxPasos;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying && audioSource.clip == sfxPasos)
            {
                audioSource.Stop();
            }
        }
    }

    bool CanSeePlayer()
    {
        Vector2 direction = player.position - transform.position;
        float distance = direction.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleLayer);

        if (hit.collider != null) return false;
        return true;
    }

    void ChasePlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        Move(direction);
    }

    void Patrol()
    {
        if (transform.position.x > startPosition.x + patrolDistance)
        {
            movingRight = false;
        }
        else if (transform.position.x < startPosition.x - patrolDistance)
        {
            movingRight = true;
        }

        float direction = movingRight ? 1f : -1f;
        Move(direction);
    }

    void Move(float direction)
    {
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        if (direction > 0) sr.flipX = true;
        else if (direction < 0) sr.flipX = false;
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

            audioSource.volume = volumenMuerte;
            audioSource.pitch = 1f; // <--- RESTAURAMOS VELOCIDAD NORMAL PARA LA MUERTE

            if (sfxMuerte != null) audioSource.PlayOneShot(sfxMuerte);
        }

        anim.SetTrigger("die");
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;

        float tiempoEspera = 1.1f;
        if (sfxMuerte != null && sfxMuerte.length > tiempoEspera)
        {
            tiempoEspera = sfxMuerte.length;
        }

        Destroy(gameObject, tiempoEspera);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;
        Vector3 center = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawLine(new Vector3(center.x - patrolDistance, center.y, 0), new Vector3(center.x + patrolDistance, center.y, 0));
    }
}