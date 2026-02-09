using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    [Header("Animación")]
    public Sprite[] fotogramasAnimacion;
    public float velocidadAnimacion = 0.1f;

    private bool activado = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

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
                // Guardamos posicion
                saludJugador.SetRespawnPoint(transform.position);
                activado = true;

                StartCoroutine(ReproducirAnimacion());
            }
        }
    }

    IEnumerator ReproducirAnimacion()
    {
        foreach (Sprite foto in fotogramasAnimacion)
        {
            spriteRenderer.sprite = foto;
            yield return new WaitForSeconds(velocidadAnimacion);
        }
    }
}