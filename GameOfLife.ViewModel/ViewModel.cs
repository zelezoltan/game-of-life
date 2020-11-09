using System;
using System.Collections.ObjectModel;
using GameOfLife.Model;

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

        public DelegateCommand LoadConfigurationCommand { get; private set; }
        public DelegateCommand StepCommand { get; private set; }
        public DelegateCommand PlayCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        #endregion

        #region Events
        public event EventHandler LoadConfiguration;
        public event EventHandler Step;
        public event EventHandler Pause;
        public event EventHandler Play;
        #endregion

        #region Constructor
        public ViewModel(Model.Model model)
        {
            this._model = model;
            _model.CellChanged += new EventHandler<CellChangedEventArgs>(Model_CellChanged);
            _model.SizeChanged += new EventHandler(Model_SizeChanged);
            _model.GenerationChanged += new EventHandler(Model_GenerationChanged);

            LoadConfigurationCommand = new DelegateCommand(x => OnLoadConfiguration());
            StepCommand = new DelegateCommand(x => OnStep());
            PlayCommand = new DelegateCommand(x => OnPlay());
            PauseCommand = new DelegateCommand(x => OnPause());

            Cells = new ObservableCollection<CellField>();
            RefreshCells();
        }
        #endregion

        #region Public Methods
        public void RefreshCells()
        {
            Cells.Clear();
            Cell[,] modelCells = _model.Cells;

            for(int i = 0; i < _model.Size; ++i)
            {
                for (int j = 0; j < _model.Size; ++j)
                {
                    Cells.Add(new CellField
                    {
                        CellState = (int)modelCells[i, j],
                        Row = i,
                        Column = j
                    });
                }
            }
            OnPropertyChanged("Generation");
            OnPropertyChanged("Size");
            OnPropertyChanged("Paused");
            OnPropertyChanged("Playing");
        }
        #endregion

        #region Private Event handlers
        private void Model_CellChanged(Object sender, CellChangedEventArgs e)
        {
            Cells[e.PosX * _model.Size + e.PosY].CellState = (int)e.CellState;
            OnPropertyChanged("Generation");
        }

        private void Model_SizeChanged(Object sender, EventArgs e)
        {
            this.Cells.Clear();
            int size = _model.Size;
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    Cells.Add(new CellField
                    {
                        CellState = (int)Cell.Dead,
                        Row = i,
                        Column = j
                    });
                }
            }
        }

        private void Model_GenerationChanged(Object sender, EventArgs e)
        {
            OnPropertyChanged("Generation");
        }
        #endregion

        #region Event Methods
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
