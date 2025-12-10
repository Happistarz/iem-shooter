using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpatialGrid<T> : IEnumerable<T>
{
    public T[] Data => m_data;

    public Vector2 Min => m_min;
    public Vector2 Max => m_max;

    public int CountX => m_countX;
    public int CountY => m_countY;

    public Vector2 Center => 0.5f * (m_max + m_min); 
    public Vector2 Size => m_max - m_min;
    public Bounds2D Bounds => new Bounds2D(Min, Max);
    public Vector2 CellSize => new Vector2((m_max.x - m_min.x) / (m_countX), (m_max.y - m_min.y) / (m_countY));

    public IEnumerable<Point> Indices {
        get {
            for (var i = 0; i < m_countX; i++) for (var j = 0; j < m_countY; j++) yield return new Point(i, j);
        }
    }

    public T this[Vector2 position] {
        get => Get(position);
        set => Set(ref value, position);
    }

    public T this[int i, int j] {
        get => Get(i, j);
        set => Set(ref value, i, j);
    }

    public T this[Point p] {
        get => Get(p.x, p.y);
        set => Set(ref value, p.x, p.y);
    }

    [HideInInspector]
    [SerializeField]
    T[] m_data;

    [SerializeField]
    Vector2 m_min, m_max;

    [SerializeField]
    int m_countX, m_countY;


    public SpatialGrid()
    {
        m_data = new T[0];
    }

    public SpatialGrid(Vector2 minCorner, Vector2 maxCorner, int nCellX, int nCellY)
    {
        m_min = new Vector2(
            System.Math.Min(minCorner.x, maxCorner.x),
            System.Math.Min(minCorner.y, maxCorner.y));
        m_max = new Vector2(
            System.Math.Max(minCorner.x, maxCorner.x),
            System.Math.Max(minCorner.y, maxCorner.y));

        m_countX = nCellX;
        m_countY = nCellY;

        m_data = new T[m_countX * m_countY];
    }
        
    public SpatialGrid( float size, int nCellXY ) 
        : this(0.5f * size * -Vector2.one, 0.5f * size * Vector2.one, nCellXY, nCellXY)
    {
    }

    public ref T Get(int i, int j)
    {
        if (Contains(i, j) == false)
            throw new ArgumentOutOfRangeException( $"Point {new Point( i, j )} is out of bound" );

        var index = GetLinearIndex(i, j);
        return ref m_data[index];
    }

    public ref T Get(Vector2 position)
    {
        var point = GetCellIndex(position);
        return ref Get(point.x, point.y);
    }

    public void Set(ref T value, int i, int j)
    {
        if (Contains(i, j) == false)
            throw new ArgumentOutOfRangeException();

        var index = GetLinearIndex(i, j);
        m_data[index] = value;
    }

    public void Set(ref T value, Vector2 position)
    {
        var point = GetCellIndex(position);
        Set(ref value, point.x, point.y);
    }

    public Point GetIndex2D(int i)
    {
        return new Point(i % m_countX, i / m_countX);
    }

    public int GetLinearIndex(int i, int j)
    {
        if (Contains(i, j) == false)
            throw new ArgumentOutOfRangeException();

        return j * m_countX + i;
    }

    public int GetLinearIndex(Point p)
    {
        return GetLinearIndex(p.x, p.y);
    }

    public int GetLinearIndex(Vector2 position)
    {
        var point = GetCellIndex(position);
        return GetLinearIndex(point.x, point.y);
    }

    public Point GetCellIndex(Vector2 position)
    {
        var localX = position.x - m_min.x;
        var localY = position.y - m_min.y;

        if (CellSize.x == 0)
            return new Point(0, 0);
        if (CellSize.y == 0)
            return new Point(0, 0);

        var i = (int)Mathf.FloorToInt(localX / CellSize.x);
        var j = (int)Mathf.FloorToInt(localY / CellSize.y);

        if (i == CountX)
            i--;

        if (j == CountY)
            j--;

        if (Contains(i, j) == false)
            throw new ArgumentOutOfRangeException();

        return new Point(i, j);
    }

    public Point GetCellIndexBounded(Vector2 position)
    {
        position = Vector2.Max(m_min, position);
        position = Vector2.Min(m_max, position);

        var localX = position.x - m_min.x;
        var localY = position.y - m_min.y;

        if (CellSize.x == 0)
            return new Point(0, 0);
        if (CellSize.y == 0)
            return new Point(0, 0);

        var i = (int)Mathf.FloorToInt(localX / CellSize.x);
        var j = (int)Mathf.FloorToInt(localY / CellSize.y);

        if (i == CountX)
            i--;

        if (j == CountY)
            j--;

        if (Contains(i, j) == false)
            throw new ArgumentOutOfRangeException();

        return new Point(i, j);
    }

    public IEnumerable<Point> GetNeighbours4(Point p)
    {
        if (p.x != 0) yield return new Point(p.x - 1, p.y);
        if (p.x != CountX - 1) yield return new Point(p.x + 1, p.y);
        if (p.y != 0) yield return new Point(p.x, p.y - 1);
        if (p.y != CountY - 1) yield return new Point(p.x, p.y + 1);
    }

    public IEnumerable<Point> GetNeighbours8(Point p)
    {
        for (var i = -1; i < 2; i++)
        {
            for (var j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0) continue;
                if (Contains(p.x + i, p.y + j)) yield return new Point(p.x + i, p.y + j);
            }
        }
    }

    public IEnumerable<Point> GetNeighbours24(Point p)
    {
        for (var i = -2; i <= 2; i++)
        {
            for (var j = -2; j <= 2; j++)
            {
                if (i == 0 && j == 0) continue;
                if (Contains(p.x + i,
                        p.y + j)) yield return new Point(p.x + i, p.y + j);
            }
        }
    }

    public IEnumerable<Point> GetIndices(Vector2 min, Vector2 max)
    {
        min.x = Mathf.Clamp(min.x, m_min.x, m_max.x);
        min.y = Mathf.Clamp(min.y, m_min.y, m_max.y);
        max.x = Mathf.Clamp(max.x, m_min.x, m_max.x);
        max.y = Mathf.Clamp(max.y, m_min.y, m_max.y);

        var minIdx = GetCellIndex(min);
        var maxIdx = GetCellIndex(max);

        for (var i = minIdx.x; i <= maxIdx.x; i++)
        for (var j = minIdx.y; j <= maxIdx.y; j++) yield return new Point(i, j);
    }

    public Vector2 GetCellCenter(Point p)
    {
        return GetCellCenter(p.x, p.y);
    }

    public Vector2 GetCellCenter(int i, int j)
    {
        return new Vector2(
            Min.x + CellSize.x * (0.5f + i),
            Min.y + CellSize.y * (0.5f + j));
    }

    public bool Contains(Vector2 position)
    {
        return position.x >= m_min.x && position.x <= m_max.x &&
               position.y >= m_min.y && position.y <= m_max.y;
    }

    public bool Contains(int i, int j)
    {
        return i >= 0 && i < m_countX && j >= 0 && j < m_countY;
    }

    public bool Contains(Point p)
    {
        return Contains(p.x, p.y);
    }

    public static SpatialGrid<T> CreateCopy<TSource>(SpatialGrid<TSource> source)
    {
        return new SpatialGrid<T>(source.Min, source.Max, source.CountX, source.CountY);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)m_data).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return m_data.GetEnumerator();
    }
}