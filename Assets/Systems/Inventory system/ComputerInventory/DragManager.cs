using UnityEngine;

public class DragManager : MonoBehaviour
{
    [SerializeField] private DragObjectUI dragObject;

    void Update()
    {
        if (dragObject != null)
            dragObject.transform.position = Input.mousePosition;
    }
}
