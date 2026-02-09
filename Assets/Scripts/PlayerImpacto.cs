using UnityEngine;

public class PlayerImpacto : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject nieveFXPrefab; // El prefab del efecto de nieve
    [Tooltip("Velocidad mínima para que salga el efecto. Si es 0, saldrá siempre que toques el suelo.")]
    public float velocidadParaImpacto = 5f; 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Comprobamos si chocamos contra el suelo de nieve
        if (collision.gameObject.CompareTag("SueloNieve"))
        {
            // Miramos la fuerza del impacto para ver si es suficiente
            if (collision.relativeVelocity.magnitude >= velocidadParaImpacto)
            {
                // Instanciamos el efecto justo en el punto de contacto
                Instantiate(nieveFXPrefab, collision.contacts[0].point, Quaternion.identity);
            }
        }
    }
}