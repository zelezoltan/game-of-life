using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GameOfLife.Persistence;

namespace GameOfLife.Model
{
    /// <summary>
    /// Game Of Life model class
    /// </summary>
    public class Model
    {
        #region Fields
        private Persistence.Persistence _persistence;

        private Cell[,] _cells;
        private int _size;
        private int _generation;
        private bool _isPlaying;
        #endregion

        #region Properties
        /// <summary>
        /// Returns a matrix with the cell states
        /// </summary>
        public Cell[,] Cells { get { return _cells; } }

        /// <summary>
        /// Returns the current generation number
        /// </summary>
        public int Generation { get { return _generation; } }

        /// <summary>
        /// Returns the size of the matrix containing the cell states
        /// </summary>
        public int Size { get { return _size; } }

        /// <summary>
        /// Returns if the simulation is playing
        /// </summary>
        public bool IsPlaying { get { return _isPlaying; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Creating a model
        /// </summary>
        /// <param name="persistence">Persistence object to be injected</param>
        public Model(Persistence.Persistence persistence)
        {
            this._persistence = persistence;
            this._generation = 0;
            this._size = 0;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates the next generation
        /// </summary>
        public void Step()
        {
            if (this._size == 0) return;
            ++_generation;
            OnGenerationChanged();
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

        /// <summary>
        /// Toggles the running of the simulation
        /// </summary>
        public bool TogglePlay()
        {
            if (this._size == 0) return false;
            _isPlaying = !_isPlaying;
            return true;
        }

        /// <summary>
        /// Loads a configuration from a file
        /// </summary>
        /// <param name="path">The path of the configuration file</param>
        /// <returns></returns>
        public async Task LoadConfiguration(String path)
        {
            if (_persistence == null) throw new InvalidOperationException("No persistence provided!");
            // Loading the alive coordinates
            Configuration configuration = await _persistence.LoadConfiguration(path);
            // To calculate the bounding square we calculate the min and max coordinates of the cells
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
            // Add some padding for dead cells surrounding the alive cells
            int padding = 5;
            // Transform the coordinates to the interval [padding, max-padding]
            foreach (Coordinates coord in configuration.aliveCoordinates)
            {
                coord.x = coord.x - minX + padding;
                coord.y = coord.y - minY + padding;
            }
            maxX = maxX - minX + padding;
            maxY = maxY - minY + padding;
            minX = padding;
            minY = padding;
            this._size = Math.Max(maxX, maxY) + padding;
            // Center everything
            if (maxX > maxY)
            {
                int diff = maxX - maxY;
                foreach (Coordinates coord in configuration.aliveCoordinates)
                {
                    coord.y += diff / 2;
                }
            } else
            {
                int diff = maxY - maxX;
                foreach (Coordinates coord in configuration.aliveCoordinates)
                {
                    coord.x += diff / 2;
                }
            }
            OnSizeChanged();
            this._generation = 0;
            // Initialize the cell matrix with alive cells
            this.InitializeCells(configuration.aliveCoordinates);
        }

        public void ChangeCell(int x, int y)
        {
            if (!this._isPlaying)
            {
                if (this._cells[x, y] == Cell.Alive)
                {
                    this._cells[x, y] = Cell.Dead;
                } else
                {
                    this._cells[x, y] = Cell.Alive;
                }
                
                OnCellChanged(x, y, this._cells[x, y]);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Counts the alive cells neighboring the current cell
        /// </summary>
        /// <param name="x">The x-coordinate of the current cell</param>
        /// <param name="y">The y-coordinate of the current cell</param>
        /// <returns>The number of alive neighboring cells</returns>
        private int CountNeighboringAliveCells(int x, int y)
        {
            int count = 0;
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    // If we are in bound and the neighboring cell is alive
                    //if (x+i >= 0 && y+j >= 0 && x+i < this._size && y+j < this._size && this._cells[x+i, y+j] == Cell.Alive)
                    //{
                    //    ++count;
                    //}

                    // Wrap around the sides
                    int index_x = (x + i + this._size) % this._size;
                    int index_y = (y + j + this._size) % this._size;
                    if (this._cells[index_x, index_y] == Cell.Alive)
                    {
                        ++count;
                    }
                }
            }
            // We count the current cell if it is alive, so we have to subtract it
            return count - (this._cells[x,y] == Cell.Alive ? 1 : 0);
        }

        /// <summary>
        /// Initializes the cell matrix
        /// </summary>
        /// <param name="aliveCoordinates">The coordinates of the alive cells</param>
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
        #endregion

        #region Events
        /// <summary>
        /// Event signaling a changed cell
        /// </summary>
        public event EventHandler<CellChangedEventArgs> CellChanged;

        /// <summary>
        /// Event signaling that the size of the cell matrix changed
        /// </summary>
        public event EventHandler SizeChanged;

        /// <summary>
        /// Event signaling that the generation changed
        /// </summary>
        public event EventHandler GenerationChanged;
        #endregion

        #region Private Event Methods
        /// <summary>
        /// Triggering the cell changed event
        /// </summary>
        /// <param name="x">The x-coordinate of the cell</param>
        /// <param name="y">The y-coordinate of the cell</param>
        /// <param name="cell">The new state of the cell</param>
        private void OnCellChanged(int x, int y, Cell cell)
        {
            CellChanged?.Invoke(this, new CellChangedEventArgs(x, y, cell));
        }

        /// <summary>
        /// Triggering the size changed event 
        /// </summary>
        private void OnSizeChanged()
        {
            SizeChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Triggering the generation changed event
        /// </summary>
        private void OnGenerationChanged()
        {
            GenerationChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
