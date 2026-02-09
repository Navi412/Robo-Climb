using UnityEngine;
using UnityEngine.EventSystems; // Necesario para detectar ratón y mando
using DG.Tweening; // Necesario para las animaciones

public class BotonAnimado : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    [Header("Configuración Hover/Selección")]
    public float escalaHover = 1.1f; // Tamaño al seleccionar
    public float duracionHover = 0.2f;

    [Header("Configuración Click")]
    public float escalaClick = 0.9f; // Tamaño al pulsar
    public float duracionClick = 0.1f;

    private Vector3 escalaOriginal;

    void Start()
    {
        // Guardamos el tamaño original al iniciar
        escalaOriginal = transform.localScale;
    }

    // --- EVENTOS DE RATÓN ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimarSeleccion();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimarDeseleccion();
    }

    // --- EVENTOS DE MANDO / TECLADO (ISelectHandler) ---
    // Se ejecuta automáticamente cuando el EventSystem selecciona este botón
    public void OnSelect(BaseEventData eventData)
    {
        AnimarSeleccion();
    }

    // Se ejecuta cuando el foco se va a otro botón
    public void OnDeselect(BaseEventData eventData)
    {
        AnimarDeseleccion();
    }

    // --- LÓGICA DE ANIMACIÓN ---
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

    // --- CLICS ---
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