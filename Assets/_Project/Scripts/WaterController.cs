using UnityEngine;
using UnityEngine.Rendering;

// Steuert Parameter des Wasser-Shaders (z. B. dunkler bei Sturm).
[RequireComponent(typeof(SpriteRenderer))]
public class WaterController : MonoBehaviour
{
    static readonly int StormId = Shader.PropertyToID("_StormAmount");

    public float fadeSpeed = 0.5f;
    public Volume stormVolume;   // optional: zweites Volume, dessen Gewicht mit dem Sturm steigt

    SpriteRenderer sr;
    float current;
    float target;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetStorm(float value)
    {
        target = Mathf.Clamp01(value);
    }

    void Update()
    {
        current = Mathf.MoveTowards(current, target, fadeSpeed * Time.deltaTime);
        sr.material.SetFloat(StormId, current);
        if (stormVolume != null) stormVolume.weight = current;   // Vorlesung 7: Volume.weight per Script
    }
}
