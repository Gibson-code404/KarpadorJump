using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float offsetX = 4f;      // Kamera schaut etwas vor den Fisch
    public float fixedY = 1.5f;     // Hoehe bleibt konstant
    public float smoothTime = 0.15f;

    float currentX;
    float velocityX;
    float shakeTime;
    float shakeStrength;

    void Start()
    {
        currentX = transform.position.x;
    }

    // Kameras immer in LateUpdate bewegen: dann hat sich der Fisch schon bewegt
    void LateUpdate()
    {
        if (target == null) return;

        float goalX = target.position.x + offsetX;
        currentX = Mathf.SmoothDamp(currentX, goalX, ref velocityX, smoothTime);

        Vector3 pos = new Vector3(currentX, fixedY, transform.position.z);

        if (shakeTime > 0f)
        {
            shakeTime -= Time.deltaTime;
            pos += (Vector3)(Random.insideUnitCircle * shakeStrength);
        }

        transform.position = pos;
    }

    public void Shake(float duration, float strength)
    {
        shakeTime = duration;
        shakeStrength = strength;
    }
}
