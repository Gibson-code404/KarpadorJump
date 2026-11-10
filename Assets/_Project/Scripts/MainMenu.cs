using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Text highScoreText;

    void Start()
    {
        Time.timeScale = 1f;
        highScoreText.text = "Rekord " + PlayerPrefs.GetInt(GameManager.HighScoreKey, 0);
    }

    public void OnPlayClicked()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // im Editor beenden wir den Play Mode
#endif
    }
}
