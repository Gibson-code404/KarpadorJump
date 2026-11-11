using UnityEngine;


public class FallingHazard : MonoBehaviour
{
    public float lifeTime = 8f;
    public LayerMask groundLayer;
    public GameObject breakFxPrefab;
    public AudioClip breakSound;

    void Start()
    {
        float s = Random.Range(0.6f, 1.0f);
        transform.localScale = new Vector3(s, s, 1f);
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Fels ist ins Wasser gefallen
        if (transform.position.y < GameManager.Instance.waterLevel)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Punkt auf dem getroffenen Collider, der dem Fels am nächsten ist
        Vector2 hitPoint = other.ClosestPoint(transform.position);

        if (other.TryGetComponent(out PlayerController player))
        {
            player.Die(DeathCause.Hazard);
            Break(hitPoint);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Break(hitPoint);
        }
    }

    void Break(Vector2 pos)
    {
        if (breakFxPrefab != null)
            Instantiate(breakFxPrefab, pos, Quaternion.identity);
        if (breakSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(breakSound);
        Destroy(gameObject);
    }

}
