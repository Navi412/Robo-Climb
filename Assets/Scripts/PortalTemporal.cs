using UnityEngine;
using DG.Tweening; // Librería para las interpolaciones

public class PortalTemporal : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoDeVida = 3f; // Cuánto dura abierto
    public float velocidadEfecto = 0.5f; // Lo que tarda la animación

    void Start()
    {
        // Empezamos con tamaño 0 para que no se vea
        transform.localScale = Vector3.zero;

        // Lo hacemos crecer con efecto elástico (OutBack)
        transform.DOScale(Vector3.one, velocidadEfecto).SetEase(Ease.OutBack);

        // Programamos el cierre para dentro de un rato
        Invoke("IniciarCierre", tiempoDeVida);
    }

    void IniciarCierre()
    {
        // Lo encogemos de vuelta a 0
        transform.DOScale(Vector3.zero, velocidadEfecto)
            .SetEase(Ease.InBack) // Efecto contrario (InBack) para que quede bien
            .OnComplete(() => 
            {
                Destroy(gameObject); // Al terminar, borramos el objeto
            });
    }
}