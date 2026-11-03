using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla o estado dinâmico da partida em execução, computando a pontuação 
/// com base nos objetivos atingidos e acionando o encerramento da match.
/// </summary>
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

    /// <summary>
    /// Incrementa a contagem de inimigos derrotados, calcula a pontuação 
    /// e verifica se a condição de vitória foi atingida.
    /// </summary>
    /// <param name="amount">Quantidade de tanques destruídos.</param>
    public void IncreadeDeathTanks(int amount)
    {
        TotalDeathTanks += amount;

        // Cada inimigo destruído concede 10 pontos de pontuação
        int pontosAtuais = TotalDeathTanks * 10;

        // Condição de Vitória: Atingir 4 ou mais tanques destruídos
        if (TotalDeathTanks >= 4)
        {
            // Aciona o MatchManager para enviar os dados de vitória ao PHP antes de mudar de cena
            if (FindObjectOfType<MatchManager>() != null)
            {
                FindObjectOfType<MatchManager>().FinalizarPartida(pontosAtuais, true);
            }

            SceneManager.LoadScene(SceneName);
        }
    }
}