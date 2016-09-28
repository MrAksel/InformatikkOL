using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformatikkOL.Challenges
{
    class Gruvedrift : Challenge
    {
        const int MUD = 0;
        const int ROCK = 1;
        const int DIAMOND = 255;

        byte[][,] map;

        public override void Main()
        {
            // Read dimensions
            int width = ReadIntSpace();
            int height = ReadIntSpace();
            int depth = ReadIntLine();

            map = new byte[depth][,];
            
            // Read map
            for (int z = 0; z < depth; z++)
            {
                map[z] = new byte[width, height];
                for (int y = 0; y < height; y++)
                {
                    string row = Console.ReadLine();
                    for (int x = 0; x < width; x++)
                    {
                        switch (row[x])
                        {
                            case '*':
                                map[z][x, y] = DIAMOND; break;
                            case '#':
                                map[z][x, y] = ROCK; break;
                        }
                    }
                }
            }


        }
    }
}
