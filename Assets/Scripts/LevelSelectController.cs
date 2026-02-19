using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private string garageSceneName = "Garage";
    [SerializeField] private string mainMenuSceneName = "Main_Menu";

    public void LoadGarageLevel(int level)
    {
        GameSession.SelectedLevel = level;
        SceneManager.LoadScene(garageSceneName);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
