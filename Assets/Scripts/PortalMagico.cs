using UnityEngine;

public class PortalMagico : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadCrecer = 3f; 
    public float velocidadGirar = 50f; 
    
    private Vector3 escalaFinal;

    void Awake()
    {
        // Guardamos la escala original del objeto para saber hasta dónde crecer
        escalaFinal = transform.localScale;
    }

    void OnEnable()
    {
        // Al activarse lo ponemos a tamaño 0 para que empiece la animación
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        // Usamos Lerp para que crezca de forma suave hacia su tamaño final
        transform.localScale = Vector3.Lerp(transform.localScale, escalaFinal, Time.deltaTime * velocidadCrecer);

        // Rotación constante en el eje Z
        transform.Rotate(0, 0, velocidadGirar * Time.deltaTime);
    }
}