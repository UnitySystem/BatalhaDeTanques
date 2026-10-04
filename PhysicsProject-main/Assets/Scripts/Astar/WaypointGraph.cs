using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Dimensões do Grid (X, Y)")]
    public int gridSizeX = 10;
    public int gridSizeY = 10;
    public float nodeSpacing = 1.0f;

    [Header("Prefab / Obstáculos")]
    public GameObject nodePrefab;
    public LayerMask obstacleMask;

    [Header("Conexões")]
    public bool allowDiagonals = false;

    [Header("Gizmos")]
    public bool drawGizmos = true;
    public Color walkableColor = Color.green;
    public Color obstacleColor = Color.red;
    public Color connectionColor = Color.cyan;

    private Node[,] grid;

    /// <summary>
    /// Inicializa a criação do grid ao iniciar a execução do jogo
    /// </summary>
    private void Awake()
    {
        GenerateGrid();
    }

    /// <summary>
    /// Instancia a estrutura de nós na cena e estabelece a conexão entre os vizinhos válidos
    /// </summary>
    public void GenerateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 origin = transform.position;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPos = origin + new Vector3(x * nodeSpacing, 0, y * nodeSpacing);
                bool isWalkable = !Physics.CheckSphere(worldPos, nodeSpacing * 0.4f, obstacleMask);

                Node newNode = null;

                if (nodePrefab != null)
                {
                    GameObject obj = Instantiate(nodePrefab, worldPos, Quaternion.identity, transform);
                    obj.name = $"Node_({x},{y})";
                    newNode = obj.GetComponent<Node>();
                }

                if (newNode == null)
                {
                    newNode = new GameObject($"Node_({x},{y})").AddComponent<Node>();
                    newNode.transform.position = worldPos;
                    newNode.transform.SetParent(transform);
                }

                newNode.gridX = x;
                newNode.gridY = y;
                newNode.isWalkable = isWalkable;

                grid[x, y] = newNode;
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node currentNode = grid[x, y];
                if (currentNode == null) continue;

                currentNode.neighbors.Clear();

                for (int checkX = -1; checkX <= 1; checkX++)
                {
                    for (int checkY = -1; checkY <= 1; checkY++)
                    {
                        if (checkX == 0 && checkY == 0) continue;
                        if (!allowDiagonals && Mathf.Abs(checkX) == Mathf.Abs(checkY)) continue;

                        int neighborX = x + checkX;
                        int neighborY = y + checkY;

                        if (neighborX >= 0 && neighborX < gridSizeX && neighborY >= 0 && neighborY < gridSizeY)
                        {
                            Node neighbor = grid[neighborX, neighborY];
                            if (neighbor != null && neighbor.isWalkable)
                            {
                                currentNode.neighbors.Add(neighbor);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Retorna o nó do grid mais próximo com base em uma posição do mundo informada
    /// </summary>
    public Node GetClosestNode(Vector3 worldPosition)
    {
        if (grid == null) return null;

        Vector3 relativePos = worldPosition - transform.position;

        int x = Mathf.RoundToInt(relativePos.x / nodeSpacing);
        int y = Mathf.RoundToInt(relativePos.z / nodeSpacing);

        x = Mathf.Clamp(x, 0, gridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSizeY - 1);

        return grid[x, y];
    }

    /// <summary>
    /// Desenha a representação visual do grid e o estado dos nós diretamente na janela da cena
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Vector3 origin = transform.position;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPos = origin + new Vector3(x * nodeSpacing, 0, y * nodeSpacing);
                bool isWalkable = !Physics.CheckSphere(worldPos, nodeSpacing * 0.4f, obstacleMask);

                Gizmos.color = isWalkable ? walkableColor : obstacleColor;
                Gizmos.DrawSphere(worldPos, 0.15f);
            }
        }
    }
}