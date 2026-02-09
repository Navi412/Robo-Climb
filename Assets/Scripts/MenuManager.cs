using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems; // Necesario para el mando
using System.Collections; // Necesario para la correcci�n del "Click inicial"

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [Header("Referencias UI")]
    public GameObject canvasDelMenu;
    public GameObject contenedorMenuPrincipal;
    public GameObject panelConfiguracion;

    [Header("Textos Din�micos")]
    public TextMeshProUGUI textoBotonJugar;

    [Header("Navegaci�n Mando (ARRASTRAR AQU�)")]
    public GameObject primerBotonMenu;     // Arrastra el Btn_Jugar
    public GameObject primerBotonOpciones; // Arrastra el Slider de Volumen o el bot�n de Cerrar Opciones

    private bool juegoPausado = false;

    void Awake()
    {
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
        // Forzamos la selecci�n al arrancar el juego con un peque�o retraso
        // para asegurar que Unity ha cargado todo el sistema de Input
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartCoroutine(SeleccionarBotonConRetraso(primerBotonMenu));
        }
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            MostrarMenuPrincipal();
        }
        else
        {
            juegoPausado = false;
            canvasDelMenu.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            // ESC o START (Mando)
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton9))
            {
                if (juegoPausado) Reanudar();
                else Pausar();
            }
        }
    }

    // --- L�GICA ---

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

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Seleccionar bot�n Reanudar
        StartCoroutine(SeleccionarBotonConRetraso(primerBotonMenu));
    }

    public void Reanudar()
    {
        juegoPausado = false;
        canvasDelMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Limpiamos la selecci�n para que no se quede nada resaltado invisiblemente
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SalirDelJuego()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
        else
        {
            Debug.Log("Saliendo...");
            Application.Quit();
        }
    }

    public void MostrarConfiguracion(bool mostrar)
    {
        panelConfiguracion.SetActive(mostrar);
        contenedorMenuPrincipal.SetActive(!mostrar);

        if (mostrar)
        {
            // Si entramos a opciones -> Seleccionar Slider o Bot�n de opciones
            StartCoroutine(SeleccionarBotonConRetraso(primerBotonOpciones));
        }
        else
        {
            // Si volvemos al men� -> Seleccionar Jugar/Reanudar
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

    // --- CORUTINA M�GICA ---
    // Esta funci�n espera un frame antes de seleccionar el bot�n.
    // Es vital para corregir el fallo de "tengo que hacer clic primero".
    IEnumerator SeleccionarBotonConRetraso(GameObject boton)
    {
        if (boton != null)
        {
            // Esperamos al final del frame para asegurarnos que la UI ya est� activa
            yield return null;

            EventSystem.current.SetSelectedGameObject(null); // Limpiar
            EventSystem.current.SetSelectedGameObject(boton); // Seleccionar
        }
    }

    public void CambiarVolumen(float valor)
    {
        AudioListener.volume = valor;
    }
}