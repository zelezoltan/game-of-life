using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace GameOfLife.Persistence
{
    public class Persistence
    {
        public Persistence() { }

        public async Task<Configuration> LoadConfiguration(String path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    String line = await reader.ReadLineAsync();
                    Configuration configuration;
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

        private async Task<Configuration> LoadLif105(StreamReader reader)
        {
            Configuration configuration = new Configuration();
            String line;
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
