using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using GameOfLife.Model;
using GameOfLife.Persistence;
using System.Linq;

namespace GameOfLife.ViewModel
{
    /// <summary>
    /// Game Of Life ViewModel class
    /// </summary>
    public class ViewModel : ViewModelBase
    {
        #region Fields
        private Model.Model _model;
        #endregion

        #region Properties
        public ObservableCollection<CellField> Cells { get; set; }

        public int Size { get { return _model.Size; } }

        public int Generation { get { return _model.Generation; } }

        public bool Paused { get { return !_model.IsPlaying; } }
        public bool Playing { get { return _model.IsPlaying; } }
        public double CellSizeX { get { return 680.0/ Size; } }
        public double CellSizeY { get { return 680.0 / Size; } }

        public DelegateCommand LoadConfigurationCommand { get; private set; }
        public DelegateCommand StepCommand { get; private set; }
        public DelegateCommand PlayCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand DeleteCellCommand { get; private set; }
        public DelegateCommand CanvasClickCommand { get; private set; }
        #endregion

        #region Events
        public event EventHandler LoadConfiguration;
        public event EventHandler Step;
        public event EventHandler Pause;
        public event EventHandler Play;
        public event EventHandler CanvasClick;
        #endregion

        #region Constructor
        public ViewModel(Model.Model model)
        {
            this._model = model;
            _model.CellChanged += new EventHandler<CellChangedEventArgs>(Model_CellChanged);
            _model.SizeChanged += new EventHandler(Model_SizeChanged);
            _model.GenerationChanged += new EventHandler(Model_GenerationChanged);
            _model.LoadComplete += new EventHandler(Model_LoadComplete);

            LoadConfigurationCommand = new DelegateCommand(x => OnLoadConfiguration());
            StepCommand = new DelegateCommand(x => OnStep());
            PlayCommand = new DelegateCommand(x => OnPlay());
            PauseCommand = new DelegateCommand(x => OnPause());
            DeleteCellCommand = new DelegateCommand(x =>
            {
                CellField field = (CellField)x;
                _model.ChangeCell((int)(field.Row / CellSizeY), (int)(field.Column / CellSizeX));
            });
            CanvasClickCommand = new DelegateCommand(x => {
                OnCanvasClick();
            });

            Cells = new ObservableCollection<CellField>();
        }
        #endregion

        #region Public Methods
        public void RefreshCells()
        {
            Cells.Clear();

            List<Coordinates> aliveCells = _model.AliveCells;
            foreach (Coordinates coord in aliveCells)
            {
                Cells.Add(new CellField
                {
                    CellState = (int)Cell.Alive,
                    Row = coord.x * CellSizeY,
                    Column = coord.y * CellSizeX
                });
            }

            OnPropertyChanged("Generation");
            OnPropertyChanged("Size");
            OnPropertyChanged("Paused");
            OnPropertyChanged("Playing");
        }
        #endregion

        #region Private Event handlers

        private void Model_LoadComplete(Object sender, EventArgs e)
        {
            RefreshCells();
        }

        private void Model_CellChanged(Object sender, CellChangedEventArgs e)
        {
            if (e.CellState == Cell.Dead)
            {
                CellField cellToRemove = Cells.Single(cell => Math.Abs(cell.Row - e.PosX * CellSizeY) < 0.0001 && Math.Abs(cell.Column - e.PosY * CellSizeX) < 0.0001);
                Cells.Remove(cellToRemove);
            } else
            {
                Cells.Add(new CellField
                {
                    CellState = (int)Cell.Alive,
                    Row = e.PosX * CellSizeY,
                    Column = e.PosY * CellSizeX
                });
            }
            
            OnPropertyChanged("Generation");
        }

        private void Model_SizeChanged(Object sender, EventArgs e)
        {
            RefreshCells();
        }

        private void Model_GenerationChanged(Object sender, EventArgs e)
        {
            OnPropertyChanged("Generation");
        }
        #endregion

        #region Event Methods

        private void OnCanvasClick()
        {
            CanvasClick?.Invoke(this, EventArgs.Empty);
        }
        private void OnLoadConfiguration()
        {
            LoadConfiguration?.Invoke(this, EventArgs.Empty);
        }

        private void OnStep()
        {
            Step?.Invoke(this, EventArgs.Empty);
        }

        private void OnPlay()
        {
            Play?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged("Paused");
            OnPropertyChanged("Playing");
        }

        private void OnPause()
        {
            Pause?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged("Paused");
            OnPropertyChanged("Playing");
        }
        #endregion
    }
}
