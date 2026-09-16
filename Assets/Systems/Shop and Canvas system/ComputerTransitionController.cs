using UnityEngine;

public class ComputerTransitionController : MonoBehaviour
{
    [SerializeField] private ComputerPlayerInteraction player;
    private bool isPlayerInRange = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            player.enterScreen();
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            player.exitScreen();
        }

    }
}
