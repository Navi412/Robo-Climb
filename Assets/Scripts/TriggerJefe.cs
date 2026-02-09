using UnityEngine;

public class TriggerJefe : MonoBehaviour
{
    // Referencias al prefab del portal y el punto de spawn
    public GameObject prefabPortalTemporal;
    public Transform puntoDeAparicion;

    private bool activado = false; // Flag para que no se repita el evento

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si es el player y si no se ha activado antes
        if (collision.CompareTag("Player") && !activado)
        {
            activado = true;

            // Spawneamos el portal en la posición marcada
            Instantiate(prefabPortalTemporal, puntoDeAparicion.position, Quaternion.identity);
        }
    }
}