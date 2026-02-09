using UnityEngine;
using DG.Tweening;

public class BouncyBall : MonoBehaviour
{
    [Header("Configuración Física")]
    [Tooltip("Fuerza con la que te escupe")]
    public float fuerzaRebote = 25f;

    [Tooltip("Ayuda al jugador: 0 es física real (difícil), 1 es siempre hacia arriba (fácil). Prueba 0.5.")]
    [Range(0f, 1f)]
    public float asistenciaVertical = 0.5f;

    [Header("Configuración Visual")]
    public float intensidadVisual = 0.5f;
    public float duracionEfecto = 0.2f;

    [Header("Audio")] // <--- NUEVA SECCIÓN
    public AudioSource audioSource;
    public AudioClip sfxRebote;
    [Range(0f, 1f)] public float volumen = 1f;
    [Range(0.5f, 2f)] public float pitch = 1f; // Para hacerlo más agudo o grave

    void Start()
    {
        // Autocompletar si se te olvida ponerlo en el inspector
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Movement player))
        {
            // --- LÓGICA DE AUDIO (NUEVO) ---
            if (audioSource != null && sfxRebote != null)
            {
                audioSource.pitch = pitch;
                // Usamos PlayOneShot para que si rebotas muy rápido no se corte el sonido anterior
                audioSource.PlayOneShot(sfxRebote, volumen);
            }
            // -------------------------------

            // 1. Calculamos la dirección REAL (Física pura)
            Vector2 direccionReal = (collision.transform.position - transform.position).normalized;

            // 2. APLICAMOS LA AYUDA (Magia de diseño)
            Vector2 direccionFinal = Vector2.Lerp(direccionReal, Vector2.up, asistenciaVertical).normalized;

            // 3. Empujamos al jugador con la dirección corregida
            player.ReboteDireccional(direccionFinal, fuerzaRebote);

            // 4. Efecto visual
            transform.DOComplete(true);
            transform.DOPunchScale(Vector3.one * intensidadVisual, duracionEfecto, 10, 1);
        }
    }
}