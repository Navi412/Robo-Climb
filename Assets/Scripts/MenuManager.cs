using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems; 
using System.Collections; 

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [Header("Referencias UI")]
    public GameObject canvasDelMenu;
    public GameObject contenedorMenuPrincipal;
    public GameObject panelConfiguracion;

    [Header("Textos Dinámicos")]
    public TextMeshProUGUI textoBotonJugar;

    [Header("Navegación Mando (ARRASTRAR AQUÍ)")]
    public GameObject primerBotonMenu;     // El botón de Jugar
    public GameObject primerBotonOpciones; // El slider o botón de cerrar opciones

    private bool juegoPausado = false;

    void Awake()
    {
        // Singleton: aseguramos que no se destruya al cambiar de escena y solo haya uno
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

    void Start()
    {
        // Si empezamos en el menú, seleccionamos el botón a la fuerza para que el mando funcione
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartCoroutine(SeleccionarBotonConRetraso(primerBotonMenu));
        }
    }

    // Nos suscribimos a los eventos de carga de escena
    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si cargamos el menú (índice 0)
        if (scene.buildIndex == 0)
        {
            MostrarMenuPrincipal();
        }
        else
        {
            // Si es el juego, ocultamos todo y bloqueamos el ratón
            juegoPausado = false;
            canvasDelMenu.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // Si no estamos en el menú principal (estamos jugando)
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            // Botón pausa (Escape o Start en mando)
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton9))
            {
                if (juegoPausado) Reanudar();
                else Pausar();
            }
        }
    }

    // --- FUNCIONES DE CONTROL ---

    public void CargarJuego()
    {
        if (juegoPausado) Reanudar();
        else SceneManager.LoadScene(1);
    }

    public void Pausar()
    {
        juegoPausado = true;
        canvasDelMenu.SetActive(true);
        contenedorMenuPrincipal.SetActive(true);
        panelConfiguracion.SetActive(false);

        if (textoBotonJugar != null) textoBotonJugar.text = "Reanudar";

        // Paramos el tiempo y mostramos el cursor
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Marcamos el botón para poder navegar con mando en el menú de pausa
        StartCoroutine(SeleccionarBotonConRetraso(primerBotonMenu));
    }

    public void Reanudar()
    {
        juegoPausado = false;
        canvasDelMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Limpiamos la selección de la UI
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SalirDelJuego()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            // Si estamos ingame, volvemos al menú
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
        else
        {
            // Si ya estamos en el menú, cerramos la aplicación
            Application.Quit();
        }
    }

    public void MostrarConfiguracion(bool mostrar)
    {
        panelConfiguracion.SetActive(mostrar);
        contenedorMenuPrincipal.SetActive(!mostrar);

        if (mostrar)
        {
            // Al entrar en opciones, seleccionamos el primer control de ese panel
            StartCoroutine(SeleccionarBotonConRetraso(primerBotonOpciones));
        }
        else
        {
            // Al volver, seleccionamos de nuevo el botón de Jugar
            StartCoroutine(SeleccionarBotonConRetraso(primerBotonMenu));
        }
    }

    void MostrarMenuPrincipal()
    {
        canvasDelMenu.SetActive(true);
        contenedorMenuPrincipal.SetActive(true);
        panelConfiguracion.SetActive(false);
        juegoPausado = false;

        if (textoBotonJugar != null) textoBotonJugar.text = "Jugar";

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(SeleccionarBotonConRetraso(primerBotonMenu));
    }

    // Corutina para arreglar el bug de Unity donde el botón no se selecciona si no esperas un frame
    IEnumerator SeleccionarBotonConRetraso(GameObject boton)
    {
        if (boton != null)
        {
            yield return null; // Esperamos un frame

            EventSystem.current.SetSelectedGameObject(null); 
            EventSystem.current.SetSelectedGameObject(boton); 
        }
    }

    public void CambiarVolumen(float valor)
    {
        AudioListener.volume = valor;
    }
}