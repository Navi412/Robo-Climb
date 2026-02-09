using UnityEngine;

public class TriggerJefe : MonoBehaviour
{
    public GameObject prefabPortalTemporal; // Arrastra aquí tu prefab del paso 2
    public Transform puntoDeAparicion; // Un objeto vacío donde quieres que salga

    private bool activado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !activado)
        {
            activado = true;
            Debug.Log("💀 ¡Invocando Portal del Boss!");

            // Hacemos aparecer el portal
            Instantiate(prefabPortalTemporal, puntoDeAparicion.position, Quaternion.identity);
            
            // Aquí podrías activar también al Boss...
        }
    }
}