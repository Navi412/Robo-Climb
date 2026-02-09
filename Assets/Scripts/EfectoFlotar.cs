using UnityEngine;

public class EfectoFlotar : MonoBehaviour
{
    [Header("Configuración del Flote")]
    public float velocidad = 3f; // Qué tan rápido sube y baja
    public float altura = 0.25f; // Qué tanta distancia recorre

    private Vector3 posicionInicial;

    void Start()
    {
        // Guardamos dónde estaba el objeto al empezar para usarlo de referencia
        posicionInicial = transform.position;
    }

    void Update()
    {
        // Calculamos la nueva altura usando el Seno del tiempo
        // Mathf.Sin crea una onda que va de -1 a 1 suavemente
        float nuevoY = Mathf.Sin(Time.time * velocidad) * altura;

        // Aplicamos la posición: 
        // X y Z se quedan igual, solo sumamos el movimiento a la Y original
        transform.position = new Vector3(posicionInicial.x, posicionInicial.y + nuevoY, posicionInicial.z);
    }
}