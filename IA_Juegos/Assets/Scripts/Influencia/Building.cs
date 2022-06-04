using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour
{
    private string _name;
    [SerializeField]
    private List<Vector2Int> corners;
    private List<Vector2Int> _cells;
    [SerializeField]
    private Teams team;

    private Vector2Int _center;
    [SerializeField]
    private int _capturePoints;

    public List<Vector2Int> Cells
    {
        get { return new List<Vector2Int>(_cells); }
    }

    public Vector2Int Center
    {
        get { return _center; }
    }

    public Teams Team
    {
        get { return team; }
        set { team = value; }
    }

    private void Awake()
    {
        _name = transform.name;
        _cells = CornersToCells();
        _center = CalculateCenter();
        _capturePoints = 0;
    }

    private void Update()
    {
        if (_capturePoints >= 100)
        {
            GameObject.Find("GameController").GetComponent<GameController>().EndGame(team);
        }
    }

    private List<Vector2Int> CornersToCells()
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        if (corners.Count == 1)
        { 
            cells.Add(corners[0]);
            return cells;
        }

        Vector2Int leftDownCorner = corners[0];
        Vector2Int topRightCorner = corners[1];

        for (int i = leftDownCorner.x; i <= topRightCorner.x; i++)
        {
            for (int j = leftDownCorner.y; j <= topRightCorner.y; j++)
            {
                cells.Add(new Vector2Int(i, j));
            }
        }

        return cells;
    }

    private Vector2Int CalculateCenter()
    {
        if (_cells.Count == 1)
        {
            return _cells[0];
        }
        
        Vector2Int leftDownCorner = corners[0];
        Vector2Int topRightCorner = corners[1];

        return leftDownCorner + (topRightCorner - leftDownCorner) / 2;
    }

    public void GetCapturePoints(int points)
    {
        _capturePoints += points;
    }
}
