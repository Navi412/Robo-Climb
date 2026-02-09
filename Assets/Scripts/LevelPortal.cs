using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using DG.Tweening; 

public class LevelPortal : MonoBehaviour
{
    [Header("Configuración del Viaje")]
    // Nombre exacto de la escena a cargar
    public string siguienteNivel = "Nivel2";

    [Header("Efectos Visuales")]
    // La imagen negra del Canvas para el fundido
    public Image cortinaNegra;
    public float duracionFade = 1f; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si es el jugador quien entra
        if (collision.CompareTag("Player"))
        {
            // Bloqueamos el movimiento para que no se pueda mover mientras carga
            if (collision.TryGetComponent(out Movement playerMove))
            {
                playerMove.canMove = false;
                playerMove.rb.linearVelocity = Vector2.zero; // Frenamos al personaje
            }

            // Si tenemos la imagen asignada, hacemos el efecto fade
            if (cortinaNegra != null)
            {
                cortinaNegra.gameObject.SetActive(true);
                cortinaNegra.color = new Color(0, 0, 0, 0); // Nos aseguramos que empiece transparente

                // Usamos DoTween para ir a negro y cuando termine (.OnComplete) cambiamos de escena
                cortinaNegra.DOFade(1f, duracionFade).OnComplete(() =>
                {
                    SceneManager.LoadScene(siguienteNivel);
                });
            }
            else
            {
                // Si se nos olvidó poner la imagen, cargamos directo para que funcione igual
                SceneManager.LoadScene(siguienteNivel);
            }
        }
    }
}