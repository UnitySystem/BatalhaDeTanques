using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// ==============================================================================
/// RELATÓRIO TÉCNICO E DOCUMENTAÇÃO OFICIAL - ATIVIDADE 2 (UNITY + PHP + MYSQL)
/// ==============================================================================
/// 
/// 1. IDENTIFICAÇÃO DO PROJETO E EQUIPA:
///    - Nome do Jogo: Batalha dos Tanques
///    - Integrantes e Papéis:
///      * Felipe Sousa Pinto (Programação e Arquitetura de Scripts na Unity e PHP)
///      * Guilherme (Implementação da Interface)
///      * Alex (Execução dos Playtests e Análise de Desempenho)
/// 
/// 2. DINÂMICA DO JOGO:
///    - Objetivo: Destruir 4 tanques inimigos controlados por IA antes de perder toda a vida em combate.
///    - Controles: Teclas WASD para translação e rotação física do tanque, teclas dedicadas para a inclinação do canhão e Botão Esquerdo faz o disparo de projéteis.
///    - Condições de Vitória e Derrota: O jogador vence ao atingir ou superar a meta de 4 tanques inimigos eliminados a derrota ocorre caso a vida do jogador chegue a zero.
/// 
/// 3. CRITÉRIOS DE AVALIAÇÃO E RANKING:
///    - Critério de Melhor Resultado: Maior pontuação obtida (10 pontos acumulados por cada inimigo destruído com sucesso).
///    - Regra de Ordenação do Ranking: Ordenado de forma decrescente pela pontuação (maior score primeiro).
///    - Regra de Desempate: Menor duração total da partida em segundos.
/// 
/// 4. GOVERNANÇA E LGPD (CONFORMIDADE):
///    - Dados Coletados: ID único do jogador, timestamp de início, timestamp de término, duração total em segundos, pontuação final e indicadores binários de vitória/derrota.
///    - Finalidade: Análise de desempenho de gameplay, cálculo estatístico de tempo médio, quantidades de vitórias/derrotas e exibição de histórico consolidado na view web do MVC.
///    - Armazenamento e Retenção: Os dados são persistidos de forma segura na tabela relacional 'matches' da base de dados MySQL (`matches.sql`)[cite: 26, 29] por tempo indeterminado para fins académicos.
///    - Transparência na Interface: O utilizador é devidamente avisado na interface sobre a finalidade e segurança dos dados antes de iniciar o gameplay.
/// 
/// 5. INSTRUÇÕES DE CONFIGURAÇÃO E EXECUÇÃO:
///    - Configuração do Middleware: Certifique-se de que o servidor Apache (via XAMPP) está em execução e que a URL base nos scripts de rede da Unity (`storeUrl` e `loginUrl`) aponta corretamente para os endpoints da API PHP (ex.: http://localhost/Senac_aulas/SistemaWeb/index.php?action=store).
///    - Pré-requisito de Acesso: Para poder acessar o jogo, o usuário primeiramente deve criar uma conta nova acessando o painel web (view web) através da tela de registro.
///    - Execução do Projeto: Importe o arquivo `matches.sql` no phpMyAdmin para recriar a estrutura do banco de dados e carregar os dados de teste. Em seguida, abra o jogo, utilize as credenciais da conta criada na tela de login e inicie o gameplay.
/// 
/// 6. ESTRUTURA DE DADOS E BASE DE DADOS:
///    - Tabelas do Sistema (Esquema MySQL):
///      * 'jogadores': Gere o cadastro e credenciais encriptadas (`password_hash`) dos utilizadores.
///      * 'tokens': Gere o controlo de sessões ativas e prazos de validade de autenticação.
///      * 'matches': Regista o histórico detalhado de partidas (ID, player_id, Start_Time, End_Time, 
///        Duration, Victory, Defeats, Scores) associadas via chave estrangeira.
///    - Arquivo SQL Entregue: `matches.sql`.
/// 
/// ==============================================================================
/// </summary>
public class MatchManager : MonoBehaviour
{
    [Header("Configuração da API")]
    [Tooltip("Rota do middleware PHP responsável por salvar os dados da partida.")]
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

    /// <summary>
    /// Registra o timestamp exato do início da partida ao carregar o gameplay.
    /// </summary>
    public void IniciarPartida()
    {
        startTime = DateTime.Now;
    }

    /// <summary>
    /// Finaliza a partida calculando a duração, empacotando os dados e disparando a corrotina de envio.
    /// </summary>
    /// <param name="scoreFinal">Pontuação acumulada pelo jogador.</param>
    /// <param name="venceu">Booleano indicando se o jogador venceu (true) ou perdeu (false).</param>
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

    /// <summary>
    /// Corrotina responsável por enviar os dados da partida via UnityWebRequest (POST) para a API PHP.
    /// - Rota utilizada: index.php?action=store[cite: 3]
    /// - Tratamento de Erro: Valida o sucesso da requisição HTTP e exibe logs em caso de falha de comunicação.
    /// </summary>
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
                Debug.Log("Partida enviada e salva com sucesso no banco de dados!");
            }
            else
            {
                Debug.LogError("Erro ao enviar partida para o middleware: " + request.error);
            }
        }
    }
}