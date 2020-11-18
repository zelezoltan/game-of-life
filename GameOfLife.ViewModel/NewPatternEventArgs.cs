using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife.ViewModel
{
    public class NewPatternEventArgs : EventArgs
    {
        public string Size { get; set; }
        public bool Random { get; set; }

        public NewPatternEventArgs(string size, bool random)
        {
            this.Size = size;
            this.Random = random;
        }
    }
}
