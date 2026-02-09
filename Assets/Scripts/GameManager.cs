using UnityEngine;
using TMPro; // Necesario para el texto
using UnityEngine.SceneManagement; // Necesario para detectar cambio de nivel

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Configuración General")]
    public int monedasTotales = 0;

    // --- NUEVO: El Altavoz Central para evitar LAG ---
    public AudioSource audioSourceEfectos;

    // Variable para guardar el Checkpoint (del paso 1)
    public Vector3 ultimoPuntoRespawn;

    void Awake()
    {
        // PATRÓN SINGLETON
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ActualizarTextoUI();

        // Lógica de seguridad para el Respawn al cambiar de nivel
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && ultimoPuntoRespawn == Vector3.zero)
        {
            ultimoPuntoRespawn = player.transform.position;
        }
    }

    // --- SECCIÓN DE AUDIO (NUEVO) ---
    // Esta función elimina el lag porque usa un AudioSource que ya existe en memoria
    public void ReproducirSonido(AudioClip clip, float volumen)
    {
        if (clip != null && audioSourceEfectos != null)
        {
            // PlayOneShot permite que suenen varias cosas a la vez (ej: coger 3 monedas rápido)
            audioSourceEfectos.PlayOneShot(clip, volumen);
        }
    }
    // --------------------------------

    // --- SECCIÓN DE MONEDAS ---
    public void SumarMoneda()
    {
        monedasTotales++;
        ActualizarTextoUI();
    }

    public void PerderMonedas()
    {
        monedasTotales = 0;
        ActualizarTextoUI();
    }

    // --- SECCIÓN DE CHECKPOINTS ---
    public void ActualizarCheckpoint(Vector3 nuevaPosicion)
    {
        ultimoPuntoRespawn = nuevaPosicion;
    }

    public Vector3 ObtenerPuntoRespawn()
    {
        return ultimoPuntoRespawn;
    }
    // ------------------------------

    void ActualizarTextoUI()
    {
        GameObject textoObj = GameObject.FindWithTag("TextoMonedas");

        if (textoObj != null)
        {
            TextMeshProUGUI textoTMP = textoObj.GetComponent<TextMeshProUGUI>();
            if (textoTMP != null)
            {
                textoTMP.text = "x " + monedasTotales.ToString();
            }
        }
    }
}