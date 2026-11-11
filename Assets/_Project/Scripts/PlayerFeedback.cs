using UnityEngine;

// Version 2: Sound + Partikel + Post Processing + Kamerawackeln.
// Ersetzt Version 1 komplett (die Audio-Felder im Inspector bleiben erhalten).
[RequireComponent(typeof(PlayerController))]
public class PlayerFeedback : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip[] jumpSfx;          // 2 bis 4 Varianten, Size im Inspector setzen
    public AudioClip landSfx;
    public AudioClip splashSfx;
    public AudioClip hitSfx;
    public AudioClip coinSfx;

    [Header("VFX (Partikel-Prefabs)")]
    public GameObject splashPrefab;
    public GameObject dustPrefab;
    public GameObject hitPrefab;
    public Transform feet;

    [Header("Kamera")]
    public CameraFollow cameraFollow;

    PlayerController player;

    void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    void Start()
    {
        player.Jumped += OnJumped;
        player.Landed += OnLanded;
        player.Died += OnDied;
        GameManager.Instance.CoinCollected += OnCoin;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.CoinCollected -= OnCoin;
    }

    void OnJumped()
    {
        Play(jumpSfx);
    }

    void OnLanded()
    {
        Play(landSfx, 0.6f);
        if (feet != null)
        {
            Vector3 pos = feet.position + Vector3.right * 0.35f;
            Spawn(dustPrefab, pos, Quaternion.Euler(-90f, 0f, 0f));
        }
    }

    void OnCoin()
    {
        Play(coinSfx);
        if (PostFXController.Instance != null) PostFXController.Instance.CoinPulse();
    }

    void OnDied(DeathCause cause)
    {
        if (cause == DeathCause.Water)
        {
            Play(splashSfx);
            Vector3 surface = new Vector3(transform.position.x, GameManager.Instance.waterLevel + 0.3f, 0f);
            Spawn(splashPrefab, surface, Quaternion.Euler(-90f, 0f, 0f));
        }
        else
        {
            Play(hitSfx);
            Spawn(hitPrefab, transform.position, Quaternion.identity);
        }

        if (PostFXController.Instance != null) PostFXController.Instance.HitPulse();
        if (cameraFollow != null) cameraFollow.Shake(0.35f, 0.3f);
    }

    void Play(AudioClip clip, float volume = 1f)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(clip, volume);
    }

    void Play(AudioClip[] clips, float volume = 1f)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(clips, volume);
    }

    void Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab != null) Instantiate(prefab, position, rotation);
    }

    

}
