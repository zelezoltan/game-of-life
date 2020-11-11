using System;
using System.Windows.Media;

namespace GameOfLife.ViewModel
{
    public class CellField : ViewModelBase
    {
        private int _cellState;

        public int CellState
        {
            get { return _cellState; }
            set
            {
                if (_cellState != value)
                {
                    _cellState = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Row { get; set; }
        public double Column { get; set; }
    }
}
