using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class OneWayRoads : MonoBehaviour
{
    public PointGraph pointGraph; /// <summary>
    /// /
    /// </summary>
    /// 

    void Awake()
    {
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.1f);

        var graph = AstarPath.active.data.pointGraph;

        for (int i = 0; i < graph.nodes.Length; i++)
        {
            graph.nodes[i].ClearConnections(true);

        }
        Debug.Log("graph.nodes lenght:" + graph.nodes.Length);

        for (int i = 0; i < graph.nodes.Length; i++)
        {
            GraphNode nodeA = graph.nodes[i]; // Replace with actual node
            GraphNode nodeB = graph.nodes[(i + 1) % graph.nodes.Length]; // Replace with actual node

            nodeA.AddPartialConnection(nodeB, 1000, true, false);
            nodeB.AddPartialConnection(nodeA, 0, false, true);
        }
        // Example: Set one-way connection from node A to node B
        //nodeA.AddPartialConnection(node2, cost, true, true);

        GameObject rootObject = new GameObject("PointGraphRoot");
        foreach (var node in graph.nodes)
        {
            if (node is PointNode pointNode)
            {
                Transform nodeTransform = pointNode.gameObject?.transform;
                nodeTransform.SetParent(rootObject.transform);
            }
        }

        pointGraph.root = rootObject.transform;
        AstarPath.active.Scan();

        Debug.Log("Scan");
    }
}
