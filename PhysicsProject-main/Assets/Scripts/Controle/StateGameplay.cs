using UnityEngine;
using UnityEngine.SceneManagement;

public class StateGameplay : MonoBehaviour
{
    public static StateGameplay Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public string SceneName = "Resultado";
    public int TotalDeathTanks = 0;

    public void IncreadeDeathTanks(int amount)
    {
        TotalDeathTanks += amount;

        // Cada inimigo vale 10 pontos
        int pontosAtuais = TotalDeathTanks * 10;

        if (TotalDeathTanks >= 4)
        {
            // O jogador venceu! Envia os dados para o PHP via MatchManager
            if (FindObjectOfType<MatchManager>() != null)
            {
                FindObjectOfType<MatchManager>().FinalizarPartida(pontosAtuais, true);
            }

            SceneManager.LoadScene(SceneName);
        }
    }
}