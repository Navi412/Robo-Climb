using UnityEngine;

public class PlayerImpacto : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject nieveFXPrefab; // Arrastra aquí tu prefab de la nieve
    [Tooltip("Velocidad mínima para que salga el efecto. Si es 0, saldrá siempre que toques el suelo.")]
    public float velocidadParaImpacto = 5f; 

    // Esta función salta sola cuando el Player choca físicamente con algo
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. ¿Contra qué hemos chocado? ¿Es el suelo de nieve?
        if (collision.gameObject.CompareTag("SueloNieve"))
        {
            // 2. ¿Hemos chocado fuerte?
            // 'relativeVelocity.magnitude' nos dice a qué velocidad ocurrió el golpe.
            if (collision.relativeVelocity.magnitude >= velocidadParaImpacto)
            {
                // 3. ¡PUM! Instanciamos el efecto
                // Usamos 'collision.contacts[0].point' para que salga EXACTAMENTE donde se tocaron (en el suelo)
                Instantiate(nieveFXPrefab, collision.contacts[0].point, Quaternion.identity);
                
                Debug.Log("❄️ ¡Catapum! Golpe de nieve.");
            }
        }
    }
}