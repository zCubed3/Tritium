using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;



namespace Tritium.Concepts.Grid
{
    // A GridEntity is a bit of a special entity
    // It is an entity that defines a "walkable" area
    // GridEntity's have 2 primary child types
    //  Entity (for dynamic things)
    //  TileEntity (for static things)
    // We don't discriminate against any Entity type when storing them!
    //
    // Things that move on the "grid" transfer onto it by stepping onto the tiles occupying it
    // TODO: Grid transfers
    // TODO: Grid physics
    // TODO: OPTIMIZE THE FUCKING GRID
    //  We can do this through a compound grid entity!
    public class GridEntity : Entity
    {
        // We need to store a list of where the tiles are
        // This partly circumvents the normal parenting system
        // Because the ship can have awkward shapes the tiles themselves have weird shapes
        // Rows are linear lists but have an offset applied to help account for weird shapes
        // This allows for ships shaped like this for instance...
        //
        // XXXXX
        //   X
        //   X
        //   XXX
        //
        // Connectivity is done based on rows phoning to adjacent rows asking if there are connecting tiles
        public class GridRow
        {
            // Shift = min
            public int min = 0;
            public List<Dictionary<TileImpl.TileLayer, TileEntity>> rowTiles = new List<Dictionary<TileImpl.TileLayer, TileEntity>>();

            // TODO: Layer checking
            // TODO: Sanity checking coordinates
            // TODO: Make sure floors exist before walls?
            public bool IsHorizontalOrphaned(int x, TileImpl.TileLayer layer)
            {
                if (TryGetTile(x - 1, layer, out TileEntity left))
                    if (left != null)
                        return false;

                if (TryGetTile(x + 1, layer, out TileEntity right))
                    if (right != null)
                        return false;

                return true;
            }

            public bool IsVerticalOrphaned(GridEntity parent, int x, int y, TileImpl.TileLayer layer)
            {
                if (parent.TryGetRow(y + 1, out GridRow above))
                    if (above != null)
                        if (above.TryGetTile(x, layer, out TileEntity otherEnt))
                            if (otherEnt != null)
                                return false;

                if (parent.TryGetRow(y - 1, out GridRow below))
                    if (below != null)
                        if (below.TryGetTile(x, layer, out TileEntity otherEnt))
                            if (otherEnt != null)
                                return false;

                return true;
            }

            /// <summary>
            /// Returns the surrounding tiles, in the order of "N E S W"
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="layer"></param>
            /// <param name="corners"></param>
            /// <returns></returns>
            public TileEntity[] GetNeighbors(GridEntity parent, int x, int y, TileImpl.TileLayer layer, bool corners = false)
            {
                TileEntity[] neighbors = corners ? new TileEntity[8] : new TileEntity[4];

                parent.TryGetTile(x, y - 1, layer, out neighbors[0]);
                TryGetTile(x - 1, layer, out neighbors[1]);
                parent.TryGetTile(x, y + 1, layer, out neighbors[2]);
                TryGetTile(x + 1, layer, out neighbors[3]);

                if (corners)
                {
                    parent.TryGetTile(x - 1, y - 1, layer, out neighbors[4]);
                    parent.TryGetTile(x - 1, y + 1, layer, out neighbors[5]);
                    parent.TryGetTile(x + 1, y - 1, layer, out neighbors[6]);
                    parent.TryGetTile(x + 1, y + 1, layer, out neighbors[7]);
                }

                return neighbors;
            }

            // TODO: Possibly compress the tiles a bit?
            // Do this by using a linked list instead?
            // We'd be able to store offsets instead and have a virtual coordinate system
            // Ex:
            //
            // X X X
            // XXXXX
            //
            // In memory would look like
            //
            // XXX
            // XXXXX
            //
            // But the tiles would know there's a gap
            // So if I insert a new tile between two of the prongs, it inserts into the list between them and fills the gap!

            public void ConnectWalls(GridEntity parent, int y)
            {
                for (int x = 0; x < rowTiles.Count; x++)
                    ConnectWall(parent, x + min, y);
            }

            public void ConnectWall(GridEntity parent, int x, int y)
            {
                var neighbors = GetNeighbors(parent, x, y, TileImpl.TileLayer.Wall, false);

                // TODO: Better way to do this?
                // TODO: Tiles that can't connect to each other?

                if (!ValidCoord(x))
                    return;

                if (rowTiles[x - min] != null)
                    if (rowTiles[x - min].TryGetValue(TileImpl.TileLayer.Wall, out TileEntity entity))
                        entity.ChangeAppearance(
                            neighbors[0] != null,
                            neighbors[1] != null,
                            neighbors[2] != null,
                            neighbors[3] != null
                        );
            }

            public bool TryAddTile(TileEntity entity, GridEntity parent, int x, int y)
            {
                // We need to make sure this is connected to something beforehand
                // If it has some form of connection we also need to add blank tiles before this if it needs it!
                // The only special case is if the row is completely empty, we're allowed to insert anywhere on the X axis

                // Is the tile occupied?
                if (TryGetTile(x, entity.tileImpl.layer, out TileEntity tile))
                    if (tile != null)
                        return false;

                // TODO: Underfloor and walls
                TileImpl.TileLayer seekLayer = TileImpl.TileLayer.Floor;

                bool isOrphaned = IsHorizontalOrphaned(x, seekLayer) && IsVerticalOrphaned(parent, x, y, seekLayer);

                // TODO: Better method?
                if (x == 0 && y == 0 && isOrphaned)
                    isOrphaned = false;

                if (isOrphaned)
                    return false;
                else
                {
                    // If X is below shift but not orphaned, we need to prepend padding
                    int shiftDiff = x - min;
                    if (shiftDiff < 0)
                    {
                        while (shiftDiff++ < 0)
                            rowTiles.Insert(0, null);

                        min = x;
                    }
                    else
                    {
                        // If this is empty when we are setting up the index we need to shift forwards
                        if (rowTiles.Count <= 0)
                        {
                            min = x;
                            shiftDiff = x - min;
                        }

                        while (shiftDiff >= rowTiles.Count)
                            rowTiles.Add(null);
                    }

                    if (rowTiles[x - min] == null)
                        rowTiles[x - min] = new Dictionary<TileImpl.TileLayer, TileEntity>();

                    rowTiles[x - min][entity.tileImpl.layer] = entity;

                    if (entity.tileImpl.layer == TileImpl.TileLayer.Wall)
                    {
                        ConnectWall(parent, x, y);
                        ConnectWall(parent, x - 1, y);
                        ConnectWall(parent, x + 1, y);
                        ConnectWall(parent, x, y - 1);
                        ConnectWall(parent, x, y + 1);
                    }

                    return true;
                }
            }

            // TODO: Safety?
            public bool TryGetTile(int x, TileImpl.TileLayer layer, out TileEntity tile) {
                int index = x - min;

                if (!(index < 0 || index >= rowTiles.Count)) 
                {
                    var tiles = rowTiles[index];
                    if (tiles != null)
                        return tiles.TryGetValue(layer, out tile);
                }

                tile = null;
                return false;
            }

            public bool ValidCoord(int x ) => x >= min && x < rowTiles.Count + min;
        }

        // Defines a closed area on the grid
        public class GridEnclosure
        {
            public List<Vector2Int> containedPoints = new List<Vector2Int>();
            public AABB enclosingBox;

            public bool ContainsPoint(Vector2Int point) => containedPoints.Contains(point);

            public void CalculateAABB() 
            { 
            
            }
        }

        // This list can shift up and down!
        public int rowShift = 0;
        public List<GridRow> rows = new List<GridRow>();
        public List<GridEnclosure> enclosures = new List<GridEnclosure>();

        // TODO: Compound bounding boxes?
        public AABB shipAABB = new AABB(Vector2.Zero, Vector2.Zero);

        public virtual void CalculateBounds()
        {
            Vector2 min = Vector2.Zero;
            Vector2 max = Vector2.Zero;

            // We need to get the furthest corners
            if (rows.Count > 0)
            {
                min.Y = rowShift;
                max.Y = rows.Count - 1 - rowShift;

                // We need to scan every row to find the largest ones
                foreach (GridRow row in rows)
                {
                    int x = row.min;
                    int farX = x + row.rowTiles.Count - 1;

                    if (min.X > x)
                        min.X = x;

                    if (max.X < farX)
                        max.X = farX;
                }

                min -= Vector2.One / 2;
                max += Vector2.One / 2;
            }

            shipAABB = new AABB(min, max);
        }

        // TODO: For the love of god optimize this
        public virtual void CalculateEnclosures(Vector2Int? start = null)
        {
            // First we need all the floors
            // TODO: Floor permeability?
            List<TileEntity> floors = new List<TileEntity>();
            List<Vector2Int> floorCoords = new List<Vector2Int>();

            // Once we have a wall we look at all connected children surrounding this
            // We can't back seek so we keep a record of the previous direction we found a child in
            Queue<Vector2Int> coords = new Queue<Vector2Int>();
            List<Vector2Int> searchedCoords = new List<Vector2Int>();

            // Brute force finding a spot with a floor and not a wall
            // Only if we don't give it a start value
            // Walls NEED to be connected
            while (start == null)
            {
                int y = rowShift;
                foreach (GridRow row in rows)
                {
                    int x = row.min;
                    foreach (var tileDict in row.rowTiles)
                    {
                        bool hasWall = false;
                        foreach (var tilePair in tileDict) 
                        {
                            if (tilePair.Key == TileImpl.TileLayer.Wall && tilePair.Value != null)
                                hasWall = true;

                            if (tilePair.Key == TileImpl.TileLayer.Floor && tilePair.Value != null)
                                start = new Vector2Int(x, y);
                        }

                        if (hasWall)
                            start = null;

                        if (start.HasValue)
                            break;

                        x++;
                    }

                    if (start.HasValue)
                        break;

                    y++;
                }
            }

            coords.Enqueue(start.Value);

            // TODO: We do a flood fill, could this be replaced?
            List<GridEnclosure> dupeEnclosures = new List<GridEnclosure>();
            while (coords.Count > 0)
            {
                Vector2Int coord = coords.Dequeue();

                if (searchedCoords.Contains(coord))
                    continue;

                if (TryGetTile(coord.X, coord.Y, TileImpl.TileLayer.Floor, out TileEntity ent)
                    && !TryGetTile(coord.X, coord.Y, TileImpl.TileLayer.Wall, out TileEntity _)
                    && shipAABB.ContainsPoint(new Vector2(coord.X, coord.Y)))
                {
                    if (ent != null)
                    {
                        if (!floors.Contains(ent))
                        {
                            floors.Add(ent);
                            floorCoords.Add(coord);

                            foreach (GridEnclosure enclosure in enclosures)
                                if (!dupeEnclosures.Contains(enclosure) && enclosure.ContainsPoint(coord))
                                    dupeEnclosures.Add(enclosure);

                            coords.Enqueue(new Vector2Int(coord.X - 1, coord.Y));
                            coords.Enqueue(new Vector2Int(coord.X + 1, coord.Y));
                            coords.Enqueue(new Vector2Int(coord.X, coord.Y - 1));
                            coords.Enqueue(new Vector2Int(coord.X, coord.Y + 1));
                        }
                    }
                }

                searchedCoords.Add(coord);
            }

            //TritiumGame.Logger.Log($"Merging {dupeEnclosures.Count} enclosures!");

            foreach (GridEnclosure dupe in dupeEnclosures)
                enclosures.Remove(dupe);

            GridEnclosure targetEnclosure = new GridEnclosure();
            targetEnclosure.containedPoints = floorCoords;
            enclosures.Add(targetEnclosure);

            searchedCoords.Clear();
            floors.Clear();
            coords.Clear();
        }

        // Very expensive, call sparingly!
        public virtual void ConnectAllTiles()
        {
            int y = rowShift;
            foreach (GridRow row in rows)
            {
                row.ConnectWalls(this, y);
                y++;
            }
        }

        // TODO: Orphan checking (eg: not connected)
        // TODO: Make orphans become new grids
        public bool TryAddTile(TileEntity tile, int x, int y, bool update = true)
        {
            // TODO: Layer checking
            if (TryGetRow(y, out GridRow testRow))
                if (testRow != null)
                    if (testRow.TryGetTile(x, tile.tileImpl.layer, out TileEntity otherTile))
                        if (otherTile != null)
                            return false;

            // TODO: Too far
            if (y < rowShift - 1 || y - rowShift >= rows.Count + 1)
                return false;

            if (y < rowShift)
            {
                for (int c = 0; c < rowShift - y; c++)
                    rows.Insert(0, new GridRow());

                rowShift = y;
            }

            if (y - rowShift >= rows.Count)
                while (y - rowShift >= rows.Count)
                    rows.Add(new GridRow());

            GridRow row = rows[y - rowShift];

            if (row.TryAddTile(tile, this, x, y))
            {
                tile.Position = new Vector2(x, y);
                Parent(tile);

                if (update)
                {
                    CalculateBounds();
                    CalculateEnclosures(new Vector2Int(x, y));
                    ConnectAllTiles();
                }

                return true;
            }

            return false;
        }

        public bool TryGetRow(int y, out GridRow row)
        {
            int index = y - rowShift;

            if (index < 0 || index >= rows.Count)
            {
                row = null;
                return false;
            }
            else
            {
                row = rows[index];
                return true;
            }
        }

        public bool TryGetTile(int x, int y, TileImpl.TileLayer layer, out TileEntity entity)
        {
            if (TryGetRow(y, out GridRow row))
                if (row != null)
                    if (row.TryGetTile(x, layer, out TileEntity tile))
                    {
                        entity = tile;
                        return true;
                    }

            entity = null;
            return false;
        }
    }
}
