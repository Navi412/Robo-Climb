using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Configuración General")]
    public int monedasTotales = 0;

    // AudioSource centralizado para evitar crear objetos temporales
    public AudioSource audioSourceEfectos;

    public Vector3 ultimoPuntoRespawn;

    void Awake()
    {
        // Singleton para persistencia entre escenas
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

        // Inicializamos el respawn si es la primera vez que cargamos
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && ultimoPuntoRespawn == Vector3.zero)
        {
            ultimoPuntoRespawn = player.transform.position;
        }
    }

    public void ReproducirSonido(AudioClip clip, float volumen)
    {
        if (clip != null && audioSourceEfectos != null)
        {
            audioSourceEfectos.PlayOneShot(clip, volumen);
        }
    }

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

    public void ActualizarCheckpoint(Vector3 nuevaPosicion)
    {
        ultimoPuntoRespawn = nuevaPosicion;
    }

    public Vector3 ObtenerPuntoRespawn()
    {
        return ultimoPuntoRespawn;
    }

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