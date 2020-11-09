using System;

namespace GameOfLife.Model
{
    /// <summary>
    /// Event arguments class for a cell changing state
    /// </summary>
    public class CellChangedEventArgs : EventArgs
    {
        private int _x;
        private int _y;
        private Cell _cellState;

        /// <summary>
        /// Returns the x-coordinate of the cell
        /// </summary>
        public int PosX { get { return _x; } }

        /// <summary>
        /// Returns the y-coordinate of the cell
        /// </summary>
        public int PosY { get { return _y; } }

        /// <summary>
        /// Return the cell state of the cell
        /// </summary>
        public Cell CellState { get { return _cellState; } }

        /// <summary>
        /// Creating the event argument
        /// </summary>
        /// <param name="x">The x-coordinate of the cell</param>
        /// <param name="y">The y-coordinate of the cell</param>
        /// <param name="cell">The new state of the cell</param>
        public CellChangedEventArgs(int x, int y, Cell cell)
        {
            this._x = x;
            this._y = y;
            this._cellState = cell;
        }
    }
}
