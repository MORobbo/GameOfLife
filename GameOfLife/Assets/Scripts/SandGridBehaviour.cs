using UnityEngine;

public class SandGridBehaviour : MonoBehaviour
{
    public int gridWidth = 50;
    public int gridHeight = 50;
    public Color sandCol = Color.cyan;
    public Color backgroundCol = Color.black;

    private enum Tile { Empty, Sand }
    private Tile[,] cells;

    private Texture2D texture;
    private SpriteRenderer renderer2D;

    void Awake()
    {
        cells = new Tile[gridWidth, gridHeight];

        renderer2D = GetComponent<SpriteRenderer>();
        texture = new Texture2D(gridWidth, gridHeight, TextureFormat.RGBA32, false);

        renderer2D.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, gridWidth, gridHeight),
            Vector2.one * 0.5f,
            1f
        );

        ResetGrid();
    }

    void Update()
    {
        PlaceSand();
        Simulate();
        Draw();
    }

    void ResetGrid()
    {
        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
                cells[x, y] = Tile.Empty;
    }

    void PlaceSand()
    {
        if (!Input.GetMouseButton(0)) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int gx = Mathf.FloorToInt(worldPos.x + gridWidth * 0.5f);
        int gy = Mathf.FloorToInt(worldPos.y + gridHeight * 0.5f);

        if (InBounds(gx, gy))
            cells[gx, gy] = Tile.Sand;
    }

    void Simulate()
    {
        for (int y = 1; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (cells[x, y] == Tile.Sand)
                    MoveSand(x, y);
            }
        }
    }

    void MoveSand(int x, int y)
    {
        if (CanMoveTo(x, y - 1))
            Exchange(x, y, x, y - 1);
        else if (CanMoveTo(x - 1, y - 1))
            Exchange(x, y, x - 1, y - 1);
        else if (CanMoveTo(x + 1, y - 1))
            Exchange(x, y, x + 1, y - 1);
    }

    bool CanMoveTo(int x, int y)
    {
        return InBounds(x, y) && cells[x, y] == Tile.Empty;
    }

    bool InBounds(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    void Exchange(int xA, int yA, int xB, int yB)
    {
        Tile temp = cells[xA, yA];
        cells[xA, yA] = cells[xB, yB];
        cells[xB, yB] = temp;
    }

    void Draw()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                texture.SetPixel(
                    x,
                    y,
                    cells[x, y] == Tile.Sand ? sandCol : backgroundCol
                );
            }
        }

        texture.Apply();
    }
}