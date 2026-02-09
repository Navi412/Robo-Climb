using UnityEngine;

public class MusicaContinua : MonoBehaviour
{
    private static MusicaContinua instance;

    void Awake()
    {
        // Singleton: aseguramos que solo exista un reproductor de música
        if (instance == null)
        {
            instance = this;
            // Esto evita que el objeto se destruya al cambiar de escena
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // Si ya existe otro (por ejemplo al recargar nivel), borramos este nuevo para que no se duplique
            Destroy(gameObject); 
        }
    }
}