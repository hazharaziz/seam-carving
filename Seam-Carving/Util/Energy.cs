using Seam_Carving.Models;
using Seam_Carving.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;


namespace Seam_Carving.Util
{
    public enum Axis { X, Y };
    public enum RGB { R, G, B };

    /// <summary>
    /// Energy class for calculating the energies of image pixels
    /// </summary>
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
        /// ComputeEnergy method for computing the energies of the pixels
        /// </summary>
        /// <param name="verticalDeletedCount">number of the deleted vertical seams till now</param>
        /// <returns>An array of carved pixels</returns>
        public Pixel[][] ComputeEnergy(int verticalDeletedCount)
        {
            Pixel[][] carvedPixels = Utility.Initialize2DArray<Pixel>(_width - verticalDeletedCount, _height);

            for (int i = 0; i < _height; i++)
            {
                int index = 0;
                for (int j = 0; j < _width; j++)
                {
                    if (index < _width - verticalDeletedCount)
                    {
                        carvedPixels[i][index] = new Pixel();
                        while (_initialPixels[i][j].IsSeamPixel)
                            j++;
                        carvedPixels[i][index].Color = _initialPixels[i][j].Color;
                        carvedPixels[i][index].AbsoluteX = i;
                        carvedPixels[i][index].AbsoluteY = j;
                        index++;
                    }
                }
            }

            _carvedPixels = carvedPixels;

            double deltaXSquared, deltaYSquared; 
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width - verticalDeletedCount; j++)
                {
                    deltaXSquared = GetSquaredDeltaXY(j, i, Axis.X, verticalDeletedCount);
                    deltaYSquared = GetSquaredDeltaXY(j, i, Axis.Y, verticalDeletedCount);
                    carvedPixels[i][j].Energy = Math.Sqrt(deltaXSquared + deltaYSquared);
                }
            }

            return carvedPixels;
        }


        /// <summary>
        /// GetSquaredDeltaXY method for calculating the DeltaX or DeltaY of pixel(x,y)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="axis"></param>
        /// <param name="vDeletedCount">Number of vertical seams deleted</param>
        /// <param name="hDeletedCount">Number of horizontal seams deleted</param>
        /// <returns>Squared DeltaX or DeltaY</returns>
        public double GetSquaredDeltaXY(int x, int y, Axis axis, int vDeletedCount = 0, int hDeletedCount = 0)
        {
            double R = GetSquaredDifference(x, y, RGB.R, axis, vDeletedCount, hDeletedCount);
            double G = GetSquaredDifference(x, y, RGB.G, axis, vDeletedCount, hDeletedCount);
            double B = GetSquaredDifference(x, y, RGB.B, axis, vDeletedCount, hDeletedCount);
            return (R + G + B);
        }

        /// <summary>
        /// GetSquaredDifference method for calculating the squared R or G or B
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rgb"></param>
        /// <param name="axis"></param>
        /// <param name="vDeletedCount">Number of vertical seams deleted</param>
        /// <param name="hDeletedCount">Number of horizontal seams deleted</param>
        /// <returns>Squared Rx|Ry, Gx|Gy, Bx|By</returns>
        public double GetSquaredDifference(int x, int y, RGB rgb, Axis axis, 
            int vDeletedCount = 0, int hDeletedCount = 0)
        {
            Color pre = GetPreviousColor(x, y, axis, vDeletedCount, hDeletedCount);
            Color next = GetNextColor(x, y, axis, vDeletedCount, hDeletedCount);
            double difference = (rgb == RGB.R) ? next.R - pre.R :
                (rgb == RGB.G) ? next.G - pre.G : next.B - pre.B;
            return Math.Pow(difference, 2);
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
            return (axis == Axis.X) ?
                ((Utility.ValidateCoordinate(x + 1, y, width, height)) ?
                    _carvedPixels[y][x + 1].Color : Color.Black) :
                ((Utility.ValidateCoordinate(x, y + 1, width, height)) ?
                _carvedPixels[y + 1][x].Color : Color.Black);
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

            return (axis == Axis.X) ?
                ((Utility.ValidateCoordinate(x - 1, y, width, height)) ?
                    _carvedPixels[y][x - 1].Color : Color.Black) :
                ((Utility.ValidateCoordinate(x, y - 1, width, height)) ?
                    _carvedPixels[y - 1][x].Color : Color.Black);
        }
    }
}
