using System.Collections;
using UnityEngine;

// Startet in Abstaenden zufaellige Events. Jedes Event ist eine Coroutine.
public class EventDirector : MonoBehaviour
{
    [Header("Referenzen")]
    public Transform player;
    public StoneSpawner spawner;
    public WaterController water;          // optional
    public GameUI ui;                      // optional: zeigt den Event-Namen als Banner

    [Header("Prefabs")]
    public GameObject updraftPrefab;
    public GameObject hazardPrefab;

    [Header("Timing")]
    public float firstEventDelay = 20f;
    public Vector2 intervalRange = new Vector2(12f, 20f);

    // Die UI hoert hier zu und zeigt den Namen des Events an
    public event System.Action<string> EventStarted;

    float timer;

    void Start()
    {
        timer = firstEventDelay;
        if (ui != null) EventStarted += ui.ShowBanner;
    }

    void OnDestroy()
    {
        if (ui != null) EventStarted -= ui.ShowBanner;
    }

    void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        StartRandomEvent();

        // Mit steigender Schwierigkeit kommen Events haeufiger
        float shrink = Mathf.Lerp(1f, 0.6f, GameManager.Instance.Difficulty);
        timer = Random.Range(intervalRange.x, intervalRange.y) * shrink;
    }

    void StartRandomEvent()
    {
        switch (Random.Range(0, 2))
        {
            case 0: StartCoroutine(UpdraftEvent()); break;
            case 1: StartCoroutine(SinkingEvent()); break;
            default: StartCoroutine(StormEvent()); break;
        }
    }

    // Event 1: Aufwind-Zonen (Area Effector 2D)
    IEnumerator UpdraftEvent()
    {
        EventStarted?.Invoke("Aufwind!");
        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = new Vector3(player.position.x + 20f, 2f, 0f);
            Instantiate(updraftPrefab, pos, Quaternion.identity);
            yield return new WaitForSeconds(2.5f);
        }
    }

    // Event 2: viele Steine sinken (kinematischer Rigidbody)
    IEnumerator SinkingEvent()
    {
        EventStarted?.Invoke("Wackelsteine!");
        float before = spawner.sinkChance;
        spawner.sinkChance = 0.6f;
        yield return new WaitForSeconds(12f);
        spawner.sinkChance = before;
    }

    // Event 3: Sturm. Wasser wird dunkel (Shader), Steine fallen vom Himmel
    IEnumerator StormEvent()
    {
        EventStarted?.Invoke("Sturm!");
        if (water != null) water.SetStorm(1f);

        for (int i = 0; i < 5; i++)
        {
            float fallTime = 1.3f;    // so lange faellt ein Stein aus 9 m Hoehe
            float x = player.position.x + GameManager.Instance.CurrentSpeed * fallTime + Random.Range(-2f, 2f);
            Instantiate(hazardPrefab, new Vector3(x, 9f, 0f), Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(1.2f, 2f));
        }

        if (water != null) water.SetStorm(0f);
    }
}
