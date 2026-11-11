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
        switch (Random.Range(0, 3))
        {
            case 0: StartCoroutine(UpdraftEvent()); break;
            case 1: StartCoroutine(SinkingEvent()); break;
            case 2: StartCoroutine(StormEvent()); break;
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

        int count = Random.Range(2, 4);          // mal 4, mal 7 Felsen
        for (int i = 0; i < count; i++)
        {
            float spawnY = 7f;                   // knapp über dem Bildrand, sofort sichtbar
            float fallTime = 1.15f;              // Fallzeit aus 7 m bei Gravity Scale 1
            float x;
            if (Random.value < 0.1f)
            {
                // gezielt: dorthin, wo der Fisch gleich ist, leicht gestreut
                x = player.position.x + GameManager.Instance.CurrentSpeed * fallTime + Random.Range(-1.5f, 1.5f);
            }
            else
            {
                // zufällig irgendwo vor dem Fisch
                x = player.position.x + Random.Range(3f, 14f);
            }
            Instantiate(hazardPrefab, new Vector3(x, spawnY, 0f), Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
        }

        if (water != null) water.SetStorm(0f);
    }
}
