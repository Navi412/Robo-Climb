using UnityEngine;
using UnityEngine.Events;
using System.Collections; // <--- ¡NUEVO! IMPORTANTE PARA QUE FUNCIONE EL RELOJ

public class BotonFisico : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite botonArriba;
    public Sprite botonAbajo;

    [Header("Ajuste Físico")]
    public Vector2 tamanoPresionado; 
    public Vector2 offsetPresionado;

    [Header("Eventos Inmediatos (Confeti/Texto)")]
    public UnityEvent AlPresionar; 

    [Header("Evento con Retraso (Portal)")]
    public float segundosDeRetraso = 6f; // Tiempo a esperar
    public UnityEvent AlTerminarEspera; // <--- Aquí pondremos el portal

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

        // 1. Cambio Físico y Visual
        sr.sprite = botonAbajo;
        col.size = tamanoPresionado;
        col.offset = offsetPresionado;

        // 2. Acción INMEDIATA (Confeti y Texto)
        Debug.Log("🎉 ¡Fiesta!");
        AlPresionar.Invoke();

        // 3. Acción RETRASADA (Empezamos la cuenta atrás)
        StartCoroutine(CuentaAtras());
    }

    // Este es el reloj interno
    IEnumerator CuentaAtras()
    {
        Debug.Log("⏳ Esperando " + segundosDeRetraso + " segundos...");
        yield return new WaitForSeconds(segundosDeRetraso);
        
        Debug.Log("🚪 ¡Portal abierto!");
        AlTerminarEspera.Invoke(); // <--- Aquí se activa el portal
    }
}