using UnityEngine;
using DG.Tweening;

public class BouncyBall : MonoBehaviour
{
    [Header("Configuración Física")]
    public float fuerzaRebote = 25f;

    [Range(0f, 1f)]
    public float asistenciaVertical = 0.5f;

    [Header("Configuración Visual")]
    public float intensidadVisual = 0.5f;
    public float duracionEfecto = 0.2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sfxRebote;
    [Range(0f, 1f)] public float volumen = 1f;
    [Range(0.5f, 2f)] public float pitch = 1f;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Movement player))
        {
            if (audioSource != null && sfxRebote != null)
            {
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(sfxRebote, volumen);
            }

            // Calculamos direccion normal y la ajustamos para ayudar al salto
            Vector2 direccionReal = (collision.transform.position - transform.position).normalized;
            Vector2 direccionFinal = Vector2.Lerp(direccionReal, Vector2.up, asistenciaVertical).normalized;

            player.ReboteDireccional(direccionFinal, fuerzaRebote);

            transform.DOComplete(true);
            transform.DOPunchScale(Vector3.one * intensidadVisual, duracionEfecto, 10, 1);
        }
    }
}