using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MatchManager : MonoBehaviour
{
    private string storeUrl = "http://localhost/Senac_aulas/SistemaWeb/index.php?action=store";

    [Serializable]
    public class MatchData
    {
        public int player_id;
        public string start_time;
        public string end_time;
        public int duration;
        public int scores;
        public int victory;
        public int defeats;
    }

    private DateTime startTime;

    void Start()
    {
        IniciarPartida();
    }

    public void IniciarPartida()
    {
        startTime = DateTime.Now;
    }

    public void FinalizarPartida(int scoreFinal, bool venceu)
    {
        DateTime endTime = DateTime.Now;
        int durationSeconds = Mathf.RoundToInt((float)(endTime - startTime).TotalSeconds);

        MatchData dadosPartida = new MatchData
        {
            player_id = GameManager.Instance.playerId,
            start_time = startTime.ToString("yyyy-MM-dd HH:mm:ss"),
            end_time = endTime.ToString("yyyy-MM-dd HH:mm:ss"),
            duration = durationSeconds,
            scores = scoreFinal,
            victory = venceu ? 1 : 0,
            defeats = venceu ? 0 : 1
        };

        string jsonString = JsonUtility.ToJson(dadosPartida);
        StartCoroutine(EnviarPartidaRoutine(storeUrl, jsonString));
    }

    private IEnumerator EnviarPartidaRoutine(string url, string jsonBody)
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
                Debug.Log("Partida enviada com sucesso!");
            }
            else
            {
                Debug.LogError("Erro ao enviar partida: " + request.error);
            }
        }
    }
}