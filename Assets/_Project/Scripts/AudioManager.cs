using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public const string VolumeKey = "MasterVolume";

    [Header("Musik")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("Quellen")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    void Awake()
    {
        // Singleton mit Doppelgaenger-Schutz: In jeder Szene liegt ein AudioManager,
        // nur der erste bleibt bestehen.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        AudioListener.volume = PlayerPrefs.GetFloat(VolumeKey, 0.8f);   // gespeicherte Lautstaerke
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        if (Instance != this) return;
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    void PlayMusicForScene(string sceneName)
    {
        PlayMusic(sceneName == "MainMenu" ? menuMusic : gameMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;   // nicht neu starten
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // Vorlesung 7: kleine Zufallsaenderungen gegen den "Maschinengewehr-Effekt".
    // Pitch 0.9 bis 1.1, Lautstaerke 80 bis 100 %.
    public void PlaySfx(AudioClip clip, float volume = 1f, bool vary = true)
    {
        if (clip == null) return;
        sfxSource.pitch = vary ? Random.Range(0.9f, 1.1f) : 1f;
        float v = vary ? volume * Random.Range(0.8f, 1f) : volume;
        sfxSource.PlayOneShot(clip, v);   // ueberlappt, schneidet nichts ab
    }

    // Variante mit 2 bis 4 Clips: es wird zufaellig einer gewaehlt
    public void PlaySfx(AudioClip[] clips, float volume = 1f)
    {
        if (clips == null || clips.Length == 0) return;
        PlaySfx(clips[Random.Range(0, clips.Length)], volume);
    }

    // Gesamtlautstaerke 0..1, wird in PlayerPrefs gespeichert
    public static void SetMasterVolume(float value)
    {
        AudioListener.volume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(VolumeKey, AudioListener.volume);
    }
}
