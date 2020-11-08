using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife.Persistence
{
    public class Configuration
    {
        public List<Coordinates> aliveCoordinates;
        public Configuration()
        {
            aliveCoordinates = new List<Coordinates>();
        }
    }
}
