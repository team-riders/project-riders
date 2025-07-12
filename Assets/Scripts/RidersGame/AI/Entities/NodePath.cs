using UnityEngine;

public class NodePath : MonoBehaviour
{
    public Transform[] nodes;

    void OnDrawGizmos()
    {
        if (nodes == null || nodes.Length < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < nodes.Length - 1; i++)
        {
            Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);
        }
    }
}