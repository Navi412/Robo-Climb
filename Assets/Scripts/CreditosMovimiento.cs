using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena al final

public class CreditosMovimiento : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadSubida = 50f; // Ajusta esto según lo largo que sea el texto
    public float alturaFinal = 1500f;   // La posición Y donde termina (cuando sale de pantalla)
    public string escenaMenu = "MenuPrincipal"; // Nombre EXACTO de tu escena de menú

    void Update()
    {
        // 1. Mover el texto hacia arriba
        transform.Translate(Vector3.up * velocidadSubida * Time.deltaTime);

        // 2. Comprobar si ya ha terminado
        // Usamos localPosition porque es parte de la UI
        if (transform.localPosition.y > alturaFinal)
        {
            Debug.Log("🏁 Créditos terminados. Volviendo al menú...");
            SceneManager.LoadScene(escenaMenu);
        }
    }
}