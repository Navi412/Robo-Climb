using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Movement : MonoBehaviour
{
    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    private AnimationScript anim;
    private GhostTrail ghostTrail;

    [Space]
    [Header("🎮 CONFIGURACIÓN DE CONTROLES")]
    public KeyCode saltarTeclado = KeyCode.Space;
    public KeyCode saltarMando = KeyCode.JoystickButton0;
    public KeyCode dashTeclado = KeyCode.E;
    public KeyCode dashMando = KeyCode.JoystickButton2;
    public KeyCode agarrarTeclado = KeyCode.LeftShift;
    public KeyCode agarrarMandoR1 = KeyCode.JoystickButton5;

    [Space]
    [Header("Stats Movimiento")]
    public float speed = 10;
    public float jumpForce = 50;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;

    [Header("Inercia (Efecto Hielo)")]
    public float acceleration = 60f;
    public float deceleration = 20f;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;

    [Space]
    private bool groundTouch;
    private bool hasDashed;
    public int side = 1;

    [Space]
    [Header("Polish Visual")]
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;

    [Header("Audio Pasos (Bucle)")]
    public AudioSource audioSource; // Este será SOLO para los pasos
    public AudioClip sfxPasos;
    [Range(0f, 1f)] public float volumenPasos = 0.5f;
    [Range(0.1f, 3f)] public float pitchPasos = 1f;

    [Header("Audio Efectos (Salto/Dash)")] // <--- ¡NUEVO!
    public AudioClip sfxSalto;
    [Range(0f, 1f)] public float volumenSalto = 1f;

    public AudioClip sfxDash;
    [Range(0f, 1f)] public float volumenDash = 1f;

    // Canal invisible secundario para que el Stop() de los pasos no corte el Salto
    private AudioSource efectosSource;

    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationScript>();
        ghostTrail = FindFirstObjectByType<GhostTrail>();

        // 1. Configuramos el AudioSource principal (Pasos)
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // 2. Creamos un AudioSource secundario por código para los efectos
        // Así los saltos no se cortan cuando dejamos de caminar
        efectosSource = gameObject.AddComponent<AudioSource>();
        efectosSource.spatialBlend = 0; // 2D (Sonido plano)

        canMove = true;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        // 1. INPUT DE MOVIMIENTO
        Vector2 dir = new Vector2(xRaw, y);

        // 2. LOGICA DE MOVIMIENTO (WALK)
        Walk(dir);

        anim.SetHorizontalMovement(Mathf.Abs(x), y, rb.linearVelocity.y);

        // 3. LOGICA DE AGARRE (WALL GRAB)
        bool agarreTeclado = Input.GetKey(agarrarTeclado);
        bool agarreR1 = Input.GetKey(agarrarMandoR1);
        bool agarreR2 = Input.GetAxisRaw("Grab") > 0.1f;
        bool inputAgarre = agarreTeclado || agarreR1 || agarreR2;

        if (coll.onWall && inputAgarre && canMove)
        {
            if (side != coll.wallSide) anim.Flip(side * -1);
            wallGrab = true;
            wallSlide = false;
        }

        if (!inputAgarre || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<BetterJumping>().enabled = true;
        }

        // --- FISICAS DE AGARRE ---
        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            float speedModifier = y > 0 ? .5f : 1;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, y * (speed * speedModifier));
        }
        else
        {
            rb.gravityScale = 3;
        }

        // --- WALL SLIDE ---
        if (coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround) wallSlide = false;

        // --- SALTO ---
        if (Input.GetKeyDown(saltarTeclado) || Input.GetKeyDown(saltarMando))
        {
            anim.SetTrigger("jump");
            if (coll.onGround) Jump(Vector2.up, false);
            if (coll.onWall && !coll.onGround) WallJump();
        }

        // --- DASH ---
        if ((Input.GetKeyDown(dashTeclado) || Input.GetKeyDown(dashMando)) && !hasDashed)
        {
            if (xRaw != 0 || yRaw != 0) Dash(xRaw, yRaw);
        }

        // --- GROUND TOUCH ---
        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }
        if (!coll.onGround && groundTouch) groundTouch = false;

        // --- DETECCION DE TOPE DE PARED ---
        if (coll.onWall && !coll.onHead && wallGrab)
        {
            if (y > 0.1f)
            {
                wallGrab = false;
                wallSlide = false;
                float pushOut = coll.onRightWall ? 1f : -1f;
                rb.linearVelocity = new Vector2(pushOut * 5f, 10f);
            }
        }

        WallParticle(y);

        // --- GESTIÓN DE AUDIO PASOS ---
        GestionarAudioPasos(dir);

        if (wallGrab || wallSlide || !canMove) return;

        if (xRaw > 0) { side = 1; anim.Flip(side); }
        if (xRaw < 0) { side = -1; anim.Flip(side); }
    }

    void GestionarAudioPasos(Vector2 dir)
    {
        bool estaCaminando = coll.onGround && Mathf.Abs(dir.x) > 0.01f && !wallGrab && !isDashing && canMove;

        if (estaCaminando)
        {
            if (!audioSource.isPlaying || audioSource.clip != sfxPasos)
            {
                audioSource.clip = sfxPasos;
                audioSource.volume = volumenPasos;
                audioSource.pitch = pitchPasos;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            // Solo paramos si lo que suena son los pasos
            // (Gracias al canal secundario, esto NO cortará el salto)
            if (audioSource.isPlaying && audioSource.clip == sfxPasos)
            {
                audioSource.Stop();
            }
        }
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove) return;
        if (wallGrab) return;

        if (!wallJumped)
        {
            float targetSpeed = dir.x * speed;
            float currentSpeed = rb.linearVelocity.x;
            float rate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
            float nextSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.deltaTime);
            rb.linearVelocity = new Vector2(nextSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, (new Vector2(dir.x * speed, rb.linearVelocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void WallSlide()
    {
        if (coll.wallSide != side) anim.Flip(side * -1);
        if (!canMove) return;

        bool pushingWall = false;
        if ((rb.linearVelocity.x > 0 && coll.onRightWall) || (rb.linearVelocity.x < 0 && coll.onLeftWall)) pushingWall = true;

        float push = pushingWall ? 0 : rb.linearVelocity.x;
        rb.linearVelocity = new Vector2(push, -slideSpeed);
    }

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;
        side = anim.sr.flipX ? -1 : 1;
    }

    private void Dash(float x, float y)
    {
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        if (ghostTrail != null) ghostTrail.ShowGhost();

        hasDashed = true;
        anim.SetTrigger("dash");

        // --- SONIDO DASH (NUEVO) ---
        if (efectosSource != null && sfxDash != null)
        {
            efectosSource.PlayOneShot(sfxDash, volumenDash);
        }
        // ---------------------------

        rb.linearVelocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);
        rb.linearVelocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());
        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);
        rb.gravityScale = 0;
        GetComponent<BetterJumping>().enabled = false;
        wallJumped = true;
        isDashing = true;
        yield return new WaitForSeconds(.3f);
        rb.gravityScale = 3;
        GetComponent<BetterJumping>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround) hasDashed = false;
    }

    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            anim.Flip(side);
        }
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));
        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;
        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);
        wallJumped = true;
    }

    private void Jump(Vector2 dir, bool wall)
    {
        // --- SONIDO SALTO (NUEVO) ---
        if (efectosSource != null && sfxSalto != null)
        {
            efectosSource.PlayOneShot(sfxSalto, volumenSalto);
        }
        // ----------------------------

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += dir * jumpForce;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void RigidbodyDrag(float x) { rb.linearDamping = x; }
    void WallParticle(float vertical) { }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            foreach (ContactPoint2D point in collision.contacts)
            {
                if (point.normal.y > 0.5f)
                {
                    hasDashed = false;
                    Jump(Vector2.up, false);
                    return;
                }
            }
        }
    }

    public void ReboteExterno(float fuerzaRebote)
    {
        hasDashed = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += Vector2.up * fuerzaRebote;
        anim.SetTrigger("jump");

        // También ponemos sonido al rebotar en cosas externas (opcional)
        if (efectosSource != null && sfxSalto != null)
            efectosSource.PlayOneShot(sfxSalto, volumenSalto);
    }

    public void ReboteDireccional(Vector2 direccion, float fuerza)
    {
        hasDashed = false;
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = direccion * fuerza;
        anim.SetTrigger("jump");

        // También ponemos sonido al rebotar direccionalmente
        if (efectosSource != null && sfxSalto != null)
            efectosSource.PlayOneShot(sfxSalto, volumenSalto);
    }
}