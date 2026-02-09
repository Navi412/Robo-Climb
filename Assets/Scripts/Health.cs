using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public float vidaMaxima = 5f;
    public float vidaActual;

    [Header("Configuración UI")]
    public Image barraRelleno;
    public Gradient gradienteVida;
    public float velocidadBarra = 0.5f;

    [Header("Efectos de Muerte")]
    public GameObject explosionPrefab;
    public float tiempoEsperaMuerte = 1f;

    // Variables estaticas para mantener el checkpoint al recargar escena
    private static Vector3 respawnPointPermanente;
    private static string nombreNivelCheckpoint;
    private static bool hayCheckpointActivado = false;

    private bool canTakeDamage = true;
    private bool isDead = false;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Collider2D coll;
    private MonoBehaviour movementScript;

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        movementScript = GetComponent<Movement>();

        vidaActual = vidaMaxima;

        string nombreEscenaActual = SceneManager.GetActiveScene().name;

        // Si tenemos un checkpoint valido en este nivel, nos movemos alli
        if (hayCheckpointActivado && nombreNivelCheckpoint == nombreEscenaActual)
        {
            transform.position = respawnPointPermanente;
        }
        else
        {
            respawnPointPermanente = transform.position;
            nombreNivelCheckpoint = nombreEscenaActual;
            hayCheckpointActivado = true;
        }

        ActualizarBarra(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trampa"))
        {
            transform.position = respawnPointPermanente;
            TakeDamage(1f);
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
    }

    public void TakeDamage(float damage)
    {
        if (!canTakeDamage || isDead) return;

        vidaActual -= damage;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
        ActualizarBarra(false);

        if (vidaActual <= 0) StartCoroutine(MorirConEstilo());
        else StartCoroutine(Invulnerability());
    }

    void ActualizarBarra(bool inmediato)
    {
        if (barraRelleno == null) return;
        float porcentaje = vidaActual / vidaMaxima;
        barraRelleno.color = gradienteVida.Evaluate(porcentaje);
        if (inmediato) barraRelleno.fillAmount = porcentaje;
        else barraRelleno.DOFillAmount(porcentaje, velocidadBarra);
    }

    IEnumerator MorirConEstilo()
    {
        isDead = true;
        if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        if (sr != null) sr.enabled = false;
        if (rb != null) { rb.linearVelocity = Vector2.zero; rb.bodyType = RigidbodyType2D.Kinematic; }
        if (coll != null) coll.enabled = false;
        if (movementScript != null) movementScript.enabled = false;

        yield return new WaitForSeconds(tiempoEsperaMuerte);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator Invulnerability()
    {
        canTakeDamage = false;
        for (int i = 0; i < 5; i++)
        {
            if (sr != null) sr.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.1f);
            if (sr != null) sr.color = new Color(1, 1, 1, 1f);
            yield return new WaitForSeconds(0.1f);
        }
        canTakeDamage = true;
    }

    public void SetRespawnPoint(Vector3 newPosition)
    {
        respawnPointPermanente = newPosition;
        nombreNivelCheckpoint = SceneManager.GetActiveScene().name;
        hayCheckpointActivado = true;
    }
}