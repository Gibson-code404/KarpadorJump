using UnityEngine;

// Fisch im Hauptmenü: springt in Bögen aus dem Wasser, ohne Physik
public class MenuFish : MonoBehaviour
{
    public float jumpHeight = 3f;
    public float jumpDuration = 1.2f;
    public float pauseUnderwater = 0.6f;
    public float minX = -7f;
    public float maxX = 7f;
    public float waterY = -2f;

    SpriteRenderer sr;
    float timer;
    float startX, endX;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startX = Random.Range(minX, maxX);
        NewJump();
    }

    void NewJump()
    {
        timer = 0f;
        startX = transform.position.x;
        float dir = Random.value < 0.5f ? -1f : 1f;
        endX = Mathf.Clamp(startX + dir * Random.Range(3f, 6f), minX, maxX);
        if (Mathf.Abs(endX - startX) < 1f) endX = startX - dir * 4f;   // am Rand umdrehen
        sr.flipX = endX < startX;   // Blickrichtung (je nach Bild evtl. umgekehrt)
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > jumpDuration + pauseUnderwater) { NewJump(); return; }

        float t = Mathf.Clamp01(timer / jumpDuration);
        float x = Mathf.Lerp(startX, endX, t);
        float y = waterY + 4f * jumpHeight * t * (1f - t);   // Parabel
        transform.position = new Vector3(x, y, 0f);

        // Nase zeigt entlang der Flugbahn
        float dx = endX - startX;
        float angle = Mathf.Atan2(4f * jumpHeight * (1f - 2f * t), Mathf.Abs(dx)) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, dx >= 0f ? angle : -angle);
    }
}