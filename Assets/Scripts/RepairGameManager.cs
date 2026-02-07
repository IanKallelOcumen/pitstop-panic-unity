using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Runs the garage level: score, time, win condition when all RepairTargets are repaired.
/// Assign UI text and car image; optionally assign "fixed" car sprite to show when all repaired.
/// </summary>
public class RepairGameManager : MonoBehaviour
{
    [Header("Objectives - assign all RepairTargets in scene")]
    public RepairTarget[] repairTargets;

    [Header("UI")]
    public Text scoreText;
    public Text timeText;
    public Text instructionText;
    public string instructionFormat = "Drag the correct tool to fix the {0}";

    [Header("Car display - optional 'fixed' version")]
    public Image carImage;
    public Sprite carBrokenSprite;
    public Sprite carFixedSprite;

    [Header("Game settings")]
    public int scorePerPart = 100;
    public float levelTimeSeconds = 60f;
    public bool countDown = true;

    int score;
    float timeLeft;
    bool gameOver;

    void Start()
    {
        if (repairTargets == null || repairTargets.Length == 0)
            repairTargets = FindObjectsByType<RepairTarget>(FindObjectsSortMode.None);

        timeLeft = levelTimeSeconds;
        score = 0;
        gameOver = false;
        UpdateUI();
        if (carImage != null && carBrokenSprite != null)
            carImage.sprite = carBrokenSprite;
    }

    void Update()
    {
        if (gameOver) return;

        if (countDown)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f)
            {
                timeLeft = 0f;
                EndLevel(false);
            }
        }
        else
        {
            timeLeft += Time.deltaTime;
        }

        UpdateUI();
    }

    public void OnPartRepaired(RepairTarget target)
    {
        if (gameOver) return;
        score += scorePerPart;
        CheckAllRepaired();
    }

    void CheckAllRepaired()
    {
        foreach (var t in repairTargets)
            if (t != null && !t.isRepaired) return;

        EndLevel(true);
    }

    void EndLevel(bool won)
    {
        gameOver = true;
        if (won)
        {
            if (carImage != null && carFixedSprite != null)
                carImage.sprite = carFixedSprite;
            PlayerPrefs.SetInt("LastScore", score);
            SceneManager.LoadScene(SceneNames.Victory);
        }
        else
        {
            // Optional: load "GameOver" scene or show retry UI
            if (instructionText != null)
                instructionText.text = "Time's up! Try again.";
        }
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (timeText != null) timeText.text = "Time: " + Mathf.CeilToInt(timeLeft);
    }
}
