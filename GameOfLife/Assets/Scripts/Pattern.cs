using UnityEngine;

[CreateAssetMenu(fileName = "New Pattern", menuName = "Pattern")]
public class Pattern : ScriptableObject
{
    public Vector2Int[] cells;

    public Vector2Int GetCenter()
    {
        if (cells == null || cells.Length == 0)
        {
            return Vector2Int.zero;
        }

        int minX = cells[0].x;
        int maxX = cells[0].x;
        int minY = cells[0].y;
        int maxY = cells[0].y;

        foreach (var cell in cells)
        {
            if (cell.x < minX) minX = cell.x;
            if (cell.x > maxX) maxX = cell.x;
            if (cell.y < minY) minY = cell.y;
            if (cell.y > maxY) maxY = cell.y;
        }

        //Gives the center position
        return new Vector2Int((minX + maxX) / 2, (minY + maxY) / 2);
    }

}
