using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BotonAnimado : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    [Header("Configuración Hover/Selección")]
    public float escalaHover = 1.1f;
    public float duracionHover = 0.2f;

    [Header("Configuración Click")]
    public float escalaClick = 0.9f;
    public float duracionClick = 0.1f;

    private Vector3 escalaOriginal;

    void Start()
    {
        escalaOriginal = transform.localScale;
    }

    // Detectar raton entrando
    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimarSeleccion();
    }

    // Detectar raton saliendo
    public void OnPointerExit(PointerEventData eventData)
    {
        AnimarDeseleccion();
    }

    // Detectar seleccion con mando/teclado
    public void OnSelect(BaseEventData eventData)
    {
        AnimarSeleccion();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        AnimarDeseleccion();
    }

    // Funciones de animacion con DOTween
    void AnimarSeleccion()
    {
        transform.DOKill();
        transform.DOScale(escalaOriginal * escalaHover, duracionHover).SetEase(Ease.OutBack);
    }

    void AnimarDeseleccion()
    {
        transform.DOKill();
        transform.DOScale(escalaOriginal, duracionHover);
    }

    // Detectar clic
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(escalaOriginal * escalaClick, duracionClick);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(escalaOriginal * escalaHover, duracionClick);
    }

    void OnDisable()
    {
        transform.localScale = escalaOriginal;
    }
}