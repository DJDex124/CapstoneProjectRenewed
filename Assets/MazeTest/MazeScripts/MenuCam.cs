using UnityEngine;

public class MenuCam : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField]
    private Vector3 moveDirection =
        new Vector3(1f, 0f, 1f);

    [SerializeField] private float rotationSpeed = 3f;

    private void Update()
    {
        transform.position +=
            moveDirection.normalized *
            moveSpeed *
            Time.deltaTime;

        transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.World
        );
    }
}
