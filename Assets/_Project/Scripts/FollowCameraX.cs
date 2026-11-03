using UnityEngine;

// Haengt ein Objekt (z. B. das Wasser) an die x-Position der Kamera.
public class FollowCameraX : MonoBehaviour
{
    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        Vector3 p = transform.position;
        p.x = cam.position.x;
        transform.position = p;
    }
}
