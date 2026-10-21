using UnityEngine;

/// <summary>
/// Put this on the root of every room prefab.
/// Add two empty child objects (EntrySocket, ExitSocket) and assign them here.
/// Place them at floor level, centred in the doorway, on the room's west (entry)
/// and east (exit) sides. Their rotation is ignored in the straight-line version.
/// </summary>
public class RoomPrefab : MonoBehaviour
{
    public Transform entrySocket;
    public Transform exitSocket;

    [Tooltip("Higher = spawns more often.")]
    [Min(1)] public int weight = 1;

    /// <summary>Moves this room so its entry socket sits exactly at worldPoint.</summary>
    public void AlignEntryTo(Vector3 worldPoint)
    {
        transform.position += worldPoint - entrySocket.position;
    }

    private void OnDrawGizmosSelected()
    {
        if (entrySocket != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(entrySocket.position, 0.3f);
        }
        if (exitSocket != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(exitSocket.position, 0.3f);
        }
    }
}
