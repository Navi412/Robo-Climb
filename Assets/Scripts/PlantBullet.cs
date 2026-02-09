using UnityEngine;

public class PlantBullet : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 7f;
    public float tiempoDeVida = 4f;
    public int damage = 1; // Variable para controlar cuánto daño hace

    private Vector2 direccionDisparo;
    private bool estaVolando = false;

    void Start()
    {
        Destroy(gameObject, tiempoDeVida); // Autodestrucción por tiempo
    }

    void Update()
    {
        if (estaVolando)
        {
            // Moverse en la dirección correcta
            transform.Translate(direccionDisparo * velocidad * Time.deltaTime, Space.World);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direccionDisparo = dir.normalized;
        estaVolando = true;

        // Giramos el dibujo para que mire hacia donde va
        float angle = Mathf.Atan2(direccionDisparo.y, direccionDisparo.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Ignorar a la planta y otros enemigos para no matarlos
        if (other.CompareTag("Enemy")) return;

        // 2. Chocar con Jugador
        if (other.CompareTag("Player"))
        {
            // --- CORRECCIÓN APLICADA AQUÍ ---
            // Buscamos el script "Health" que tiene tu jugador (el mismo que usaba la seta)
            Health playerHealth = other.GetComponent<Health>();

            // Si el jugador tiene vida, se la quitamos
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            // --------------------------------

            // Destruimos la bala al impactar
            Destroy(gameObject);
        }
        // 3. Chocar con Paredes (Suelo)
        // Destruimos la bala si choca con algo que NO sea un trigger (como una pared sólida)
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}