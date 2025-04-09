using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GrindTrigger : MonoBehaviour
{
    public GrindPath path;

    private PlayerGrind lastPlayer; // Track the last player who triggered

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerGrind player) && path != null)
        {
            // Avoid retriggering if already grinding or if it's the same player
            if (!player.IsGrinding() && player != lastPlayer)
            {
                lastPlayer = player;
                player.StartGrind(path);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset the trigger once the player fully exits
        if (other.TryGetComponent(out PlayerGrind player) && player == lastPlayer)
        {
            lastPlayer = null;
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
