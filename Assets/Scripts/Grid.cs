using System;
using System.Collections.Generic;
using EditorCools;
using UnityEngine;

// _     __________    _    ______   __   ____ ___  ____  _____           
//| |   | ____/ ___|  / \  / ___\ \ / /  / ___/ _ \|  _ \| ____|          
//| |   |  _|| |  _  / _ \| |    \ V /  | |  | | | | | | |  _|            
//| |___| |__| |_| |/ ___ | |___  | |   | |__| |_| | |_| | |___           
//|_____|_____\____/_/   \_\____| |_|    \____\___/|____/|_____|          


// ____   ___    _   _  ___ _____    ____ _   _    _    _   _  ____ _____ 
//|  _ \ / _ \  | \ | |/ _ |_   _|  / ___| | | |  / \  | \ | |/ ___| ____|
//| | | | | | | |  \| | | | || |   | |   | |_| | / _ \ |  \| | |  _|  _|  
//| |_| | |_| | | |\  | |_| || |   | |___|  _  |/ ___ \| |\  | |_| | |___ 
//|____/ \___/  |_| \_|\___/ |_|    \____|_| |_/_/   \_|_| \_|\____|_____|

public class Grid : MonoBehaviour
{
    [SerializeField] private int Width;
    [SerializeField] private int Height;

    [Range(0.0f, 5.0f)] [SerializeField] private float Spacing;

    [Range(0.0f, 1.0f)] [SerializeField] private float VisualTileSize;

    [Range(0.0f, 1.0f)] [SerializeField] private float AlphaTileSize;

    public bool GeneratedGrid;

    [SerializeField] private Transform FinishTileTrans;

    public List<Tile> tiles = new();

    private void OnDrawGizmos()
    {
        if (GeneratedGrid)
            foreach (var t in tiles)
            {
                if (t.occupied) Gizmos.color = Color.red;
                else Gizmos.color = Color.green;
                if (t.finishTile) Gizmos.color = Color.blue;

                AlphaColor();
                var cubeSize = new Vector3(Spacing * VisualTileSize, 0.1f, Spacing * VisualTileSize);


                var cubePos = new Vector3();

                cubePos.x = t.x * Spacing + Spacing / 2.0f;
                cubePos.z = t.y * Spacing + Spacing / 2.0f;

                Gizmos.DrawWireCube(cubePos + OffsetPos(), cubeSize);
            }
        else
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                Gizmos.color = Color.yellow;
                AlphaColor();

                var cubeSize = new Vector3(Spacing * VisualTileSize, 0.1f, Spacing * VisualTileSize);
                var cubePos = new Vector3();

                cubePos.x = x * Spacing + Spacing / 2.0f;
                cubePos.z = y * Spacing + Spacing / 2.0f;

                Gizmos.DrawWireCube(cubePos + OffsetPos(), cubeSize);
            }
    }

    [Button]
    private void BakeGrid()
    {
        ClearGrid();

        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
        {
            var t = new Tile();
            t.x = x;
            t.y = y;
            t.occupied = false;

            var cubeSize = new Vector3(Spacing, 0.1f, Spacing);
            var cubePos = new Vector3();

            cubePos.x = x * Spacing + Spacing / 2.0f;
            cubePos.z = y * Spacing + Spacing / 2.0f;

            cubeSize /= 2;

            var cols = Physics.OverlapBox(cubePos + OffsetPos(), cubeSize);
            if (cols.Length > 0)
                foreach (var c in cols)
                    if (c.transform.CompareTag("Obstacle"))
                    {
                        t.occupied = true;
                        break;
                    }

            tiles.Add(t);
        }

        GetClosest(FinishTileTrans.position).finishTile = true;

        GeneratedGrid = true;
    }

    public Tile GetClosest(Tile aTile)
    {
        var dist = float.MaxValue;

        var aPosition = WorldPos(aTile);

        Tile returnTile = null;
        foreach (var t in tiles)
        {
            var curDist = Vector3.Distance(aPosition, WorldPos(t));
            if (curDist < dist && !t.occupied && !IsSameTile(t, aTile))
            {
                dist = curDist;
                returnTile = t;
            }
        }

        return returnTile;
    }

    public Tile TryGetTile(Vector2Int aPos)
    {
        Tile returnTile = null;

        foreach (var t in tiles)
            if (t.x == aPos.x && t.y == aPos.y)
            {
                returnTile = t;
                break;
            }

        return returnTile;
    }

    public Tile GetFinishTile()
    {
        foreach (var t in tiles)
            if (t.finishTile)
                return t;

        return null;
    }

    public List<Tile> GetTiles()
    {
        return tiles;
    }


    public Tile GetClosest(Vector3 aPosition)
    {
        var dist = float.MaxValue;
        Tile returnTile = null;
        foreach (var t in tiles)
        {
            var curDist = Vector3.Distance(aPosition, WorldPos(t));
            if (curDist < dist && !t.occupied)
            {
                dist = curDist;
                returnTile = t;
            }
        }

        return returnTile;
    }

    [Button]
    private void ClearGrid()
    {
        GeneratedGrid = false;
        tiles.Clear();
    }

    public bool isReachable(Tile from, Tile to)
    {
        var fromPos = WorldPos(from);
        var toPos = WorldPos(to);

        return Vector3.Distance(fromPos, toPos) < Spacing * MathF.Sqrt(2.1f);
    }

    public Vector3 WorldPos(Tile aTile)
    {
        var offset = new Vector3();

        offset.x = transform.position.x - Spacing * Width / 2;
        offset.z = transform.position.z - Spacing * Height / 2;
        offset.y = transform.position.y;

        var world = new Vector3();

        world.x = aTile.x * Spacing + Spacing / 2.0f;
        world.z = aTile.y * Spacing + Spacing / 2.0f;
        world.y = 0;

        return world + offset;
    }

    private Vector3 OffsetPos()
    {
        var offset = new Vector3();

        offset.x = transform.position.x - Spacing * Width / 2;
        offset.z = transform.position.z - Spacing * Height / 2;
        offset.y = transform.position.y;

        return offset;
    }

    public bool IsSameTile(Tile aFirst, Tile aSecond)
    {
        if (aFirst.x == aSecond.x && aFirst.y == aSecond.y) return true;
        return false;
    }

    private void AlphaColor()
    {
        var gcolor = Gizmos.color;
        gcolor.a = AlphaTileSize;
        Gizmos.color = gcolor;
    }

    public Vector3 GetWinPos()
    {
        return FinishTileTrans.position;
    }

    public List<Tile> GetNeighbours(Tile tile)
    {
        var neighbours = new List<Tile>();

        for (var x = -1; x <= 1; x++)
        for (var y = -1; y <= 1; y++)
        {
            if (x == 0 && y == 0)
                continue;

            var checkX = tile.x + x;
            var checkY = tile.y + y;

            if (checkX >= 0 && checkX < Width && checkY >= 0 && checkY < Height)
                neighbours.Add(TryGetTile(new Vector2Int(checkX, checkY)));
        }

        return neighbours;
    }

    [Serializable]
    public class Tile
    {
        public int x, y;
        public bool occupied;
        public bool finishTile;
    }

    #region Singleton

    public static Grid Instance;

    private void Awake()
    {
        if (Instance)
            Debug.LogError("Grid already exists");
        else
            Instance = this;
    }

    #endregion
}