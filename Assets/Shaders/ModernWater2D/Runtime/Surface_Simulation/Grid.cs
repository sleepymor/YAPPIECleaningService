using UnityEngine;

namespace Water2D
{
    // Grid class used for chunking or space partitioning
    // Last Update : 31.08.2023, Version: 1.0
    // Creator: Leafousio
    //
    // Notes: 
    //
    // Grid is indexed from topLeft to bottomRight
    // Use Functions in order to update grid position and subdivision
    //
    // End
    public class Grid 
    {
        public int cellsX { get; private set; }
        public int cellsY { get; private set; }

        public int cellsTotal { get => cellsY * cellsX; }

        public float width { get; private set; }
        public float height { get; private set; }

        public float cellWidth { get; private set; }
        public float cellHeight { get; private set; }

        public Vector2 bl { get; private set; }
        public Vector2 tr { get; private set; }

        public Grid(int cellsX, int cellsY, float width, float height, Vector2 bl)
        {
            this.cellsX = cellsX;
            this.cellsY = cellsY;
            this.width = width;
            this.height = height;
            this.bl = bl;
            this.tr = bl + new Vector2(width, height);
            this.cellWidth = this.width / this.cellsX;
            this.cellHeight = this.height / this.cellsY;
        }

        public void UpdatePosition(Vector2 bottomLeft)
        {
            this.bl = bottomLeft;
            this.tr = bottomLeft + new Vector2(width, height);
        }

        public void UpdateCellCount(int cellsX, int cellsY)
        {
            this.cellsX = cellsX;
            this.cellsY = cellsY;
            this.cellWidth = this.width / this.cellsX;
            this.cellHeight = this.height / this.cellsY;
        }

        public bool RectInCell(Rect rect, int cellIdx)
        {
            //generate chunk rect
            Vector2Int xy = to2D(cellIdx);
            Rect chunkRect = new Rect(bl.x + (xy.x * cellWidth), tr.y - (xy.y * cellHeight), cellWidth, cellHeight);
            return rect.Overlaps(chunkRect);
        }

        public bool RectInGrid(Rect rect, int cellIdx)
        {
            Rect gridRect = new Rect(bl.x , tr.y , width, height);
            return rect.Overlaps(gridRect);
        }

        public void DrawGizmos() 
        {
            Gizmos.color = Color.green;
            for (int x = 0; x < cellsX + 1; x++) Gizmos.DrawLine(new Vector3(bl.x + (x * cellWidth), bl.y), new Vector3(bl.x + (x * cellWidth), tr.y));
            for (int y = 0; y < cellsY + 1; y++) Gizmos.DrawLine(new Vector3(bl.x, bl.y + (y * cellHeight)), new Vector3(tr.x, bl.y + (y * cellHeight)));
        }

        /// <summary>
        /// returns (-1,-1) if not in any cell, function is inclusive
        /// </summary>
        public Vector2Int GetGridCellCoords(Vector2 pos)
        {
            //boundary conditions
            if (pos.x < bl.x || pos.x > tr.x) return new Vector2Int(-1,-1);
            if (pos.y < bl.y || pos.y > tr.y) return new Vector2Int(-1, -1);

            pos.x -= bl.x;
            pos.y -= bl.y;

            int x = Mathf.FloorToInt(pos.x/cellWidth);
            int y = (cellsY-1) - Mathf.FloorToInt(pos.y/cellWidth); //reverse

            return new Vector2Int(x, y);
        }

        /// <summary>
        /// returns -1 if not in any cell, function is inclusive
        /// </summary>
        public int GetGridCellIndex(Vector2 pos)
        {
            Vector2Int coords = GetGridCellCoords(pos);
            if (coords.x == -1) return -1;
            return toIdx(coords.x, coords.y);
        }

        int toIdx(int x, int y)
        {
            return (x + y * cellsX);
        }

        Vector2Int to2D(int idx)
        {
            return new Vector2Int(idx % cellsX, idx / cellsX);
        }
    }
}