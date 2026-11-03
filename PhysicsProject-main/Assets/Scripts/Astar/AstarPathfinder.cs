using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implementa o algoritmo de busca de caminhos A* (A-Star) para calcular 
/// a rota mais eficiente entre dois pontos num grid de nós.
/// </summary>
public class AStarPathfinding : MonoBehaviour
{
    [Tooltip("Referência ao gestor do grid responsável por mapear os nós do mapa.")]
    public GridManager gridManager;

    /// <summary>
    /// Calcula o caminho entre duas posições baseadas em coordenadas do mundo (Vector3).
    /// Converte as posições físicas para os nós mais próximos no grid.
    /// </summary>
    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        if (gridManager == null)
        {
            Debug.LogError("GridManager não foi atribuído no AStarPathfinding!");
            return null;
        }

        Node startNode = gridManager.GetClosestNode(startPos);
        Node targetNode = gridManager.GetClosestNode(targetPos);

        return FindPath(startNode, targetNode);
    }

    /// <summary>
    /// Executa o algoritmo A* entre um nó inicial e um nó de destino específicos.
    /// Utiliza uma lista aberta (openSet) e fechada (closedSet) para avaliar custos F, G e H.
    /// </summary>
    public List<Node> FindPath(Node startNode, Node targetNode)
    {
        if (startNode == null || targetNode == null) return null;
        if (!startNode.isWalkable || !targetNode.isWalkable) return null;

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // Seleciona o nó na Open List com o menor FCost (ou menor hCost em caso de empate)
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost ||
                   (Mathf.Approximately(openSet[i].FCost, currentNode.FCost) && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Verifica se o objetivo foi alcançado
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            // Avalia cada vizinho do nó atual
            foreach (Node neighbor in currentNode.neighbors)
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor)) continue;

                // Calcula o custo acumulado de movimento até ao vizinho
                float newCostToNeighbor = currentNode.gCost + Vector3.Distance(currentNode.WorldPosition, neighbor.WorldPosition);

                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = Vector3.Distance(neighbor.WorldPosition, targetNode.WorldPosition);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null; // Retorna nulo caso nenhum caminho seja encontrado
    }

    /// <summary>
    /// Reconstrói o caminho percorrido a partir do nó de destino, 
    /// seguindo a cadeia de referências parentais até à origem.
    /// </summary>
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Add(startNode);
        path.Reverse(); // Inverte para ordenar do início até ao fim

        return path;
    }
}