using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necesario para controlar la Imagen UI
using DG.Tweening; // Necesario para la animación suave

public class LevelPortal : MonoBehaviour
{
    [Header("Configuración del Viaje")]
    [Tooltip("Escribe aquí EXACTAMENTE el nombre de la escena del siguiente nivel")]
    public string siguienteNivel = "Nivel2";

    [Header("Efectos Visuales")]
    [Tooltip("Arrastra aquí la imagen 'CortinaNegra' de tu Canvas")]
    public Image cortinaNegra;
    public float duracionFade = 1f; // Tiempo que tarda en ponerse negro

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el jugador toca el portal
        if (collision.CompareTag("Player"))
        {
            // Opcional: Bloquear el movimiento del jugador para que no se mueva durante el fade
            if (collision.TryGetComponent(out Movement playerMove))
            {
                playerMove.canMove = false;
                playerMove.rb.linearVelocity = Vector2.zero; // Frenarlo en seco
            }

            Debug.Log("🌀 Iniciando transición a: " + siguienteNivel);

            // LOGICA DEL FADE
            if (cortinaNegra != null)
            {
                // Nos aseguramos de que empiece transparente
                cortinaNegra.gameObject.SetActive(true);
                cortinaNegra.color = new Color(0, 0, 0, 0);

                // ANIMACIÓN: Vamos a Alpha 1 (Negro) y AL TERMINAR (.OnComplete), cargamos nivel
                cortinaNegra.DOFade(1f, duracionFade).OnComplete(() =>
                {
                    SceneManager.LoadScene(siguienteNivel);
                });
            }
            else
            {
                // Si se te olvidó poner la imagen, cargamos directo para que no se rompa el juego
                Debug.LogWarning("⚠️ No has asignado la CortinaNegra en el inspector del Portal");
                SceneManager.LoadScene(siguienteNivel);
            }
        }
    }
}