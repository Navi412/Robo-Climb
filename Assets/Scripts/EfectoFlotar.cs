using UnityEngine;

public class EfectoFlotar : MonoBehaviour
{
    [Header("Configuración del Flote")]
    public float velocidad = 3f;
    public float altura = 0.25f;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        // Usamos una onda senoidal para crear el movimiento suave de arriba a abajo
        float nuevoY = Mathf.Sin(Time.time * velocidad) * altura;

        transform.position = new Vector3(posicionInicial.x, posicionInicial.y + nuevoY, posicionInicial.z);
    }
}