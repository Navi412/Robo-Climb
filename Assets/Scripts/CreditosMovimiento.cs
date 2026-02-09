using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditosMovimiento : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadSubida = 50f;
    public float alturaFinal = 1500f;
    public string escenaMenu = "MenuPrincipal";

    void Update()
    {
        // Movimiento vertical constante
        transform.Translate(Vector3.up * velocidadSubida * Time.deltaTime);

        // Comprobamos si el texto ha salido de la pantalla
        if (transform.localPosition.y > alturaFinal)
        {
            SceneManager.LoadScene(escenaMenu);
        }
    }
}