using UnityEngine;

// Version 1: nur Sound. Reagiert auf die Events des PlayerControllers.
[RequireComponent(typeof(PlayerController))]
public class PlayerFeedback : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip[] jumpSfx;          // 2 bis 4 Varianten, Size im Inspector setzen
    public AudioClip landSfx;
    public AudioClip splashSfx;
    public AudioClip hitSfx;
    public AudioClip coinSfx;

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

    void OnJumped() { Play(jumpSfx); }
    void OnLanded() { Play(landSfx, 0.4f); }
    void OnCoin() { Play(coinSfx, 0.4f); }

    void OnDied(DeathCause cause)
    {
        Play(cause == DeathCause.Water ? splashSfx : hitSfx);
    }

    void Play(AudioClip clip, float volume = 1f)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(clip, volume);
    }

    void Play(AudioClip[] clips, float volume = 1f)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(clips, volume);
    }
}
