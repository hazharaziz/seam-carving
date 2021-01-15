using Seam_Carving.Models;
using Seam_Carving.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;


namespace Seam_Carving.Util
{
    public class Energy
    {
        private readonly Pixel[][] _initialPixels;
        private Pixel[][] _carvedPixels;
        private readonly int _width;
        private readonly int _height;

        public Energy(Pixel[][] initialPixels, int width, int height)
        {
            _initialPixels = initialPixels;
            _width = width;
            _height = height;
        }
        

        /// <summary>
        /// ComputeVerticalEnergy method returns a 2D array of carved pixels with their energies
        /// </summary>
        /// <param name="verticalDeletedCount">Number of vertical seams deleted</param>
        /// <returns>An array of vertically carved pixels with their energies</returns>
        public Pixel[][] ComputeVerticalEnergy(int verticalDeletedCount)
        {
            Pixel[][] carvedPixels = Utility.Initialize2DArray<Pixel>(_width - verticalDeletedCount, _height);

            for (int i = 0; i < _height; i++)
            {
                int y = 0;
                for (int j = 0; j < _width; j++)
                {
                    if (y < _width - verticalDeletedCount)
                    {
                        carvedPixels[i][y] = new Pixel();
                        while (_initialPixels[i][j].IsSeamPixel)
                            j++;
                        carvedPixels[i][y].Color = _initialPixels[i][j].Color;
                        carvedPixels[i][y].AbsoluteX = i;
                        carvedPixels[i][y].AbsoluteY = j;
                        y++;
                    }
                }
            }

            _carvedPixels = carvedPixels;

            double deltaXSquared, deltaYSquared; 
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width - verticalDeletedCount; j++)
                {
                    deltaXSquared = GetSquaredDeltaXY(j, i, Axis.X, verticalDeletedCount, 0);
                    deltaYSquared = GetSquaredDeltaXY(j, i, Axis.Y, verticalDeletedCount, 0);
                    carvedPixels[i][j].Energy = Math.Sqrt(deltaXSquared + deltaYSquared);
                }
            }

            return carvedPixels;
        }


        /// <summary>
        /// ComputeHorizontalEnergy method returns a 2D array of carved pixels with their energies
        /// </summary>
        /// <param name="horizontalDeletedCount">Number of horizontal seams deleted</param>
        /// <returns>An array of horizontally carved pixels with their energies</returns>
        public Pixel[][] ComputeHorizontalEnergy(int horizontalDeletedCount)
        {
            Pixel[][] carvedPixels = Utility.Initialize2DArray<Pixel>(_width, _height - horizontalDeletedCount);

            for (int j = 0; j < _width; j++)
            {
                int x = 0;  
                for (int i = 0; i < _height; i++)
                {
                    if (x < _height - horizontalDeletedCount)
                    {
                        carvedPixels[x][j] = new Pixel();
                        while (_initialPixels[i][j].IsSeamPixel)
                            i++;
                        carvedPixels[x][j].Color = _initialPixels[i][j].Color;
                        carvedPixels[x][j].AbsoluteX = i;
                        carvedPixels[x][j].AbsoluteY = j;
                        x++;
                    }
                }
            }

            _carvedPixels = carvedPixels;

            double deltaXSquared, deltaYSquared;
            for (int j = 0; j < _width; j++)
            {
                for (int i = 0; i < _height - horizontalDeletedCount; i++)
                {
                    deltaXSquared = GetSquaredDeltaXY(j, i, Axis.X, 0, horizontalDeletedCount);
                    deltaYSquared = GetSquaredDeltaXY(j, i, Axis.Y, 0, horizontalDeletedCount);
                    carvedPixels[i][j].Energy = Math.Sqrt(deltaXSquared + deltaYSquared);
                }
            }

            return carvedPixels;
        }

        /// <summary>
        /// GetSquaredDeltaXY method for calculating the squared deltaX or deltaY
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rgb"></param>
        /// <param name="axis"></param>
        /// <param name="vDeletedCount">Number of vertical seams deleted</param>
        /// <param name="hDeletedCount">Number of horizontal seams deleted</param>
        /// <returns>Squared deltaX or deltaY</returns>
        public double GetSquaredDeltaXY(int x, int y, Axis axis, 
            int vDeletedCount = 0, int hDeletedCount = 0)
        {
            Color pre = GetPreviousColor(x, y, axis, vDeletedCount, hDeletedCount);
            Color next = GetNextColor(x, y, axis, vDeletedCount, hDeletedCount);
            double SquaredR = Math.Pow(next.R - pre.R, 2);
            double SquaredG = Math.Pow(next.G - pre.G, 2);
            double SquaredB = Math.Pow(next.B - pre.B, 2);
            return SquaredR + SquaredG + SquaredB;
        }


        /// <summary>
        /// GetNextColor method returns next adjacent color of each pixel (e.g. R(x+1, y))
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="axis"></param>
        /// <param name="vDeletedCount">Number of vertical seams deleted</param>
        /// <param name="hDeletedCount">Number of horizontal seams deleted</param>
        /// <returns>Next adjacent color</returns>
        public Color GetNextColor(int x, int y, Axis axis,
            int vDeletedCount = 0, int hDeletedCount = 0)
        {
            int width = _width - vDeletedCount;
            int height = _height - hDeletedCount;
            if (axis == Axis.X)
            {
                return Utility.ValidateCoordinate(x + 1, y, width, height) ?
                    _carvedPixels[y][x + 1].Color : Color.Black;

            } 
            else
            {
                return Utility.ValidateCoordinate(x, y + 1, width, height) ?
                    _carvedPixels[y + 1][x].Color : Color.Black;
            }
        }

        /// <summary>
        /// GetPreviousColor method returns the previous adjacent color (e.g. R(x-1,y))
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="axis"></param>
        /// <param name="vDeletedCount">Number of vertical seams deleted</param>
        /// <param name="hDeletedCount">Number of horizontal seams deleted</param>
        /// <returns>Previous adjacent pixels</returns>
        public Color GetPreviousColor(int x, int y, Axis axis,
            int vDeletedCount = 0, int hDeletedCount = 0)
        {
            int width = _width - vDeletedCount;
            int height = _height - hDeletedCount;

            if (axis == Axis.X)
            {
                return Utility.ValidateCoordinate(x - 1, y, width, height) ?
                    _carvedPixels[y][x - 1].Color : Color.Black;
            } 
            else
            {
                return Utility.ValidateCoordinate(x, y - 1, width, height) ?
                    _carvedPixels[y - 1][x].Color : Color.Black;
            }
        }
    }
}
