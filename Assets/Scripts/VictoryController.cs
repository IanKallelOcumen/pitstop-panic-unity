using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryController : MonoBehaviour
{
    [SerializeField] private string garageSceneName = "Garage";
    [SerializeField] private string mainMenuSceneName = "Main_Menu";
    
    // Exposed for PitstopSetup.cs (even if used only for display)
    [SerializeField] private TMP_Text scoreText;

    private void Start()
    {
        if (scoreText != null)
        {
            int lastScore = PlayerPrefs.GetInt("LastScore", 0);
            scoreText.text = $"Final Score: {lastScore}";
        }
    }

    public void OnNextLevel()
    {
        // For now, just reload the garage (or load next level logic)
        SceneManager.LoadScene(garageSceneName);
    }

    public void OnBackToMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
