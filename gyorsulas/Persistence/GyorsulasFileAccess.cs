using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading.Tasks;
using System.Data;

namespace gyorsulas.Persistence
{
    public class GyorsulasFileAccess : GyorsulasFileAccessInterface
    {
        private string defaultPath;

        public GyorsulasFileAccess(string defaultPath)
        {
            if (defaultPath != null) { this.defaultPath = defaultPath; }
            else { this.defaultPath = string.Empty; }
        }

        public async Task GyorsulasSave(string path, GyorsulasData data)
        {
            if (!(string.IsNullOrEmpty(defaultPath))) path = Path.Combine(defaultPath, path);

            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write($"{data._y} {data._x} {data._gameTime} {data._fuel} {data._playerPosition}");
                    await sw.WriteLineAsync($" {data._tickRate}");

                    for(int i = 0; i < data._y; i++)
                    {
                        for(int j = 0; j < data._x; j++)
                        {
                            await sw.WriteAsync($"{data[i,j]} ");
                        }
                        await sw.WriteLineAsync();
                    }
                }
            }
            catch
            {
                throw new GyorsulasDataException();
            }
        }
        public async Task<GyorsulasData> GyorsulasLoad(string path)
        {
            if (!(string.IsNullOrEmpty(defaultPath))) path = Path.Combine(defaultPath, path);

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line = await sr.ReadLineAsync() ?? string.Empty;
                    string[] data = line.Split(' ');
                    int y = int.Parse(data[0]);
                    int x = int.Parse(data[1]);
                    int gameTime = int.Parse(data[2]);
                    int fuel = int.Parse(data[3]);
                    int playerPosition = int.Parse(data[4]);
                    int TickRate = int.Parse(data[5]);
                    int[,] grid = new int[y,x];

                    for(int i = 0; i < y; i++)
                    {
                        line = await sr.ReadLineAsync() ?? string.Empty;
                        data = line.Split(' ');
                        for (int j = 0; j < x; j++)
                        {
                            grid[i,j] = int.Parse(data[j]);
                        }
                    }
                    return (new GyorsulasData(gameTime,fuel,playerPosition,TickRate,grid));  
                }
            }
            catch
            {
                throw new GyorsulasDataException();
            }
        }
    }
}
