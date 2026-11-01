using UnityEngine;
using UnityEngine.SceneManagement;

public class Uicontrole : MonoBehaviour
{
    public string SceneName = "Gameplay";

    public void StartButton()
    {
        SceneManager.LoadScene(SceneName);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
