using UnityEngine;
using UnityEngine.Tilemaps;
public class GridBehaviour : MonoBehaviour
{
    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tilemap nextState;
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile jedTile;
    [SerializeField] private Pattern pattern;
    [SerializeField] private float updateInterval = 0.05f;


    private void Start()
    {
        SetPattern(pattern);
    }

    private void SetPattern(Pattern pattern)
    {
        Clear();

        for (int i = 0; i < pattern.cells.Length; i++)
        {
            Vector2Int cell = pattern.cells[i];
            currentState.SetTile(new Vector3Int(cell.x, cell.y, 0), aliveTile);
        }
    }

    private void Clear()
    {
        currentState.ClearAllTiles();
        nextState.ClearAllTiles();
    }
}
