using System;
using System.Threading.Tasks;
using System.IO;

namespace GameOfLife.Persistence
{
    /// <summary>
    /// Persistence class for loading .lif files.
    /// It supports loading from Life 1.05 and 1.06 files
    /// </summary>
    public class Persistence
    {
        public Persistence() { }

        /// <summary>
        /// Loading configuration from a file
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <returns></returns>
        public async Task<Configuration> LoadConfiguration(String path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    String line = await reader.ReadLineAsync();
                    Configuration configuration;
                    // Check version
                    if (line.Substring(6, 4) == "1.05")
                    {
                        configuration = await LoadLif105(reader);
                        return configuration;
                    }
                    else if (line.Substring(6, 4) == "1.06")
                    {
                        configuration = await LoadLif106(reader);
                        return configuration;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
            catch(Exception)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Loads the configuration from a Life 1.05 file
        /// </summary>
        /// <param name="reader">The stream reader object reading from the file</param>
        /// <returns></returns>
        private async Task<Configuration> LoadLif105(StreamReader reader)
        {
            Configuration configuration = new Configuration();
            String line;
            // Calculate the alive cell coordinates
            int currentUpperLeftX = 0;
            int currentUpperLeftY = 0;
            while (!reader.EndOfStream)
            {
                line = await reader.ReadLineAsync();
                if (line.Length > 0)
                {
                    if (line[0] != '#')
                    {
                        for (int i = 0; i < line.Length; ++i)
                        {
                            if (line[i] == '*')
                            {
                                int x = currentUpperLeftX + i;
                                int y = currentUpperLeftY;
                                configuration.aliveCoordinates.Add(new Coordinates(x, y));
                            }
                        }
                        ++currentUpperLeftY;
                    }
                    else if (line.Substring(0, 2) == "#P")
                    {
                        String[] lineElements = line.Split();
                        currentUpperLeftX = int.Parse(lineElements[1]);
                        currentUpperLeftY = int.Parse(lineElements[2]);
                    }
                    
                }
            }
            return configuration;
        }

        /// <summary>
        /// Loads the configuration from a Life 1.06 file
        /// </summary>
        /// <param name="reader">The stream reader object reading from the file</param>
        /// <returns></returns>
        private async Task<Configuration> LoadLif106(StreamReader reader)
        {
            Configuration configuration = new Configuration();
            String line;
            while (!reader.EndOfStream)
            {
                line = await reader.ReadLineAsync();
                if (line.Length > 0)
                {
                    String[] lineElements = line.Split();
                    int x = int.Parse(lineElements[0]);
                    int y = int.Parse(lineElements[1]);
                    configuration.aliveCoordinates.Add(new Coordinates(x, y));
                }
            }
            return configuration;
        }
    }
}
