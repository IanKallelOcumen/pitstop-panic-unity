using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Level select: Back to menu, or load level by index/name.
/// </summary>
public class LevelSelectController : MonoBehaviour
{
    public void OnBack()
    {
        SceneManager.LoadScene(SceneNames.MainMenu);
    }

    public void OnLevel(int levelIndex)
    {
        // Level 1 = build index 2 if 0=MainMenu, 1=LevelSelect
        SceneManager.LoadScene(SceneNames.Garage);
    }

    public void OnLevel1() => OnLevel(1);
    public void OnLevel2() => OnLevel(2);
    public void OnLevel3() => OnLevel(3);
}
