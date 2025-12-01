using UnityEngine;

public class SandGridBehaviour : MonoBehaviour
{
    public int width = 200;
    public int height = 200;
    public Color sandColor = new Color(0.9f, 0.85f, 0.6f);
    public Color emptyColor = Color.black;

    enum CellType { Empty, Sand }
    CellType[,] grid;

    Texture2D tex;
    SpriteRenderer sr;

    void Start()
    {
        grid = new CellType[width, height];
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("Add a SpriteRenderer to this GameObject.");
            return;
        }
        tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 1f);
        ClearGrid();
    }

    void Update()
    {
        HandleInput();
        UpdateSandSimulation();
        RenderGrid();
    }

    void ClearGrid()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = CellType.Empty;
    }

    void HandleInput()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            int x = Mathf.FloorToInt(wp.x + width / 2f);
            int y = Mathf.FloorToInt(wp.y + height / 2f);

            if (x >= 0 && x < width && y >= 0 && y < height)
                grid[x, y] = CellType.Sand;
        }
    }

    void UpdateSandSimulation()
    {

        for (int y = 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] == CellType.Sand)
                    TryMoveSand(x, y);
            }
        }
    }

    void TryMoveSand(int x, int y)
    {
        if (IsEmpty(x, y - 1))
        {
            Swap(x, y, x, y - 1);
        }
        else if (IsEmpty(x - 1, y - 1))
        {
            Swap(x, y, x - 1, y - 1);
        }
        else if (IsEmpty(x + 1, y - 1))
        {
            Swap(x, y, x + 1, y - 1);
        }
    }

    bool IsEmpty(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;
        return grid[x, y] == CellType.Empty;
    }

    void Swap(int x1, int y1, int x2, int y2)
    {
        var t = grid[x1, y1];
        grid[x1, y1] = grid[x2, y2];
        grid[x2, y2] = t;
    }

    void RenderGrid()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tex.SetPixel(x, y,
                    (grid[x, y] == CellType.Sand) ? sandColor : emptyColor);
            }
        }
        tex.Apply();
    }
}
