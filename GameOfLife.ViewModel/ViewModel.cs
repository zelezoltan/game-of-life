using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using GameOfLife.Model;
using GameOfLife.Persistence;

namespace GameOfLife.ViewModel
{
    public class ViewModel : ViewModelBase
    {
        private Model.Model _model;
        public ObservableCollection<CellField> Cells { get; set; }

        public int Size { get { return _model.Size; } }

        public int Generation { get { return _model.Generation; } }

        public bool Paused { get { return !_model.IsPlaying; } }
        public bool Playing { get { return _model.IsPlaying; } }

        public DelegateCommand LoadConfigurationCommand { get; private set; }
        public DelegateCommand StepCommand { get; private set; }
        public DelegateCommand PlayCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }

        public event EventHandler LoadConfiguration;
        public event EventHandler Step;
        public event EventHandler Pause;
        public event EventHandler Play;

        public ViewModel(Model.Model model)
        {
            this._model = model;
            _model.CellChanged += new EventHandler<CellChangedEventArgs>(Model_CellChanged);
            _model.SizeChanged += new EventHandler(Model_SizeChanged);

            LoadConfigurationCommand = new DelegateCommand(x => OnLoadConfiguration());
            StepCommand = new DelegateCommand(x => OnStep());
            PlayCommand = new DelegateCommand(x => OnPlay());
            PauseCommand = new DelegateCommand(x => OnPause());

            Cells = new ObservableCollection<CellField>();
            RefreshCells();
        }

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
    }
}
