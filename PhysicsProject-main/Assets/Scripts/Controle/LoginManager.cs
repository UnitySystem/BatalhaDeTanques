using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Gerencia o processo de autenticação do jogador, validação de credenciais via API PHP 
/// e transição segura para a cena de gameplay.
/// </summary>
public class LoginManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI feedbackText;
    public string gameplaySceneName = "Gameplay";

    [Tooltip("Endpoint da API PHP para autenticação.")]
    private string loginUrl = "http://localhost/Senac_aulas/SistemaWeb/index.php?action=api_login";

    [Serializable]
    public class LoginData
    {
        public string email;
        public string password;
        public string token;
    }

    [Serializable]
    public class JogadorResponse
    {
        public int id;
        public string name;
        public string email;
    }

    /// <summary>
    /// Coleta os dados de login inseridos pelo usuário, gera um token GUID único 
    /// e inicia a corrotina de requisição HTTP.
    /// </summary>
    public void FazerLogin()
    {
        LoginData dadosLogin = new LoginData
        {
            email = emailInput.text,
            password = passwordInput.text,
            token = Guid.NewGuid().ToString()
        };

        string jsonString = JsonUtility.ToJson(dadosLogin);
        StartCoroutine(LoginRoutine(loginUrl, jsonString));
    }

    /// <summary>
    /// Corrotina que realiza o POST para o middleware de login.
    /// - Valida se o e-mail e senha são aceitos.
    /// - Extrai o JSON de resposta contendo os dados do jogador e armazena no GameManager.
    /// - Trata falhas de conexão exibindo feedback visual na UI[cite: 13].
    /// </summary>
    private IEnumerator LoginRoutine(string url, string jsonBody)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;

                if (jsonResponse.Contains("error"))
                {
                    if (feedbackText != null) feedbackText.text = "E-mail ou senha inválidos.";
                }
                else
                {
                    JogadorResponse jogador = JsonUtility.FromJson<JogadorResponse>(jsonResponse);
                    GameManager.Instance.playerId = jogador.id;
                    GameManager.Instance.playerName = jogador.name;

                    SceneManager.LoadScene(gameplaySceneName);
                }
            }
            else
            {
                if (feedbackText != null) feedbackText.text = "Erro de conexão com o servidor.";
            }
        }
    }
}