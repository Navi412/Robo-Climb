using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Sonido")]
    public AudioClip sonidoMoneda;
    [Range(0f, 1f)] public float volumen = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.SumarMoneda();

            // Usamos el audio del manager para que suene aunque se destruya la moneda
            GameManager.instance.ReproducirSonido(sonidoMoneda, volumen);

            Destroy(gameObject);
        }
    }
}