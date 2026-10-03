using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    public GridManager gridManager;
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

            // Chegou ao objetivo
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            // Avalia cada vizinho do nó atual
            foreach (Node neighbor in currentNode.neighbors)
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor)) continue;

                // Custo do movimento para o vizinho (distância física)
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

        return null; // Caminho não encontrado
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        // Reconstrói o caminho seguindo a cadeia de pais (parent) até a origem
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Add(startNode);
        path.Reverse(); // Inverte para obter a ordem do início ao fim

        return path;
    }
}