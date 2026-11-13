using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("HUD")]
    public TMP_Text scoreText;
    public TMP_Text coinText;
    public TMP_Text highScoreText;
    public TMP_Text eventBannerText;

    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public GameObject newRecordLabel;

    [Header("Optional")]
    public Image difficultyFill;   // UI Image mit Image Type "Filled" (Vorlesung 6)

    int lastScore = -1;
    int lastCoins = -1;
    float bannerTimer;

    void Start()
    {
        highScoreText.text = "Rekord " + GameManager.Instance.HighScore;
        eventBannerText.gameObject.SetActive(false);
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;

        // Texte nur neu setzen, wenn sich der Wert aendert (spart Speicher)
        if (gm.Score != lastScore)
        {
            lastScore = gm.Score;
            scoreText.text = lastScore + " Punkte";
        }
        if (gm.Coins != lastCoins)
        {
            lastCoins = gm.Coins;
            coinText.text = "Bonbons " + lastCoins;
        }

        if (difficultyFill != null) difficultyFill.fillAmount = gm.Difficulty;

        pausePanel.SetActive(gm.State == GameState.Paused);

        bool over = gm.State == GameState.GameOver;
        gameOverPanel.SetActive(over);
        if (over)
        {
            finalScoreText.text = gm.Score + " Punkte";
            newRecordLabel.SetActive(gm.NewHighScore);
        }

        if (bannerTimer > 0f)
        {
            bannerTimer -= Time.unscaledDeltaTime;
            if (bannerTimer <= 0f) eventBannerText.gameObject.SetActive(false);
        }
    }

    public void ShowBanner(string message)
    {
        eventBannerText.text = message;
        eventBannerText.gameObject.SetActive(true);
        bannerTimer = 2.5f;
    }

    // Diese Methoden werden im Inspector an die Buttons (OnClick) gehaengt
    public void OnResumeClicked() { GameManager.Instance.Resume(); }
    public void OnRetryClicked() { GameManager.Instance.Restart(); }
    public void OnMenuClicked() { GameManager.Instance.LoadMenu(); }
}
