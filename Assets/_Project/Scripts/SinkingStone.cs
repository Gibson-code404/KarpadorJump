using UnityEngine;

// Stein sinkt kurz nachdem der Fisch darauf gelandet ist.
// Bewegt wird ein kinematischer Rigidbody2D mit MovePosition (wie bei Plattformen und Aufzuegen).
[RequireComponent(typeof(Rigidbody2D))]
public class SinkingStone : MonoBehaviour
{
    public float delay = 0.5f;
    public float sinkSpeed = 3f;

    Rigidbody2D rb;
    bool triggered;
    float timer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggered || !collision.collider.CompareTag("Player")) return;

        // nur wenn der Fisch von oben kommt, nicht von der Seite
        if (collision.transform.position.y > transform.position.y + 0.3f)
        {
            triggered = true;
            timer = delay;
        }
    }

    void FixedUpdate()
    {
        if (!triggered) return;

        timer -= Time.fixedDeltaTime;
        if (timer <= 0f)
            rb.MovePosition(rb.position + Vector2.down * sinkSpeed * Time.fixedDeltaTime);
    }
}
