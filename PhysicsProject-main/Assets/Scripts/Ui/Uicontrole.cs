using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla as interações básicas de interface de utilizador (UI), 
/// como transições de cenas e encerramento da aplicação.
/// </summary>
public class Uicontrole : MonoBehaviour
{
    [Tooltip("Nome da cena de destino ao iniciar.")]
    public string SceneName = "Login";

    /// <summary>
    /// Realiza a transição para a cena configurada ao interagir com o botão de início.
    /// </summary>
    public void StartButton()
    {
        SceneManager.LoadScene(SceneName);
    }

    /// <summary>
    /// Encerra a execução da aplicação de jogo.
    /// </summary>
    public void ExitButton()
    {
        Application.Quit();
    }
}