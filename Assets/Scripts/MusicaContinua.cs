using UnityEngine;

public class MusicaContinua : MonoBehaviour
{
    private static MusicaContinua instance;

    void Awake()
    {
        // EL PATRÓN "SINGLETON" (Solo puede haber uno)
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // <--- ¡LA MAGIA! No te destruyas al cambiar de escena
        }
        else
        {
            // Si ya existe un DJ (porque venimos del menú y volvimos a cargar),
            // borramos a este impostor para que no suenen dos canciones a la vez.
            Destroy(gameObject); 
        }
    }
}