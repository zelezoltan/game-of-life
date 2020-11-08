using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOfLife.Model;

namespace GameOfLife.ViewModel
{
    public class ViewModel : ViewModelBase
    {
        private Model.Model _model;

        public ViewModel(Model.Model model)
        {
            this._model = model;
        }
    }
}
