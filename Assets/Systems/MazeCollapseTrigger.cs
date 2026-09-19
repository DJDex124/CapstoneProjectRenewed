using UnityEngine;

public class MazeCollapseTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the maze collapse trigger.");
            GameManager.current.StartCoroutine(GameManager.current.mazeCollapseCountdown(60f));
        }
    }
}
