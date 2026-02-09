using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class BotonFisico : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite botonArriba;
    public Sprite botonAbajo;

    [Header("Ajuste Físico")]
    public Vector2 tamanoPresionado;
    public Vector2 offsetPresionado;

    [Header("Eventos Inmediatos")]
    public UnityEvent AlPresionar;

    [Header("Evento con Retraso")]
    public float segundosDeRetraso = 6f;
    public UnityEvent AlTerminarEspera;

    private SpriteRenderer sr;
    private BoxCollider2D col;
    private bool estaPulsado = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        sr.sprite = botonArriba;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (estaPulsado) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contacto in collision.contacts)
            {
                // Solo se activa si saltamos encima
                if (contacto.normal.y < -0.5f)
                {
                    Presionar();
                    break;
                }
            }
        }
    }

    void Presionar()
    {
        estaPulsado = true;

        // Cambio de sprite y collider
        sr.sprite = botonAbajo;
        col.size = tamanoPresionado;
        col.offset = offsetPresionado;

        AlPresionar.Invoke();

        StartCoroutine(CuentaAtras());
    }

    IEnumerator CuentaAtras()
    {
        yield return new WaitForSeconds(segundosDeRetraso);
        AlTerminarEspera.Invoke();
    }
}