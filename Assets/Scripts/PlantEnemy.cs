using UnityEngine;

public class PlantEnemy : MonoBehaviour
{
    [Header("Ataque")]
    public GameObject bulletPrefab;
    public float shootingRange = 10f;
    public float fireRate = 1.5f;

    [Header("Visión (Ojos)")]
    public LayerMask obstacleLayer;

    [Header("Audio")] // <--- NUEVA SECCIÓN
    public AudioSource audioSource;
    public AudioClip sfxDisparo; // Arrastra aquí el sonido "Pium!"
    [Range(0f, 1f)] public float volumenDisparo = 1f;
    [Range(0.1f, 3f)] public float pitchDisparo = 1f; // Para cambiar si suena grave o agudo

    [Header("Referencias")]
    public Transform puntoDeDisparo;

    private Transform player;
    private float nextFireTime = 0f;
    private Animator anim;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        anim = GetComponent<Animator>();

        // Autocompletar el AudioSource si se te olvida ponerlo
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null) return;

        // --- 1. LÓGICA DE GIRO (FLIP) ---
        Vector3 escala = transform.localScale;
        if (player.position.x > transform.position.x)
            escala.x = Mathf.Abs(escala.x);
        else
            escala.x = -Mathf.Abs(escala.x);
        transform.localScale = escala;

        // --- 2. LÓGICA DE DISPARO ---
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < shootingRange && Time.time > nextFireTime)
        {
            if (CanSeePlayer())
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    bool CanSeePlayer()
    {
        Vector2 direction = player.position - transform.position;
        float distanceToPlayer = direction.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distanceToPlayer, obstacleLayer);

        if (hit.collider != null) return false;
        return true;
    }

    void Shoot()
    {
        // A. Animación
        if (anim != null) anim.SetTrigger("Disparar");

        // B. SONIDO (¡NUEVO!)
        if (audioSource != null && sfxDisparo != null)
        {
            audioSource.pitch = pitchDisparo; // Ajustamos velocidad/tono
            // PlayOneShot es perfecto para disparos: permite que suenen varios a la vez si dispara muy rápido
            audioSource.PlayOneShot(sfxDisparo, volumenDisparo);
        }

        // C. Crear la bala
        if (bulletPrefab != null && puntoDeDisparo != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, puntoDeDisparo.position, Quaternion.identity);
            Vector2 direction = player.position - puntoDeDisparo.position;

            PlantBullet pb = bullet.GetComponent<PlantBullet>();
            if (pb != null)
            {
                pb.SetDirection(direction);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
        if (player != null) Gizmos.DrawLine(transform.position, player.position);
    }
}