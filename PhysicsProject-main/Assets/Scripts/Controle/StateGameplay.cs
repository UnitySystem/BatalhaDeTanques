using UnityEngine;
using UnityEngine.SceneManagement;

public class StateGameplay : MonoBehaviour
{
    public static StateGameplay Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    public string SceneName = "Resultado";
    int TotalDeathTanks = 0;

    public void IncreadeDeathTanks(int amont)
    {
        TotalDeathTanks += amont;
        if( TotalDeathTanks >= 4)
        {
            SceneManager.LoadScene(SceneName);
        }
    }

}
