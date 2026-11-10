using TMPro;
using UnityEngine;

// World Space UI
// Er schwebt nach oben, wird durchsichtig und loescht sich selbst.
public class FloatingText : MonoBehaviour
{
    public TMP_Text label;
    public CanvasGroup group;
    public float riseSpeed = 1.5f;
    public float lifeTime = 0.8f;

    float age;

    public void SetText(string text)
    {
        if (label != null) label.text = text;
    }

    void Update()
    {
        age += Time.deltaTime;
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        if (group != null) group.alpha = 1f - age / lifeTime;
        if (age >= lifeTime) Destroy(gameObject);
    }
}
