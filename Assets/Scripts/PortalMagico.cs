using UnityEngine;

public class PortalMagico : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadCrecer = 3f; // Cuanto más alto, más rápido aparece
    public float velocidadGirar = 50f; // Si quieres que gire (pon 0 para que no gire)
    
    private Vector3 escalaFinal;

    void Awake()
    {
        // Truco: Guardamos el tamaño que le pusiste en la escena antes de encogerlo
        escalaFinal = transform.localScale;
    }

    void OnEnable()
    {
        // En cuanto el botón me activa... ¡Me vuelvo invisible (tamaño 0)!
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        // 1. EFECTO CRECER (Lerp hace que sea suave, rápido al principio y lento al final)
        transform.localScale = Vector3.Lerp(transform.localScale, escalaFinal, Time.deltaTime * velocidadCrecer);

        // 2. EFECTO GIRAR (Opcional, le da toque místico)
        transform.Rotate(0, 0, velocidadGirar * Time.deltaTime);
    }
}