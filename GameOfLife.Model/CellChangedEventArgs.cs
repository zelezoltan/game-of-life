using System;
using System.Collections.Generic;
using System.Text;
using GameOfLife.Persistence;

namespace GameOfLife.Model
{
    public class CellChangedEventArgs : EventArgs
    {
        private int _x;
        private int _y;
        private Cell _cellState;

        public int PosX { get { return _x; } }
        public int PosY { get { return _y; } }
        public Cell CellState { get { return _cellState; } }

        public CellChangedEventArgs(int x, int y, Cell cell)
        {
            this._x = x;
            this._y = y;
            this._cellState = cell;
        }
    }
}
