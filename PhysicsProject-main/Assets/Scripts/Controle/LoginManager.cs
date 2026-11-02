using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI feedbackText;
    public string gameplaySceneName = "Gameplay";

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