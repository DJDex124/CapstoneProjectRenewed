using System.Collections;
using UnityEngine;

public class LowerScript : MonoBehaviour
{
    public GameObject elevatorPrefab;
    public float lowerDistance = 3.0f;
    public float lowerSpeed = 2.0f;
    public float lowerWaitTime = 0.6f;

    private bool isLowering = false;

    public void TriggerElevator()
    {
        if (!isLowering)
        StartCoroutine(LowerEleWithDelay());
    }

    void Start()
    {
      Vector3 loweredPosition = transform.position;
      loweredPosition.y -= lowerDistance;
      transform.position = loweredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LowerEleWithDelay()
    {
        isLowering = true;

        yield return new WaitForSeconds(lowerWaitTime);
        transform.Translate(Vector3.down * lowerSpeed * Time.deltaTime, Space.World);
    }
}
