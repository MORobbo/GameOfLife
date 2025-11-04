using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class GridBehaviour : MonoBehaviour
{

    private readonly HashSet<Vector3Int> aliveCells = new();
    private readonly HashSet<Vector3Int> cellsToCheck = new();
    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tilemap nextState;
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile jedTile;
    [SerializeField] private Pattern pattern;
    [SerializeField] private float updateInterval = 0.05f;




    private void Awake()
    {
        SetPattern(pattern);
    }

    private void SetPattern(Pattern pattern)
    {
        Clear();

        Vector2Int center = pattern.GetCenter();

        for (int i = 0; i < pattern.cells.Length; i++)
        {
            Vector3Int cell = (Vector3Int)(pattern.cells[i] - center);
            currentState.SetTile(cell, aliveTile);
            aliveCells.Add(cell);
        }

        Debug.Log($"Pattern has been set. There are {aliveCells.Count} cells alive in this pattern.");

    }

    private void Clear()
    {
        aliveCells.Clear();
        cellsToCheck.Clear();
        currentState.ClearAllTiles();
        nextState.ClearAllTiles();
    }


    private void OnEnable()
    {
        StartCoroutine(Simulate());
    }

    private IEnumerator Simulate()
    {
        var interval = new WaitForSeconds(updateInterval);
        yield return interval;

        while (enabled)
        {
            UpdateState();

            yield return interval;
        }
    }

    private void UpdateState()
    {
        cellsToCheck.Clear();

        Debug.Log("Updating state...");
        foreach (Vector3Int cell in aliveCells)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    cellsToCheck.Add(cell + new Vector3Int(x, y));
                }
            }
        }

        Debug.Log($"Finished calculating cells to check. There are {cellsToCheck.Count} for this cycle");


        foreach (Vector3Int cell in cellsToCheck)
        {
            int neighbours = 0;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector3Int neighbour = cell + new Vector3Int(x, y);
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    if (currentState.GetTile(neighbour) == aliveTile)
                    {
                        neighbours++;
                    }
                }
            }

            bool isAlive = currentState.GetTile(cell) == aliveTile;

            Debug.Log($"Alive ? {isAlive}");

            if ((neighbours < 2 || neighbours > 3) && isAlive)
            {
                nextState.SetTile(cell, jedTile);
                aliveCells.Remove(cell);

            }
            else if (!isAlive && neighbours == 3)
            {
                nextState.SetTile(cell, aliveTile);
                aliveCells.Add(cell);
            }
            else
            {
                nextState.SetTile(cell, currentState.GetTile(cell));
            }
        }

        Debug.Log($"Finished calculating alive / dead cells for next cyle there are {aliveCells.Count} for the next cycle");

        Tilemap temp = currentState;
        currentState = nextState;
        nextState = temp;
        nextState.ClearAllTiles();

    }

}