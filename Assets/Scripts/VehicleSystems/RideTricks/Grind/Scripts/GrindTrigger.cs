using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GrindTrigger : MonoBehaviour
{
    public GrindPath path;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerGrind player) && path != null)
        {
            player.StartGrind(path);
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, path.transform.position);
        }
    }
}
