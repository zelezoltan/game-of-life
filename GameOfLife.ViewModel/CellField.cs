using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public int Row { get; set; }
        public int Column { get; set; }
    }
}
