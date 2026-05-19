using UnityEngine;
using UnityEngine.UI;

public class MazeDrawingMech : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RawImage mapImage;
    [SerializeField] private GameObject mapPanel;

    [Header("Texture Settings")]
    [SerializeField] private int textureWidth = 512;
    [SerializeField] private int textureHeight = 512;

    [Header("Brush Settings")]
    [SerializeField] private Color drawColor = Color.black;
    [SerializeField] private Color eraseColor = Color.white;
    [SerializeField] private int brushSize = 4;

    private Texture2D drawTexture;

    private bool mapOpen;


    private Vector2 lastMousePos;

    private void Start()
    {
        CreateBlankMap();
        mapPanel.SetActive(false);
    }

    private void Update()
    {
        HandleMapToggle();

        if (mapOpen)
        {
            Draw();
        }
    }

    void HandleMapToggle()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            mapOpen = !mapOpen;

            PlayerMovementCC.current.enabled = !mapOpen;
            CameraControllerCC.current.enabled = !mapOpen;

            mapPanel.SetActive(mapOpen);

            Cursor.lockState = mapOpen
                ? CursorLockMode.None
                : CursorLockMode.Locked;

            Cursor.visible = mapOpen;
        }
    }

    void CreateBlankMap()
    {
        drawTexture = new Texture2D(
            textureWidth,
            textureHeight,
            TextureFormat.RGBA32,
            false
        );

        Color[] pixels = new Color[textureWidth * textureHeight];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;

        drawTexture.SetPixels(pixels);
        drawTexture.Apply();

        mapImage.texture = drawTexture;
    }

    void Draw()
    {
        if (!Input.GetMouseButton(0))
            return;

        bool isErasing = Input.GetMouseButton(1);
        Color currentColor = isErasing ? eraseColor : drawColor;

        RectTransform rectTransform = mapImage.rectTransform;

        Vector2 localPoint;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            Input.mousePosition,
            null,
            out localPoint))
        {
            return;
        }

        Rect rect = rectTransform.rect;

        float normalizedX = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x);
        float normalizedY = Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y);

        int x = Mathf.RoundToInt(normalizedX * textureWidth);
        int y = Mathf.RoundToInt(normalizedY * textureHeight);

        Vector2 currentMousePos = new Vector2(x, y);

        DrawLine(lastMousePos, currentMousePos, currentColor);

        lastMousePos = currentMousePos;
    }

    void DrawLine(Vector2 from, Vector2 to, Color color)
    {
        float distance = Vector2.Distance(from, to);
        int steps = Mathf.CeilToInt(distance * 2f);

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 point = Vector2.Lerp(from, to, t);

            DrawCircle((int)point.x, (int)point.y, color);
        }
    }

    void DrawCircle(int centerX, int centerY, Color color)
    {
        for (int x = -brushSize; x <= brushSize; x++)
        {
            for (int y = -brushSize; y <= brushSize; y++)
            {
                int drawX = centerX + x;
                int drawY = centerY + y;

                if (drawX < 0 || drawX >= textureWidth ||
                    drawY < 0 || drawY >= textureHeight)
                    continue;

                float distance = Mathf.Sqrt(x * x + y * y);

                if (distance <= brushSize)
                {
                    drawTexture.SetPixel(drawX, drawY, color);
                }
            }
        }

        drawTexture.Apply();
    }
}
