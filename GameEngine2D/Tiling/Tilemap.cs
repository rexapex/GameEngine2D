﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GameEngine2D.Tiling
{
    // Grid based tilemap
    public class Tilemap
    {
        // 2D grid array indexed as [x, y]
        public int[,] TileGrid { get; private set; }
        
        // Dimensions of the array
        public int Width { get; private set; }
        public int Height { get; private set; }

        // The pixel spacing between each tile (defaults to the tilewidth/tileheight)
        // Value less than use default spacing
        public int SpacingX { get; set; }
        public int SpacingY { get; set; }

        public Tilemap(string fileContents, int spacingX=-1, int spacingY=-1)
        {
            // Get the height from the string
            var lines = Regex.Split(fileContents, "\r\n|\n|\r");
            var height = lines.Length;
            if (height == 0) return;

            // Get the width from the string
            var cells = Regex.Split(lines[0], " ");
            var width = cells.Length;
            
            // Construct the tile grid
            TileGrid = new int[width, height];

            // Fill the tile grid with the contents of the file
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height && x < lines[y].Length; y++)
                {
                    // Convert the string into an int and fill grid cell
                    try
                    {
                        TileGrid[x, height-y-1] = Int32.Parse(lines[y].Split(' ')[x]);
                    }
                    catch(FormatException e)
                    {
                        Console.WriteLine(e.Message);
                        TileGrid[x, y] = -1;
                    }
                }
            }

            // Set the dimensions
            Width = width;
            Height = height;

            // Set the tile spacing
            SpacingX = spacingX;
            SpacingY = spacingY;
        }
    }
}
