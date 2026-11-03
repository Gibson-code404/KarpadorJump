using UnityEngine;

public class StoneSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject stonePrefab;
    public GameObject sinkingStonePrefab;   // optional, kommt mit den Events
    public GameObject coinPrefab;           // optional, kommt mit den Muenzen

    [Header("Welt")]
    public Transform player;
    public float spawnAhead = 30f;
    public float stoneHeight = 1f;
    public float topY = 0f;
    public float startX = -8f;
    public float firstStoneWidth = 14f;

    [Header("Steinbreite: leicht (x = min, y = max) -> schwer")]
    public Vector2 widthEasy = new Vector2(4f, 7f);
    public Vector2 widthHard = new Vector2(2f, 3.5f);

    [Header("Luecken")]
    public float minGap = 1f;
    public float easyMaxGap = 1.6f;
    [Range(0.3f, 1f)] public float gapSafety = 0.8f;   // Anteil der maximalen Sprungweite

    [Header("Extras")]
    [Range(0f, 1f)] public float coinChance = 0.6f;
    [Range(0f, 1f)] public float sinkChance = 0f;      // Events veraendern diesen Wert

    float nextX;
    float airTime;
    bool firstStone = true;

    void Start()
    {
        nextX = startX;

        // Flugzeit eines Sprungs: t = 2 * v / g
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        PlayerController pc = player.GetComponent<PlayerController>();
        airTime = 2f * pc.jumpVelocity / (Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale);
    }

    void Update()
    {
        while (nextX < player.position.x + spawnAhead) SpawnNext();
    }

    void SpawnNext()
    {
        GameManager gm = GameManager.Instance;
        float d = gm.Difficulty;

        float width = firstStone
            ? firstStoneWidth
            : Random.Range(Mathf.Lerp(widthEasy.x, widthHard.x, d),
                           Mathf.Lerp(widthEasy.y, widthHard.y, d));
        float centerX = nextX + width * 0.5f;

        bool sinking = !firstStone && sinkingStonePrefab != null && Random.value < sinkChance;
        GameObject prefab = sinking ? sinkingStonePrefab : stonePrefab;

        GameObject stone = Instantiate(prefab, new Vector3(centerX, topY - stoneHeight * 0.5f, 0f), Quaternion.identity);
        stone.transform.localScale = new Vector3(width, stoneHeight, 1f);

        if (!firstStone && !sinking && coinPrefab != null && Random.value < coinChance)
            SpawnCoins(centerX, width);

        firstStone = false;

        // Luecke so waehlen, dass sie immer ueberspringbar bleibt
        float reach = gm.CurrentSpeed * airTime;                       // Sprungweite in Metern
        float hardMaxGap = Mathf.Max(easyMaxGap, reach * gapSafety);
        float maxGap = Mathf.Lerp(easyMaxGap, hardMaxGap, d);
        nextX += width + Random.Range(minGap, maxGap);
    }

    void SpawnCoins(float centerX, float width)
    {
        int count = Mathf.Clamp(Mathf.FloorToInt(width / 1.5f), 1, 4);
        for (int i = 0; i < count; i++)
        {
            float x = centerX + (i - (count - 1) * 0.5f) * 1.2f;
            Instantiate(coinPrefab, new Vector3(x, topY + 0.9f, 0f), Quaternion.identity);
        }
    }
}
