using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool isWalkable = true;
    public List<Node> neighbors = new List<Node>();

    [HideInInspector] public float gCost;
    [HideInInspector] public float hCost;
    [HideInInspector] public Node parent;
    public int gridX;
    public int gridY;

    public Vector3 WorldPosition => transform.position;
    public float FCost => gCost + hCost;
}