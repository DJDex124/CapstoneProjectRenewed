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
    [SerializeField] private int eraserSize = 10;
    [SerializeField] private RectTransform brushUI;
    [SerializeField] private int brushUISize;

    private Texture2D drawTexture;

    private bool mapOpen;

    private bool wasDrawingLastFrame = false;

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
            UpdateBrushPreview();
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
            brushUI.gameObject.SetActive(mapOpen);
            Cursor.lockState = mapOpen
                ? CursorLockMode.None
                : CursorLockMode.Locked;

            
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
        bool isDrawing = Input.GetMouseButton(0);
        bool isErasing = Input.GetMouseButton(1);

        if (!isDrawing && !isErasing)
        {
            wasDrawingLastFrame = false;
            return;
        }

        int currentSize = isErasing ? eraserSize : brushSize;
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

        if (!wasDrawingLastFrame)
        {
            DrawCircle(x, y, currentColor, currentSize);
        }
        else
        {
            DrawLine(lastMousePos, currentMousePos, currentColor, currentSize);
        }

        lastMousePos = currentMousePos;
        wasDrawingLastFrame = true;
    }

    void DrawLine(Vector2 from, Vector2 to, Color color, int size)
    {
        float distance = Vector2.Distance(from, to);
        int steps = Mathf.CeilToInt(distance * 2f);

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 point = Vector2.Lerp(from, to, t);

            DrawCircle((int)point.x, (int)point.y, color, size);
        }
    }

    void DrawCircle(int centerX, int centerY, Color color, int size)
    {
        for (int x = -size; x <= size; x++)
        {
            for (int y = -size; y <= size; y++)
            {
                int drawX = centerX + x;
                int drawY = centerY + y;

                if (drawX < 0 || drawX >= textureWidth ||
                    drawY < 0 || drawY >= textureHeight)
                    continue;

                float distance = Mathf.Sqrt(x * x + y * y);

                if (distance <= size)
                {
                    drawTexture.SetPixel(drawX, drawY, color);
                }
            }
        }

        drawTexture.Apply();
    }

    void UpdateBrushPreview()
    {
        brushUI.gameObject.SetActive(true);

        
        brushUI.position = Input.mousePosition;

        bool isErasing = Input.GetMouseButton(1);

        int currentSize = isErasing ? eraserSize : brushSize;


        float scaleFactor = mapImage.rectTransform.rect.width / textureWidth;

        float previewSize = currentSize * 24f * scaleFactor;

        brushUI.sizeDelta = new Vector2(previewSize, previewSize);

        
        Image image = brushUI.GetComponent<Image>();

        image.color = isErasing
            ? new Color(1f, 0f, 0f, 0.35f)
            : new Color(0f, 0f, 0f, 0.35f);
    }
}
