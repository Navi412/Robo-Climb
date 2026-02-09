using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Sonido")]
    public AudioClip sonidoMoneda;
    [Range(0f, 1f)] public float volumen = 1f; // ¡Asegúrate de ponerlo al 1!

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. Sumamos la moneda
            GameManager.instance.SumarMoneda();

            // 2. Pedimos al Manager que ponga la música (CERO LAG)
            // Ya no creamos objetos temporales, usamos el altavoz del Manager
            GameManager.instance.ReproducirSonido(sonidoMoneda, volumen);

            // 3. Adiós moneda
            Destroy(gameObject);
        }
    }
}