using System;
using System.Collections.Generic;


public class Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj is Point)
        {
            Point p = obj as Point;
            return this.X == p.X && this.Y == p.Y;
        }
        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 6949;
            hash = hash * 7907 + X.GetHashCode();
            hash = hash * 7907 + Y.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        return "P(" + this.X + ", " + this.Y + ")";
    }
}

public enum CellType
{
    Empty,
    Road,
    Structure,
    SpecialStructure,
    Ground,
    None
}

public class GameGrid
{
    private CellType[,] _grid;
    private int _width;
    public int Width { get { return _width; } }
    private int _height;
    public int Height { get { return _height; } }

    private List<Point> _roadList = new List<Point>();
    private List<Point> _specialStructure = new List<Point>();

    public GameGrid(int width, int height)
    {

        _width = width;
        _height = height;
        _grid = new CellType[width, height];

    }

    // Adding index operator to our Grid class so that we can use grid[][] to access specific cell from our grid. 
    public CellType this[int i, int j]
    {
        get
        {
            return _grid[i, j];
        }
        set
        {
            if (value == CellType.Road)
            {
                _roadList.Add(new Point(i, j));
            }
            else
            {
                _roadList.Remove(new Point(i, j));
            }
            if (value == CellType.SpecialStructure)
            {
                _specialStructure.Add(new Point(i, j));
            }
            else
            {
                _specialStructure.Remove(new Point(i, j));
            }
            _grid[i, j] = value;
        }
    }

    public static bool IsCellWalkable(CellType cellType, bool aiAgent = false)
    {
        if (aiAgent)
        {
            return cellType == CellType.Road;
        }
        return cellType == CellType.Empty || cellType == CellType.Road;
    }

    public Point GetRandomRoadPoint()
    {
        Random rand = new Random();
        return _roadList[rand.Next(0, _roadList.Count - 1)];
    }

    public Point GetRandomSpecialStructurePoint()
    {
        Random rand = new Random();
        return _roadList[rand.Next(0, _roadList.Count - 1)];
    }

    public List<Point> GetAdjacentCells(Point cell, bool isAgent)
    {
        return GetWakableAdjacentCells((int)cell.X, (int)cell.Y, isAgent);
    }

    public float GetCostOfEnteringCell(Point cell)
    {
        return 1;
    }

    public List<Point> GetAllAdjacentCells(int x, int y)
    {
        List<Point> adjacentCells = new List<Point>();
        if (x > 0)
        {
            adjacentCells.Add(new Point(x - 1, y));
        }
        if (x < _width - 1)
        {
            adjacentCells.Add(new Point(x + 1, y));
        }
        if (y > 0)
        {
            adjacentCells.Add(new Point(x, y - 1));
        }
        if (y < _height - 1)
        {
            adjacentCells.Add(new Point(x, y + 1));
        }
        return adjacentCells;
    }

    //public List<Point> GetAllAdjacentCells(int x, int y)
    //{
    //    List<Point> adjacentCells = new List<Point>();

    //    // Define possible moves including diagonals
    //    int[] dx = { -1, 0, 1, 0, -1, -1, 1, 1 };
    //    int[] dy = { 0, -1, 0, 1, -1, 1, -1, 1 };

    //    for (int dir = 0; dir < 8; dir++) // loop over all possible directions
    //    {
    //        int newX = x + dx[dir];
    //        int newY = y + dy[dir];

    //        if (newX >= 0 && newX < _width && newY >= 0 && newY < _height)
    //        {
    //            adjacentCells.Add(new Point(newX, newY));
    //        }
    //    }

    //    return adjacentCells;
    //}


    public List<Point> GetWakableAdjacentCells(int x, int y, bool isAgent)
    {
        List<Point> adjacentCells = GetAllAdjacentCells(x, y);
        for (int i = adjacentCells.Count - 1; i >= 0; i--)
        {
            if (IsCellWalkable(_grid[adjacentCells[i].X, adjacentCells[i].Y], isAgent) == false)
            {
                adjacentCells.RemoveAt(i);
            }
        }
        return adjacentCells;
    }

    public List<Point> GetAdjacentCellsOfType(int x, int y, CellType type)
    {
        List<Point> adjacentCells = GetAllAdjacentCells(x, y);
        for (int i = adjacentCells.Count - 1; i >= 0; i--)
        {
            if (_grid[adjacentCells[i].X, adjacentCells[i].Y] != type)
            {
                adjacentCells.RemoveAt(i);
            }
        }
        return adjacentCells;
    }

    public CellType GetCellOf(int x, int y)
    {
        return _grid[x, y];
    }

    /// <summary>
    /// Returns array [Left neighbour, Top neighbour, Right neighbour, Down neighbour]
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public CellType[] GetAllAdjacentCellTypes(int x, int y)
    {
        CellType[] neighbours = { CellType.None, CellType.None, CellType.None, CellType.None };
        if (x > 0)
        {
            neighbours[0] = _grid[x - 1, y];
        }
        if (x < _width - 1)
        {
            neighbours[2] = _grid[x + 1, y];
        }
        if (y > 0)
        {
            neighbours[3] = _grid[x, y - 1];
        }
        if (y < _height - 1)
        {
            neighbours[1] = _grid[x, y + 1];
        }
        return neighbours;
    }
}