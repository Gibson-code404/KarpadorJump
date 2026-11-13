using System.Collections;
using UnityEngine;

// Startet in Abstaenden zufaellige Events. 
public class EventDirector : MonoBehaviour
{
    [Header("Referenzen")]
    public Transform player;
    public StoneSpawner spawner;
    public WaterController water;          
    public GameUI ui;                      

    [Header("Prefabs")]
    public GameObject updraftPrefab;
    public GameObject hazardPrefab;

    [Header("Timing")]
    public float firstEventDelay = 20f;
    public Vector2 intervalRange = new Vector2(12f, 20f);

    
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

    // Event 1: Aufwind-Zonen 
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

    // Event 2: viele Steine sinken 
    IEnumerator SinkingEvent()
    {
        EventStarted?.Invoke("Wackelsteine!");
        float before = spawner.sinkChance;
        spawner.sinkChance = 0.6f;
        yield return new WaitForSeconds(12f);
        spawner.sinkChance = before;
    }

    // Event 3: Sturm. Wasser wird dunkel , Bälle fallen vom Himmel
        IEnumerator StormEvent()
    {
        EventStarted?.Invoke("Catch them all!");
        if (water != null) water.SetStorm(1f);

        int count = Random.Range(2, 4);          // mal 4, mal 7 Felsen
        for (int i = 0; i < count; i++)
        {
            float spawnY = 7f;                   
            float fallTime = 1.15f;              
            float x;
            if (Random.value < 0.1f)
            {
                
                x = player.position.x + GameManager.Instance.CurrentSpeed * fallTime + Random.Range(-1.5f, 1.5f);
            }
            else
            {
                
                x = player.position.x + Random.Range(3f, 14f);
            }
            Instantiate(hazardPrefab, new Vector3(x, spawnY, 0f), Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
        }

        if (water != null) water.SetStorm(0f);
    }
}
