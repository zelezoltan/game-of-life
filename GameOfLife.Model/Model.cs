using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfLife.Persistence;

namespace GameOfLife.Model
{
    public class Model
    {
        private Persistence.Persistence _persistence;

        private Cell[,] _cells;
        private int _size;
        private int _generation;
        private bool _isPlaying;

        public Cell[,] Cells { get { return _cells; } }
        public int Generation { get { return _generation; } }
        public int Size { get { return _size; } }
        public bool IsPlaying { get { return _isPlaying; } }

        public Model(Persistence.Persistence persistence)
        {
            this._persistence = persistence;
            this._generation = 0;
            this._size = 0;
        }

        public void Step()
        {
            if (this._size == 0) return;
            ++_generation;
            Cell[,] newCells = new Cell[this._size, this._size];
            for (int i = 0; i < this._size; ++i)
            {
                for (int j = 0; j < this._size; ++j)
                {
                    int aliveNeighbors = CountNeighboringAliveCells(i, j);
                    if (_cells[i,j] == Cell.Alive && (aliveNeighbors == 2 || aliveNeighbors == 3))
                    {
                        newCells[i, j] = Cell.Alive;
                    }
                    else if (_cells[i,j] == Cell.Dead && aliveNeighbors == 3)
                    {
                        newCells[i, j] = Cell.Alive;
                        OnCellChanged(i, j, newCells[i, j]);
                    }
                    else if (_cells[i,j] == Cell.Alive && (aliveNeighbors < 2 || aliveNeighbors > 3))
                    {
                        newCells[i, j] = Cell.Dead;
                        OnCellChanged(i, j, newCells[i, j]);
                    } else
                    {
                        newCells[i, j] = Cell.Dead;
                    }
                }
            }
            this._cells = newCells;
        }

        private int CountNeighboringAliveCells(int x, int y)
        {
            int count = 0;
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    if (x+i >= 0 && y+j >= 0 && x+i < this._size && y+j < this._size && this._cells[x+i, y+j] == Cell.Alive)
                    {
                        ++count;
                    }
                }
            }
            return count - (this._cells[x,y] == Cell.Alive ? 1 : 0);
        }

        public void TogglePlay()
        {
            if (this._size == 0) return;
            _isPlaying = !_isPlaying;
        }

        public async Task LoadConfiguration(String path)
        {
            if (_persistence == null) throw new InvalidOperationException("No persistence provided!");
            Configuration configuration = await _persistence.LoadConfiguration(path);
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;
            foreach (Coordinates coord in configuration.aliveCoordinates)
            {
                if (coord.x < minX) minX = coord.x;
                if (coord.y < minY) minY = coord.y;
                if (coord.x > maxX) maxX = coord.x;
                if (coord.y > maxY) maxY = coord.y;
            }
            int padding = 5;
            foreach (Coordinates coord in configuration.aliveCoordinates)
            {
                coord.x = coord.x - minX + padding;
                coord.y = coord.y - minY + padding;
            }
            maxX = maxX - minX + padding;
            maxY = maxY - minY + padding;
            minX = padding;
            minY = padding;
            this._size = Math.Max(maxX, maxY) + 2*padding;
            OnSizeChanged(this._size);
            this._generation = 0;
            this.InitializeCells(configuration.aliveCoordinates);
        }

        private void InitializeCells(List<Coordinates> aliveCoordinates)
        {
            this._cells = new Cell[this._size, this._size];
            for (int i = 0; i < this._size; ++i)
            {
                for (int j = 0; j < this._size; ++j)
                {
                    this._cells[i, j] = Cell.Dead;
                    OnCellChanged(i, j, Cell.Dead);
                }
            }
            foreach (Coordinates coord in aliveCoordinates)
            {
                this._cells[coord.x, coord.y] = Cell.Alive;
                OnCellChanged(coord.x, coord.y, Cell.Alive);
            }
        }

        public event EventHandler<CellChangedEventArgs> CellChanged;
        public event EventHandler SizeChanged;

        private void OnCellChanged(int x, int y, Cell cell)
        {
            CellChanged?.Invoke(this, new CellChangedEventArgs(x, y, cell));
        }

        private void OnSizeChanged(int size)
        {
            SizeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
