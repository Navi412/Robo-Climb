using UnityEngine;
using DG.Tweening; // ¡La magia!

public class PortalTemporal : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoDeVida = 3f; // Durará 3 segundos
    public float velocidadEfecto = 0.5f; // Tiempo que tarda en abrirse/cerrarse

    void Start()
    {
        // 1. AL NACER: Lo hacemos invisible (escala 0)
        transform.localScale = Vector3.zero;

        // 2. EFECTO DE APARICIÓN (Pop up elástico)
        transform.DOScale(Vector3.one, velocidadEfecto).SetEase(Ease.OutBack);

        // 3. Preparamos su muerte para dentro de X segundos
        Invoke("IniciarCierre", tiempoDeVida);
    }

    void IniciarCierre()
    {
        // 4. EFECTO DE DESAPARICIÓN (Se encoge y luego se destruye)
        transform.DOScale(Vector3.zero, velocidadEfecto)
            .SetEase(Ease.InBack) // Efecto contrario al aparecer
            .OnComplete(() => 
            {
                Destroy(gameObject); // Cuando termina de encogerse, bye bye
            });
    }
}