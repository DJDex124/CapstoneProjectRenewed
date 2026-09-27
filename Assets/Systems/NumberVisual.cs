using UnityEngine;
using TMPro;
using System.Collections;

public class NumberVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshPro text;

    [Header("Movement")]
    [SerializeField] private float floatHeight = 1.2f;
    [SerializeField] private float animationDuration = 1.2f;

    [Header("Pop")]
    [SerializeField] private float startScale = 0.2f;
    [SerializeField] private float popScale = 1.2f;
    [SerializeField] private float finalScale = 1f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private Color originalColor;

    private Camera mainCamera;


    private void Awake()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshPro>();
        }

        if (text != null)
        {
            originalColor = text.color;
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void ShowValue(int value)
    {
        if (text == null)
        {
            Debug.LogError("FloatingValueText is missing its TextMeshPro component.");
            return;
        }


        text.text = "+" + value;


        Color color = originalColor;
        color.a = 1f;
        text.color = color;


        transform.localScale = Vector3.one * startScale;


        startPosition = transform.position;

        targetPosition = startPosition + Vector3.up * floatHeight;



        StartCoroutine(AnimateValue());
    }

    private IEnumerator AnimateValue()
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / animationDuration;

            float movementT = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                movementT
            );

            if (t < 0.25f)
            {

                float popT = t / 0.25f;

                transform.localScale = Vector3.one *
                    Mathf.Lerp(
                        startScale,
                        popScale,
                        Mathf.SmoothStep(0f, 1f, popT)
                    );
            }
            else
            {
                float settleT = (t - 0.25f) / 0.75f;

                transform.localScale = Vector3.one *
                    Mathf.Lerp(
                        popScale,
                        finalScale,
                        Mathf.SmoothStep(0f, 1f, settleT)
                    );
            }

            if (t > 0.5f)
            {
                float fadeT = (t - 0.5f) / 0.5f;

                Color color = originalColor;

                color.a = Mathf.Lerp(
                    1f,
                    0f,
                    fadeT
                );

                text.color = color;
            }

            yield return null;
        }
        Color finalColor = originalColor;
        finalColor.a = 0f;
        text.color = finalColor;

        Destroy(gameObject);
    }

    private void LateUpdate()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            transform.forward = mainCamera.transform.forward;
        }
    }


}
