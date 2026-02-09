using UnityEngine;

public class PlantEnemy : MonoBehaviour
{
    [Header("Ataque")]
    public GameObject bulletPrefab;
    public float shootingRange = 10f;
    public float fireRate = 1.5f;

    [Header("Visión (Ojos)")]
    public LayerMask obstacleLayer;

    [Header("Audio")] 
    public AudioSource audioSource;
    public AudioClip sfxDisparo; 
    [Range(0f, 1f)] public float volumenDisparo = 1f;
    [Range(0.1f, 3f)] public float pitchDisparo = 1f; 

    [Header("Referencias")]
    public Transform puntoDeDisparo;

    private Transform player;
    private float nextFireTime = 0f;
    private Animator anim;

    void Start()
    {
        // Buscamos al player y pillamos los componentes necesarios
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        anim = GetComponent<Animator>();

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null) return;

        // Orientamos el sprite hacia donde esté el jugador
        Vector3 escala = transform.localScale;
        if (player.position.x > transform.position.x)
            escala.x = Mathf.Abs(escala.x);
        else
            escala.x = -Mathf.Abs(escala.x);
        transform.localScale = escala;

        // Comprobamos distancia y tiempo de recarga
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < shootingRange && Time.time > nextFireTime)
        {
            // Solo dispara si no hay paredes bloqueando la visión
            if (CanSeePlayer())
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    bool CanSeePlayer()
    {
        // Lanzamos un rayo para ver si chocamos con obstáculos
        Vector2 direction = player.position - transform.position;
        float distanceToPlayer = direction.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distanceToPlayer, obstacleLayer);

        if (hit.collider != null) return false;
        return true;
    }

    void Shoot()
    {
        if (anim != null) anim.SetTrigger("Disparar");

        // Configuramos el tono y lanzamos el sonido
        if (audioSource != null && sfxDisparo != null)
        {
            audioSource.pitch = pitchDisparo; 
            audioSource.PlayOneShot(sfxDisparo, volumenDisparo);
        }

        // Creamos la bala y le decimos hacia dónde ir
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
        // Dibujo de ayuda en el editor para ver el rango
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
        if (player != null) Gizmos.DrawLine(transform.position, player.position);
    }
}