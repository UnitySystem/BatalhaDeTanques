using UnityEngine;

/// <summary>
/// Gestor central global da aplicação baseado no padrão Singleton. 
/// Mantém os dados da sessão persistidos entre as diferentes cenas do jogo.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Dados do Jogador Autenticado")]
    public int playerId;
    public string playerName;

    private void Awake()
    {
        // Garante instância única e persistência de dados ao carregar novas cenas
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}