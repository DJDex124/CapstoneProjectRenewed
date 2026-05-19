using UnityEngine;

public class GlowstickLogic : MonoBehaviour
{
    [SerializeField] private float lifetime = 120f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
