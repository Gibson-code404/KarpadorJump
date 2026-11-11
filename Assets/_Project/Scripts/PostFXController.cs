using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Veraendert Post-Processing-Effekte zur Laufzeit (dynamisch):
//  - Treffer / Tod: rote Vignette + Chromatic Aberration
//  - Muenze: kurzer Bloom-Blitz
//  - Schwierigkeit: Vignette wird mit der Zeit staerker
public class PostFXController : MonoBehaviour
{
    public static PostFXController Instance { get; private set; }

    public Volume volume;

    Vignette vignette;
    ChromaticAberration chroma;
    Bloom bloom;

    float vignetteBase;
    float chromaBase;
    float bloomBase;

    float hit;    // 0..1, faellt von selbst auf 0
    float coin;   // 0..1, faellt von selbst auf 0

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (volume == null) volume = FindFirstObjectByType<Volume>();

        // volume.profile ist eine Kopie. Das Asset im Projekt bleibt unveraendert.
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out chroma);
        volume.profile.TryGet(out bloom);

        if (vignette != null) vignetteBase = vignette.intensity.value;
        if (chroma != null) chromaBase = chroma.intensity.value;
        if (bloom != null) bloomBase = bloom.intensity.value;
    }

    public void HitPulse() { hit = 1f; }
    public void CoinPulse() { coin = 1f; }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;
        hit = Mathf.MoveTowards(hit, 0f, dt * 1.2f);
        coin = Mathf.MoveTowards(coin, 0f, dt * 3f);

        float difficulty = GameManager.Instance != null ? GameManager.Instance.Difficulty : 0f;

        if (vignette != null)
        {
            vignette.intensity.Override(Mathf.Clamp01(vignetteBase + difficulty * 0.6f + hit * 0.4f));
            vignette.color.Override(Color.Lerp(Color.black, Color.red, hit));
        }
        if (chroma != null)
        {
            chroma.intensity.Override(Mathf.Clamp01(chromaBase + hit * 0.8f));
        }
        if (bloom != null)
        {
            bloom.intensity.Override(bloomBase + coin * 3f);
        }
    }
}
