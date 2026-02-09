using UnityEngine;
using UnityEngine.UI; 
using DG.Tweening; 

public class SceneEntry : MonoBehaviour
{
    [Header("Configuración")]
    public float duracionFade = 1f;

    private Image imagenCortina;

    void Awake()
    {
        // Pillamos el componente de imagen
        imagenCortina = GetComponent<Image>();

        // La ponemos totalmente opaca (negra) antes de empezar para tapar todo
        if (imagenCortina != null)
        {
            Color colorInicial = imagenCortina.color;
            colorInicial.a = 1f; 
            imagenCortina.color = colorInicial;
            imagenCortina.gameObject.SetActive(true);
        }
    }

    void Start()
    {
        if (imagenCortina != null)
        {
            // Usamos DoTween para bajar el alpha a 0 (transparente)
            imagenCortina.DOFade(0f, duracionFade).OnComplete(() =>
            {
                // Al terminar la animación apagamos el objeto para que no moleste
                imagenCortina.gameObject.SetActive(false);
            });
        }
    }
}