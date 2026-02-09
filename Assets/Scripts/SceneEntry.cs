using UnityEngine;
using UnityEngine.UI; // Necesario para la UI
using DG.Tweening; // Necesario para DOTween

public class SceneEntry : MonoBehaviour
{
    [Header("Configuración")]
    public float duracionFade = 1f;

    private Image imagenCortina;

    void Awake()
    {
        // 1. Cogemos la referencia de la propia imagen
        imagenCortina = GetComponent<Image>();

        // 2. Nos aseguramos de que empiece NEGRA TOTAL antes de que se vea nada
        if (imagenCortina != null)
        {
            Color colorInicial = imagenCortina.color;
            colorInicial.a = 1f; // Alpha 1 = Opaco
            imagenCortina.color = colorInicial;
            imagenCortina.gameObject.SetActive(true);
        }
    }

    void Start()
    {
        // 3. Nada más arrancar el nivel, hacemos el fade hacia transparente (0)
        if (imagenCortina != null)
        {
            // Fade a 0 (transparente)
            imagenCortina.DOFade(0f, duracionFade).OnComplete(() =>
            {
                // Opcional: Al terminar, desactivamos el objeto para que no gaste recursos
                // ni bloquee clicks del ratón por error.
                imagenCortina.gameObject.SetActive(false);
            });
        }
    }
}