using UnityEngine;

public class EntityNodePath : MonoBehaviour
{
    public Transform[] nodes;

    void OnDrawGizmos()
    {
        if (nodes == null || nodes.Length < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < nodes.Length - 1; i++)
        {
            Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);
        }
    }
}