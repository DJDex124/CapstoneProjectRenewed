using System.Linq;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using System.Collections.Generic;

public class MazeCollapseTrigger : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        GameManager gm = GameManager.current;
        if (other.CompareTag("Player"))
        {
            gm.playerInMaze = true;
            Debug.Log("Player entered the maze collapse trigger.");
            int time = gm.currentLevel.collapseTime;
            StartCoroutine(gm.mazeCollapseStage1(time));
            if (gm.levelUIDisplays.Count == 0)
            {
                return;
            }
            gm.levelUIDisplays[0].unlock();
            gm.levelUIDisplays.RemoveAt(0);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the maze collapse trigger.");
            GameManager.current.playerInMaze = false;
        }
    }
}
