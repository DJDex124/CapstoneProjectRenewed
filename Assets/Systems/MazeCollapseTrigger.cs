using UnityEngine;

public class MazeCollapseTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager gm = GameManager.current;
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the maze collapse trigger.");
            StartCoroutine(gm.mazeCollapseCountdown(60f));
        }
    }
}
