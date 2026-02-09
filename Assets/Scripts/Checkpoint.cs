using UnityEngine;
using System.Collections; // Necesario para las animaciones por c�digo

public class Checkpoint : MonoBehaviour
{
    [Header("Animaci�n")]
    // Los corchetes [] significan que esto es una LISTA de im�genes, no solo una
    public Sprite[] fotogramasAnimacion;
    public float velocidadAnimacion = 0.1f; // Tiempo entre cada imagen

    private bool activado = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Al empezar, ponemos la primera imagen de la lista (la bandera roja)
        if (spriteRenderer != null && fotogramasAnimacion.Length > 0)
        {
            spriteRenderer.sprite = fotogramasAnimacion[0];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !activado)
        {
            Health saludJugador = collision.GetComponent<Health>();

            if (saludJugador != null)
            {
                saludJugador.SetRespawnPoint(transform.position);
                activado = true;

                Debug.Log("Checkpoint Activado!");

                // �AQU� EMPIEZA LA MAGIA! Iniciamos la animaci�n
                StartCoroutine(ReproducirAnimacion());
            }
        }
    }

    // Esta funci�n especial nos permite esperar tiempos (segundos)
    IEnumerator ReproducirAnimacion()
    {
        // Recorremos todas las im�genes de la lista una por una
        foreach (Sprite foto in fotogramasAnimacion)
        {
            spriteRenderer.sprite = foto; // Cambiamos la imagen
            yield return new WaitForSeconds(velocidadAnimacion); // Esperamos un poquito
        }
        // Al terminar el bucle, se quedar� puesta la �ltima imagen (la verde final)
    }
}