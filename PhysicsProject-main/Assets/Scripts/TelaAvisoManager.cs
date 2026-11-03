using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TelaAvisoManager : MonoBehaviour
{
    [SerializeField] private string nomeDoLevelDoJogo;
    public void Aceitar()
    {
        SceneManager.LoadScene(nomeDoLevelDoJogo);
    }

    public void Recusar()
    {
        Debug.Log("Fechando");
        Application.Quit();
    }
}
