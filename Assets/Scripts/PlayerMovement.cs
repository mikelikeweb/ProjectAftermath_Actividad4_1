using UnityEngine;
using UnityEngine.InputSystem;

// =====================================
// PROJECT AFTERMATH
// CONTROLADOR DE PERSONAJE 2D
// Actividad 4.1 - Fisicas y Movimiento
// =====================================

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class PlayerMovement : MonoBehaviour
{
    // =====================================
    // CONFIGURACION DEL PERSONAJE
    // =====================================

    [Header("Movimiento horizontal")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Salto")]
    [SerializeField] private float jumpSpeed = 8f;

    [Header("Deteccion del suelo")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.10f;

    [Header("Mejoras del salto")]
    [SerializeField] private float coyoteTime = 0.10f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Orientacion del personaje")]
    [SerializeField] private bool spriteMiraDerecha = true;

    // =====================================
    // COMPONENTES
    // =====================================

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // =====================================
    // VARIABLES DE MOVIMIENTO
    // =====================================

    private float horizontalInput;

    private bool isGrounded;

    private float coyoteCounter;
    private float jumpBufferCounter;

    private bool cutJumpRequested;

    // =====================================
    // PARAMETROS DEL ANIMATOR
    // =====================================

    private static readonly int IsMovingHash =
        Animator.StringToHash("isMoving");

    private static readonly int IsGroundedHash =
        Animator.StringToHash("isGrounded");

    private static readonly int VerticalSpeedHash =
        Animator.StringToHash("verticalSpeed");

    private bool hasIsMoving;
    private bool hasIsGrounded;
    private bool hasVerticalSpeed;

    // =====================================
    // INICIALIZACION
    // =====================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        boxCollider = GetComponent<BoxCollider2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        animator = GetComponent<Animator>();

        horizontalInput = 0f;

        // Comprobar parametros del Animator
        // para evitar errores por parametros
        // que todavia no existen.

        if (animator != null &&
            animator.runtimeAnimatorController != null)
        {
            hasIsMoving = HasAnimatorParameter(
                "isMoving",
                AnimatorControllerParameterType.Bool
            );

            hasIsGrounded = HasAnimatorParameter(
                "isGrounded",
                AnimatorControllerParameterType.Bool
            );

            hasVerticalSpeed = HasAnimatorParameter(
                "verticalSpeed",
                AnimatorControllerParameterType.Float
            );

            if (!hasIsMoving)
            {
                Debug.LogWarning(
                    "PlayerMovement: Falta el parametro Bool 'isMoving' en Animator.",
                    this
                );
            }
        }
    }

    // =====================================
    // COMPROBACION INICIAL
    // =====================================

    private void Start()
    {
        if (groundLayer.value == 0)
        {
            Debug.LogWarning(
                "PlayerMovement: Debes asignar la capa Ground en el Inspector.",
                this
            );
        }
    }

    // =====================================
    // ACTUALIZACION DEL JUGADOR
    // =====================================

    private void Update()
    {
        ReadMovementInput();

        UpdateFacingDirection();

        UpdateAnimation();
    }

    // =====================================
    // DETECCION DEL TECLADO
    // =====================================

    private void ReadMovementInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            horizontalInput = 0f;
            return;
        }

        // Movimiento hacia la izquierda
        bool left =
            keyboard.aKey.isPressed ||
            keyboard.leftArrowKey.isPressed;

        // Movimiento hacia la derecha
        bool right =
            keyboard.dKey.isPressed ||
            keyboard.rightArrowKey.isPressed;

        // Calcular direccion horizontal:
        // -1 = izquierda
        //  0 = detenido
        //  1 = derecha

        horizontalInput =
            (right ? 1f : 0f) -
            (left ? 1f : 0f);

        // =====================================
        // DETECTAR SALTO
        // =====================================

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            jumpBufferCounter = jumpBufferTime;
        }

        // Detectar cuando se suelta espacio
        // para permitir saltos mas cortos.

        if (keyboard.spaceKey.wasReleasedThisFrame)
        {
            cutJumpRequested = true;
        }
    }

    // =====================================
    // MOVIMIENTO FISICO
    // =====================================

    private void FixedUpdate()
    {
        // Comprobar si estamos tocando el suelo
        isGrounded = CheckGrounded();

        // =====================================
        // COYOTE TIME
        // =====================================

        if (isGrounded &&
            rb.linearVelocity.y <= 0.05f)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter = Mathf.Max(
                0f,
                coyoteCounter - Time.fixedDeltaTime
            );
        }

        // =====================================
        // JUMP BUFFER
        // =====================================

        jumpBufferCounter = Mathf.Max(
            0f,
            jumpBufferCounter - Time.fixedDeltaTime
        );

        // Conservar la velocidad vertical
        // para que la gravedad siga funcionando.

        float verticalVelocity = rb.linearVelocity.y;

        // =====================================
        // EJECUTAR SALTO
        // =====================================

        if (jumpBufferCounter > 0f &&
            coyoteCounter > 0f)
        {
            verticalVelocity = jumpSpeed;

            jumpBufferCounter = 0f;

            coyoteCounter = 0f;

            isGrounded = false;
        }

        // =====================================
        // SALTO DE ALTURA VARIABLE
        // =====================================

        if (cutJumpRequested &&
            verticalVelocity > 0f)
        {
            verticalVelocity *= jumpCutMultiplier;
        }

        cutJumpRequested = false;

        // =====================================
        // APLICAR MOVIMIENTO
        // =====================================

        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            verticalVelocity
        );
    }

    // =====================================
    // DETECCION DEL SUELO
    // =====================================

    private bool CheckGrounded()
    {
        if (groundLayer.value == 0)
        {
            return false;
        }

        Bounds bounds = boxCollider.bounds;

        // Calcular posicion debajo del personaje

        Vector2 checkCenter = new Vector2(
            bounds.center.x,
            bounds.min.y - groundCheckDistance * 0.5f
        );

        // Crear un area de deteccion

        Vector2 checkSize = new Vector2(
            bounds.size.x * 0.75f,
            groundCheckDistance
        );

        // Comprobar si existe suelo

        Collider2D groundHit = Physics2D.OverlapBox(
            checkCenter,
            checkSize,
            0f,
            groundLayer
        );

        return groundHit != null;
    }

    // =====================================
    // ORIENTACION DEL PERSONAJE
    // =====================================

    private void UpdateFacingDirection()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        // Mirar hacia la izquierda

        if (horizontalInput < -0.01f)
        {
            spriteRenderer.flipX = spriteMiraDerecha;
        }

        // Mirar hacia la derecha

        else if (horizontalInput > 0.01f)
        {
            spriteRenderer.flipX = !spriteMiraDerecha;
        }

        // Sin movimiento conserva
        // la ultima orientacion.
    }

    // =====================================
    // CONTROL DE ANIMACIONES
    // =====================================

    private void UpdateAnimation()
    {
        if (animator == null ||
            animator.runtimeAnimatorController == null)
        {
            return;
        }

        // =====================================
        // IDLE / WALK
        // =====================================

        if (hasIsMoving)
        {
            bool isMoving =
                Mathf.Abs(horizontalInput) > 0.01f;

            animator.SetBool(
                IsMovingHash,
                isMoving
            );
        }

        // =====================================
        // DETECCION DEL SUELO
        // FUTURAS ANIMACIONES
        // =====================================

        if (hasIsGrounded)
        {
            animator.SetBool(
                IsGroundedHash,
                isGrounded
            );
        }

        // =====================================
        // VELOCIDAD VERTICAL
        // JUMP / FALL
        // =====================================

        if (hasVerticalSpeed)
        {
            animator.SetFloat(
                VerticalSpeedHash,
                rb.linearVelocity.y
            );
        }
    }

    // =====================================
    // VERIFICAR PARAMETROS DEL ANIMATOR
    // =====================================

    private bool HasAnimatorParameter(
        string parameterName,
        AnimatorControllerParameterType parameterType)
    {
        foreach (
            AnimatorControllerParameter parameter
            in animator.parameters
        )
        {
            if (parameter.name == parameterName &&
                parameter.type == parameterType)
            {
                return true;
            }
        }

        return false;
    }

    // =====================================
    // VISUALIZAR DETECCION DEL SUELO
    // =====================================

    private void OnDrawGizmosSelected()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();

        if (col == null)
        {
            return;
        }

        Bounds bounds = col.bounds;

        Vector3 checkCenter = new Vector3(
            bounds.center.x,
            bounds.min.y - groundCheckDistance * 0.5f,
            transform.position.z
        );

        Vector3 checkSize = new Vector3(
            bounds.size.x * 0.75f,
            groundCheckDistance,
            0f
        );

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(
            checkCenter,
            checkSize
        );
    }

    // =====================================
    // SEGURIDAD DEL PERSONAJE
    // =====================================

    private void OnDisable()
    {
        horizontalInput = 0f;

        jumpBufferCounter = 0f;

        coyoteCounter = 0f;

        cutJumpRequested = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}