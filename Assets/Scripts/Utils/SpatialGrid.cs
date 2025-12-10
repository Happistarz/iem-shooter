using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpatialGrid<T> : IEnumerable<T>
{
    public T[] Data => _data;

    public Vector2 Min => _min;
    public Vector2 Max => _max;

    public int CountX => _countX;
    public int CountY => _countY;

    public Vector2  Center   => 0.5f * (_max + _min);
    public Vector2  Size     => _max - _min;
    public Bounds2D Bounds   => new(Min, Max);
    public Vector2  CellSize => new((_max.x - _min.x) / _countX, (_max.y - _min.y) / _countY);

    public IEnumerable<Point> Indices
    {
        get
        {
            for (var i = 0; i < _countX; i++)
                for (var j = 0; j < _countY; j++)
                    yield return new Point(i, j);
        }
    }

    public T this[Vector2 position]
    {
        get => Get(position);
        set => Set(ref value, position);
    }

    public T this[int i, int j]
    {
        get => Get(i, j);
        set => Set(ref value, i, j);
    }

    public T this[Point p]
    {
        get => Get(p.x, p.y);
        set => Set(ref value, p.x, p.y);
    }

    [HideInInspector] [SerializeField] private T[] _data;

    [SerializeField] private Vector2 _min, _max;

    [SerializeField] private int _countX, _countY;


    public SpatialGrid()
    {
        _data = Array.Empty<T>();
    }

    public SpatialGrid(Vector2 minCorner, Vector2 maxCorner, int nCellX, int nCellY)
    {
        _min = new Vector2(
            System.Math.Min(minCorner.x, maxCorner.x),
            System.Math.Min(minCorner.y, maxCorner.y));
        _max = new Vector2(
            System.Math.Max(minCorner.x, maxCorner.x),
            System.Math.Max(minCorner.y, maxCorner.y));

        _countX = nCellX;
        _countY = nCellY;

        _data = new T[_countX * _countY];
    }

    public SpatialGrid(float size, int nCellXY)
        : this(0.5f * size * -Vector2.one, 0.5f * size * Vector2.one, nCellXY, nCellXY)
    {
    }

    public ref T Get(int i, int j)
    {
        if (Contains(i, j) == false)
            throw new ArgumentOutOfRangeException($"Point {new Point(i, j)} is out of bound");

        var index = GetLinearIndex(i, j);
        return ref _data[index];
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
        _data[index] = value;
    }

    public void Set(ref T value, Vector2 position)
    {
        var point = GetCellIndex(position);
        Set(ref value, point.x, point.y);
    }

    public Point GetIndex2D(int i)
    {
        return new Point(i % _countX, i / _countX);
    }

    public int GetLinearIndex(int i, int j)
    {
        if (Contains(i, j) == false)
            throw new ArgumentOutOfRangeException();

        return j * _countX + i;
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
        var localX = position.x - _min.x;
        var localY = position.y - _min.y;

        if (CellSize.x == 0)
            return new Point(0, 0);
        if (CellSize.y == 0)
            return new Point(0, 0);

        var i = Mathf.FloorToInt(localX / CellSize.x);
        var j = Mathf.FloorToInt(localY / CellSize.y);

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
        position = Vector2.Max(_min, position);
        position = Vector2.Min(_max, position);

        var localX = position.x - _min.x;
        var localY = position.y - _min.y;

        if (CellSize.x == 0)
            return new Point(0, 0);
        if (CellSize.y == 0)
            return new Point(0, 0);

        var i = Mathf.FloorToInt(localX / CellSize.x);
        var j = Mathf.FloorToInt(localY / CellSize.y);

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
        if (p.x != 0) yield return new Point(p.x          - 1, p.y);
        if (p.x != CountX - 1) yield return new Point(p.x + 1, p.y);
        if (p.y != 0) yield return new Point(p.x,              p.y - 1);
        if (p.y != CountY - 1) yield return new Point(p.x,     p.y + 1);
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
        min.x = Mathf.Clamp(min.x, _min.x, _max.x);
        min.y = Mathf.Clamp(min.y, _min.y, _max.y);
        max.x = Mathf.Clamp(max.x, _min.x, _max.x);
        max.y = Mathf.Clamp(max.y, _min.y, _max.y);

        var minIdx = GetCellIndex(min);
        var maxIdx = GetCellIndex(max);

        for (var i = minIdx.x; i <= maxIdx.x; i++)
            for (var j = minIdx.y; j <= maxIdx.y; j++)
                yield return new Point(i, j);
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
        return position.x >= _min.x && position.x <= _max.x &&
               position.y >= _min.y && position.y <= _max.y;
    }

    public bool Contains(int i, int j)
    {
        return i >= 0 && i < _countX && j >= 0 && j < _countY;
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
        return ((IEnumerable<T>)_data).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _data.GetEnumerator();
    }
}