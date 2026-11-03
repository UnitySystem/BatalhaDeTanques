using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Representa uma célula ou nó individual dentro do grid de navegação, 
/// armazenando propriedades de custo e conexões com vizinhos.
/// </summary>
public class Node : MonoBehaviour
{
    public bool isWalkable = true;
    public List<Node> neighbors = new List<Node>();

    [HideInInspector] public float gCost;
    [HideInInspector] public float hCost;
    [HideInInspector] public Node parent;

    [Header("Coordenadas na Matriz")]
    public int gridX;
    public int gridY;

    /// <summary>Retorna a posição física exata do nó no mundo 3D.</summary>
    public Vector3 WorldPosition => transform.position;

    /// <summary>Calcula dinamicamente o custo total (G + H) para o algoritmo A*.</summary>
    public float FCost => gCost + hCost;
}