using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Victory screen: "Car Repaired! Test drive successful." - Next Level and Back To Menu.
/// </summary>
public class VictoryController : MonoBehaviour
{
    public Text scoreText;

    void Start()
    {
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        if (scoreText != null)
            scoreText.text = "Score: " + lastScore;
    }

    public void OnNextLevel()
    {
        SceneManager.LoadScene(SceneNames.Garage);
    }

    public void OnBackToMenu()
    {
        SceneManager.LoadScene(SceneNames.MainMenu);
    }
}
