using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;
    public float bobHeight = 0.12f;
    public float bobSpeed = 4f;
    public FloatingText popupPrefab;   

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    // Einfache Sin()-Animation
    void Update()
    {
        float y = Mathf.Sin(Time.time * bobSpeed + startPos.x) * bobHeight;
        transform.position = startPos + new Vector3(0f, y, 0f);
    }

    // Trigger-Collider: ruft die Physik auf, ohne echte Kollision
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.Instance.AddCoin(value);
        if (popupPrefab != null)
        {
            FloatingText popup = Instantiate(popupPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            popup.SetText("+" + value * 10);
        }
        Destroy(gameObject);
    }
}
