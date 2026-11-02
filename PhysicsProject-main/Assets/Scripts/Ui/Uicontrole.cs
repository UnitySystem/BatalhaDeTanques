using UnityEngine;
using UnityEngine.SceneManagement;

public class Uicontrole : MonoBehaviour
{
    public string SceneName = "Login";

    public void StartButton()
    {
        SceneManager.LoadScene(SceneName);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
