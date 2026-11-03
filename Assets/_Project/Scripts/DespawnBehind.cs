using UnityEngine;

public class DespawnBehind : MonoBehaviour
{
    public float distance = 25f;
    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        if (cam.position.x - transform.position.x > distance) Destroy(gameObject);
    }
}
