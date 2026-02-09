using UnityEngine;

public class PlantBullet : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 7f;
    public float tiempoDeVida = 4f;
    public int damage = 1; 

    private Vector2 direccionDisparo;
    private bool estaVolando = false;

    void Start()
    {
        // Programamos que se borre sola al rato para no llenar la memoria
        Destroy(gameObject, tiempoDeVida); 
    }

    void Update()
    {
        if (estaVolando)
        {
            // Movemos la bala en la dirección que le hemos dicho
            transform.Translate(direccionDisparo * velocidad * Time.deltaTime, Space.World);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direccionDisparo = dir.normalized;
        estaVolando = true;

        // Calculamos el ángulo para rotar el sprite y que mire hacia donde va
        float angle = Mathf.Atan2(direccionDisparo.y, direccionDisparo.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si choca con otro enemigo pasamos, no hay fuego amigo
        if (other.CompareTag("Enemy")) return;

        if (other.CompareTag("Player"))
        {
            // Buscamos el script de vida del player y le restamos salud
            Health playerHealth = other.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Borramos la bala al chocar
            Destroy(gameObject);
        }
        // Si choca con el escenario (paredes, suelo) se destruye también
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}