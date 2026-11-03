using System;
using UnityEngine;

public enum DeathCause { Water, Hazard }

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Sprung")]
    public float jumpVelocity = 11f;
    public float jumpBufferTime = 0.12f;   // Taste kurz VOR der Landung zaehlt noch
    public float coyoteTime = 0.08f;       // Sprung kurz NACH dem Kantenverlassen geht noch

    [Header("Bodencheck")]
    public Transform groundCheck;
    public float groundRadius = 0.15f;
    public LayerMask groundLayer;

    public event Action Jumped;
    public event Action Landed;
    public event Action<DeathCause> Died;

    public bool IsGrounded { get; private set; }

    Rigidbody2D rb;
    float lastJumpPressTime = -10f;
    float lastGroundedTime = -10f;
    bool dead;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Input gehoert in Update, sonst koennen kurze Tastendruecke verschluckt werden
    void Update()
    {
        if (dead || GameManager.Instance.State != GameState.Playing) return;
        if (GameInput.JumpPressed()) lastJumpPressTime = Time.time;
    }

    // Physik gehoert in FixedUpdate
    void FixedUpdate()
    {
        GameManager gm = GameManager.Instance;
        if (dead || gm.State != GameState.Playing) return;

        bool wasGrounded = IsGrounded;
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        if (IsGrounded)
        {
            lastGroundedTime = Time.time;
            if (!wasGrounded && Time.timeSinceLevelLoad > 0.3f) Landed?.Invoke();
        }

        // Hybrid-Controller: x und y setzen wir selbst (snappy), Kollisionen macht die Physik
        Vector2 v = rb.linearVelocity;
        v.x = gm.CurrentSpeed;

        bool wantsJump = Time.time - lastJumpPressTime <= jumpBufferTime;
        bool canJump = Time.time - lastGroundedTime <= coyoteTime;
        if (wantsJump && canJump)
        {
            v.y = jumpVelocity;
            lastJumpPressTime = -10f;
            lastGroundedTime = -10f;
            Jumped?.Invoke();
        }

        rb.linearVelocity = v;

        if (rb.position.y < gm.waterLevel) Die(DeathCause.Water);
    }

    public void Die(DeathCause cause = DeathCause.Water)
    {
        if (dead) return;
        dead = true;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        Died?.Invoke(cause);
        GameManager.Instance.GameOver();
    }

    // Zeigt den Bodencheck im Scene View (Gizmos aus Vorlesung 2)
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
