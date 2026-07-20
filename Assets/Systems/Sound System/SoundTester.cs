using UnityEngine;

public class SoundTester : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SoundManager.current.PlaySFX("FootStep");
        }
    }
}
