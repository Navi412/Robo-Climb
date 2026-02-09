using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Boss : MonoBehaviour
{
    [Header("Configuración UI Boss")]
    public Image barraVida;
    public Gradient gradienteVida;

    [Header("Deterioro Visual")]
    public Sprite[] fasesDeDanio;
    public SpriteRenderer spriteRenderer;

    [Header("Configuración de Movimiento")]
    public float velocidadSeguimiento = 8f;
    public float minX = -8f;
    public float maxX = 8f;

    [Header("Ataques")]
    [Range(0, 100)] public int probabilidadAplastar = 75;
    public float tiempoEnSuelo = 1.2f;
    public float velocidadRetorno = 4f;

    [Header("Estalactitas (Temblor)")]
    public float duracionTemblor = 2.5f;
    public GameObject[] listaEstalactitas;
    public Transform[] puntosAparicion;

    [Header("Vida y Daño")]
    public int vida = 10;
    private int vidaMaxima;

    [Header("Recompensa al Morir")]
    public GameObject portalFinal;

    [Header("Rebote y Empuje")]
    public float fuerzaReboteVertical = 15f;
    public float fuerzaEmpujeHorizontal = 25f;
    public float tiempoAturdimiento = 0.4f;
    public int danioAlJugador = 1;

    [Header("Invulnerabilidad")]
    public Color colorInvulnerable = Color.red;
    private bool esInvulnerable = false;

    [Header("Efectos")]
    public GameObject prefabEfectoSuelo;
    public Transform puntoEfecto;

    private Rigidbody2D rb;
    private Transform jugador;
    private Rigidbody2D rbJugador;
    private MonoBehaviour scriptMovimientoJugador;
    private Vector3 escalaOriginalJugador;
    private float alturaOriginalY;
    private Vector2 posicionObjetivo;
    private Animator anim;
    private bool jugadorEstaAplastado = false;

    // Estados de la maquina de estados del Boss
    enum Estado { Siguiendo, Cayendo, EsperandoSuelo, Temblando, Volviendo }
    Estado estadoActual = Estado.Siguiendo;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        vidaMaxima = vida;
        if (barraVida != null)
        {
            barraVida.fillAmount = 1f;
            barraVida.color = gradienteVida.Evaluate(1f);
            barraVida.gameObject.SetActive(true);
        }

        if (fasesDeDanio.Length > 0)
            spriteRenderer.sprite = fasesDeDanio[0];

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            jugador = playerObj.transform;
            rbJugador = playerObj.GetComponent<Rigidbody2D>();
            scriptMovimientoJugador = playerObj.GetComponent<Movement>();
            escalaOriginalJugador = jugador.localScale;
        }

        alturaOriginalY = transform.position.y;
        posicionObjetivo = rb.position;

        StartCoroutine(RutinaAtaques());
    }

    void Update()
    {
        // Solo sigue al jugador en el eje X
        if (estadoActual == Estado.Siguiendo && jugador != null)
        {
            float objetivoX = Mathf.Clamp(jugador.position.x, minX, maxX);
            posicionObjetivo = new Vector2(objetivoX, alturaOriginalY);
        }
    }

    void FixedUpdate()
    {
        if (estadoActual == Estado.Siguiendo || estadoActual == Estado.Volviendo || estadoActual == Estado.Temblando)
        {
            Vector2 nuevaPos = Vector2.MoveTowards(rb.position, posicionObjetivo, velocidadSeguimiento * Time.fixedDeltaTime);
            rb.MovePosition(nuevaPos);
        }
    }

    void RecibirDanio()
    {
        if (esInvulnerable) return;

        vida--;

        // Actualizar UI de la barra
        if (barraVida != null)
        {
            float porcentaje = (float)vida / vidaMaxima;
            barraVida.DOFillAmount(porcentaje, 0.5f).SetEase(Ease.OutCubic);
            barraVida.DOColor(gradienteVida.Evaluate(porcentaje), 0.5f);
        }

        if (anim != null) anim.SetTrigger("Golpe");

        ActualizarSpriteDeterioro();
        StartCoroutine(RutinaInvulnerabilidad());

        if (vida <= 0)
        {
            // Resetear fisicas del jugador antes de morir
            if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = true;
            if (rbJugador != null) rbJugador.linearVelocity = Vector2.zero;

            LiberarJugador();
            StopAllCoroutines();

            if (barraVida != null) barraVida.gameObject.SetActive(false);

            // Secuencia de muerte
            transform.DOShakePosition(1f, 0.5f);
            transform.DOScale(0f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                ActivarPortalConEstilo();
                Destroy(gameObject);
            });
        }
    }

    void ActivarPortalConEstilo()
    {
        if (portalFinal != null)
        {
            portalFinal.SetActive(true);
            Vector3 escalaFinal = portalFinal.transform.localScale;
            if (escalaFinal == Vector3.zero) escalaFinal = Vector3.one;

            portalFinal.transform.localScale = Vector3.zero;

            // Animacion de entrada del portal
            portalFinal.transform.DOScale(escalaFinal, 1.5f).SetEase(Ease.OutElastic).SetDelay(0.2f);
            portalFinal.transform.DORotate(new Vector3(0, 0, 360), 1.5f, RotateMode.FastBeyond360).SetEase(Ease.OutBack).SetDelay(0.2f);
        }
    }

    void ActualizarSpriteDeterioro()
    {
        if (fasesDeDanio.Length == 0 || spriteRenderer == null) return;
        float porcentajeVidaPerdida = 1f - ((float)vida / vidaMaxima);
        int indice = Mathf.FloorToInt(porcentajeVidaPerdida * fasesDeDanio.Length);
        indice = Mathf.Clamp(indice, 0, fasesDeDanio.Length - 1);
        spriteRenderer.sprite = fasesDeDanio[indice];
    }

    IEnumerator RutinaInvulnerabilidad()
    {
        esInvulnerable = true;
        Color original = Color.white;
        spriteRenderer.color = colorInvulnerable;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = original;
        esInvulnerable = false;
    }

    IEnumerator RutinaAtaques()
    {
        while (vida > 0)
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f));
            // Decide aleatoriamente que ataque usar
            if (Random.Range(0, 101) <= probabilidadAplastar) yield return StartCoroutine(AtaqueAplastar());
            else yield return StartCoroutine(AtaqueLluvia());
        }
    }

    IEnumerator AtaqueAplastar()
    {
        estadoActual = Estado.Cayendo;
        yield return new WaitForSeconds(0.5f);
        if (anim != null) anim.SetBool("Cayendo", true);

        // Cambiamos a Dynamic para que caiga por gravedad
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 10;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;

        yield return new WaitForSeconds(tiempoEnSuelo);
        LiberarJugador();

        // Volvemos a Kinematic para subir
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        estadoActual = Estado.Volviendo;
        posicionObjetivo = new Vector2(rb.position.x, alturaOriginalY);
        while (Vector2.Distance(rb.position, posicionObjetivo) > 0.1f) yield return null;
        estadoActual = Estado.Siguiendo;
    }

    IEnumerator AtaqueLluvia()
    {
        estadoActual = Estado.Temblando;
        Vector2 posBase = rb.position;
        float tiempo = 0;
        float proximaPiedra = 0;

        while (tiempo < duracionTemblor)
        {
            posicionObjetivo = posBase + (Random.insideUnitCircle * 0.2f);
            if (tiempo >= proximaPiedra)
            {
                int i = Random.Range(0, puntosAparicion.Length);
                if (puntosAparicion[i] != null && listaEstalactitas.Length > 0)
                {
                    Instantiate(listaEstalactitas[Random.Range(0, listaEstalactitas.Length)], puntosAparicion[i].position, Quaternion.identity);
                }
                proximaPiedra = tiempo + Random.Range(0.2f, 0.5f);
            }
            tiempo += Time.deltaTime;
            yield return null;
        }
        posicionObjetivo = posBase;
        yield return new WaitForSeconds(0.5f);
        estadoActual = Estado.Siguiendo;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            ContactPoint2D contacto = col.contacts[0];

            // Detectar colision por arriba (cabeza del boss)
            if (contacto.normal.y < -0.7f)
            {
                if (!esInvulnerable) RecibirDanio();

                // Knockback solo si esta vivo
                if (vida > 0 && rbJugador != null)
                {
                    float direccionEmpuje = Mathf.Sign(col.transform.position.x - transform.position.x);
                    StartCoroutine(AplicarKnockback(direccionEmpuje));
                }
            }
            // Si el boss nos cae encima
            else if (contacto.normal.y > 0.7f && estadoActual == Estado.Cayendo)
            {
                AplastarJugador(col.transform);
                Camera.main.transform.DOShakePosition(0.2f, 0.5f);
                DetenerCaidaBoss();
            }
        }

        // Choque contra el suelo
        if (estadoActual == Estado.Cayendo && col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (prefabEfectoSuelo) Instantiate(prefabEfectoSuelo, puntoEfecto.position, Quaternion.identity);
            Camera.main.transform.DOShakePosition(0.3f, 0.8f);
            DetenerCaidaBoss();
        }
    }

    IEnumerator AplicarKnockback(float direccionX)
    {
        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = false;

        if (rbJugador != null)
        {
            rbJugador.linearVelocity = Vector2.zero;
            rbJugador.linearVelocity = new Vector2(direccionX * fuerzaEmpujeHorizontal, fuerzaReboteVertical);
        }

        yield return new WaitForSeconds(tiempoAturdimiento);

        if (scriptMovimientoJugador != null) scriptMovimientoJugador.enabled = true;
    }

    void DetenerCaidaBoss()
    {
        if (anim != null) anim.SetBool("Cayendo", false);
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void AplastarJugador(Transform playerTransform)
    {
        if (jugadorEstaAplastado) return;
        jugadorEstaAplastado = true;
        playerTransform.GetComponent<Health>()?.TakeDamage(danioAlJugador);
        if (rbJugador != null)
        {
            rbJugador.linearVelocity = Vector2.zero;
            rbJugador.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        // Aplastamos el sprite del jugador
        playerTransform.DOKill();
        playerTransform.DOScale(new Vector3(escalaOriginalJugador.x * 1.5f, escalaOriginalJugador.y * 0.2f, 1f), 0.1f);
    }

    void LiberarJugador()
    {
        if (!jugadorEstaAplastado || jugador == null) return;
        if (rbJugador != null) rbJugador.constraints = RigidbodyConstraints2D.FreezeRotation;
        jugador.DOScale(escalaOriginalJugador, 0.2f).SetEase(Ease.OutBack);
        jugadorEstaAplastado = false;
    }
}