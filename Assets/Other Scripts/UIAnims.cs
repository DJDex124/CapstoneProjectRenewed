using UnityEngine;
using TMPro;
using System.Collections;

public class UIAnims : MonoBehaviour
{
    [Header("Jitter Amount")]
    [SerializeField] private float horizontalJitter = 0.05f;
    [SerializeField] private float verticalJitter = 0.01f;

    [Header("Glitch Timing")]
    [SerializeField] private float minDelay = 0.1f;
    [SerializeField] private float maxDelay = 1.5f;

    [Header("Glitch Duration")]
    [SerializeField] private float minDuration = 0.02f;
    [SerializeField] private float maxDuration = 0.15f;

    [Header("Jitter Speed")]
    [SerializeField] private float jitterSpeed = 0.02f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;

        StartCoroutine(GlitchLoop());
    }

    private IEnumerator GlitchLoop()
    {
        while (true)
        {
            
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            
            float duration = Random.Range(minDuration, maxDuration);

            yield return StartCoroutine(Jitter(duration));
        }
    }

    private IEnumerator Jitter(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            Vector2 offset = new Vector2(
                Random.Range(-horizontalJitter, horizontalJitter),
                Random.Range(-verticalJitter, verticalJitter)
            );

            rectTransform.anchoredPosition = originalPosition + offset;

            float delay = Random.Range(0.01f, jitterSpeed);

            timer += delay;

            yield return new WaitForSeconds(delay);
        }

        
        rectTransform.anchoredPosition = originalPosition;
    }

    private void OnDisable()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}
