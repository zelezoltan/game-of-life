using System;
using System.Collections.Generic;

namespace GameOfLife.Persistence
{
    /// <summary>
    /// Class representing a configuration
    /// </summary>
    public class Configuration
    {
        public List<Coordinates> aliveCoordinates;
        public Configuration()
        {
            aliveCoordinates = new List<Coordinates>();
        }
    }
}
