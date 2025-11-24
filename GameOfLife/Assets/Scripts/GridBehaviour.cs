using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Colour = UnityEngine.Color;

public class GridBehaviour : MonoBehaviour, IGrid
{

    bool IGrid.Playing => m_playing;
    private readonly HashSet<Vector3Int> aliveCells = new();
    private readonly HashSet<Vector3Int> cellsToCheck = new();
    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tilemap nextState;
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile jedTile;
    [SerializeField] private Button PlayButton;
    public bool m_playing;
    private Coroutine simulateRoutine;
    private float accelerometerSpeed;
    private float updateInterval = 0.1f;

    private void Awake()
    {
        PlayButton.onClick.AddListener(() =>
        {
            m_playing = !m_playing;

            if (m_playing && simulateRoutine == null)
            {
                simulateRoutine = StartCoroutine(Simulate());
            }
            else
            {
                Clear();
                if (simulateRoutine != null)
                {
                    StopCoroutine(simulateRoutine);
                    simulateRoutine = null;
                }
            }

        });

    }



    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && !m_playing)
        {
            Vector3Int cell;
            if (ShouldCreateTile(out cell))
            {
                aliveTile.color = RandomTileColour();
                currentState.SetTile(cell, aliveTile);
                aliveCells.Add(cell);
            }
        }
    }

    private bool ShouldCreateTile(out Vector3Int cell)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 buttonPos = PlayButton.transform.position;
        cell = currentState.WorldToCell(mouseWorldPos);

        if (!CellIsAlive(cell) & Vector3.Distance(mouseWorldPos, buttonPos) > 0.5f)
        {
            return true;
        }
        return false;
    }
    public Colour RandomTileColour()
    {
        var values = Enum.GetValues(typeof(TileColour));
        int index = UnityEngine.Random.Range(0, values.Length);

        return ToUnityColour((TileColour)values.GetValue(index));

    }

    public Colour ToUnityColour(TileColour tc)
    {
        return tc switch
        {
            TileColour.Red => Colour.red,
            TileColour.Green => Colour.green,
            TileColour.Blue => Colour.blue,
            TileColour.Yellow => Colour.yellow,
            TileColour.Cyan => Colour.cyan,
            TileColour.Magenta => Colour.magenta,
            _ => Colour.white
        };
    }

    private IEnumerator Simulate()
    {
        var interval = new WaitForSeconds(updateInterval);
        yield return interval;

        while (m_playing)
        {
            UpdateState();

            yield return interval;
        }
    }

    private void UpdateState()
    {

        cellsToCheck.Clear();
        CalculateCellsToCheck();

        foreach (Vector3Int cell in cellsToCheck)
        {
            int neighbours = CalculateNeighbours(cell);
            bool isAlive = CellIsAlive(cell);
            ComputeAlgorithm(neighbours, isAlive, cell);
        }

        SwapStates();
    }

    private void CalculateCellsToCheck()
    {
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
    }

    private int CalculateNeighbours(Vector3Int cell)
    {
        int neighbours = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int neighbour = cell + new Vector3Int(x, y);
                if (CheckingCurrentCell(x, y))
                {
                    continue;
                }
                if (CellIsAlive(neighbour))
                {
                    neighbours++;
                }
            }
        }

        return neighbours;
    }

    private void ComputeAlgorithm(int neighbours, bool isAlive, Vector3Int cell)
    {
        if ((neighbours < 2 || neighbours > 3) && isAlive)
        {
            nextState.SetTile(cell, jedTile);
            aliveCells.Remove(cell);

        }
        else if (!isAlive && neighbours == 3)
        {
            aliveTile.color = RandomTileColour();
            nextState.SetTile(cell, aliveTile);
            aliveCells.Add(cell);
        }
        else
        {
            nextState.SetTile(cell, currentState.GetTile(cell));
        }
    }

    private void SwapStates()
    {
        Tilemap temp = currentState;
        currentState = nextState;
        nextState = temp;
        nextState.ClearAllTiles();
    }

    public void Clear()
    {
        aliveCells.Clear();
        cellsToCheck.Clear();
        currentState.ClearAllTiles();
        nextState.ClearAllTiles();
    }

    private bool CellIsAlive(Vector3Int cell)
    {
        return currentState.GetTile(cell) == aliveTile;
    }

    private bool CheckingCurrentCell(int x, int y)
    {
        return x == 0 & y == 0;
    }

}