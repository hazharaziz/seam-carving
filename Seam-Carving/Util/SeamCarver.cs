using Seam_Carving.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Seam_Carving.Util
{
    public class SeamCarver
    {
        private Pixel[][] _initialPixels;

        public SeamCarver(Pixel[][] pixels)
        {
            _initialPixels = pixels;
        }

        /// <summary>
        /// FindVerticalSeam method for finding the vertical seams of the image
        /// </summary>
        /// <param name="carvedPixels"></param>
        public void FindVerticalSeam(ref Pixel[][] carvedPixels)
        {
            int width = carvedPixels[0].Length;
            int height = carvedPixels.Length;
            SeamItem[][] seamTable = Utility.Initialize2DArray<SeamItem>(width, height);

            for (int i = 0; i < width; i++)
            {
                seamTable[0][i] = new SeamItem()
                {
                    Value = carvedPixels[0][i].Energy,
                    ParentIndex = -1,
                };
            }

            FindVerticalMinimumValues(ref seamTable, carvedPixels);
            (double min, int index) = FindMinimumIndex(seamTable[height - 1]);
            CarveVerticalSeam(carvedPixels, seamTable, index, height);
        }




        /// <summary>
        /// FindVerticalMinimumValues method finds the minimum values of the DP table vertically
        /// </summary>
        /// <param name="seamTable"></param>
        /// <param name="pixels"></param>
        private void FindVerticalMinimumValues(ref SeamItem[][] seamTable, Pixel[][] pixels)
        {
            int width = pixels[0].Length;
            int height = pixels.Length;
            double min;
            int index;
            double left, top, right;
            for (int i = 1; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    left = (Utility.ValidateCoordinate(j - 1, i - 1, width, height)) ?
                            pixels[i][j].Energy + seamTable[i - 1][j - 1].Value : double.MaxValue;
                    top = (Utility.ValidateCoordinate(j, i - 1, width, height)) ?
                            pixels[i][j].Energy + seamTable[i - 1][j].Value : double.MaxValue;
                    right = (Utility.ValidateCoordinate(j + 1, i - 1, width, height)) ?
                                pixels[i][j].Energy + seamTable[i - 1][j + 1].Value : double.MaxValue;
                    (min, index) = FindMinimumIndex(new double[] { left, top, right });
                    seamTable[i][j] = new SeamItem()
                    {
                        Value = min,
                        ParentIndex = j + index - 1
                    };
                }
            }
        }

        /// <summary>
        /// CarveVerticalSeam method carves the vertical seams of the image
        /// </summary>
        /// <param name="carvedPixels">Pixels of the carved image</param>
        /// <param name="seam">Seam table for finding the seam path to be deleted</param>
        /// <param name="minIndex">Minimum index of the pixel to deleted</param>
        /// <param name="height">Height of image</param>
        public void CarveVerticalSeam(Pixel[][] carvedPixels, SeamItem[][] seam, int minIndex, int height)
        {
            int x, y;

            for (int i = height - 1; i >= 0; i--)
            {
                x = carvedPixels[i][minIndex].AbsoluteX;
                y = carvedPixels[i][minIndex].AbsoluteY;
                _initialPixels[x][y].Color = Color.Red;
                _initialPixels[x][y].IsSeamPixel = true;
                minIndex = seam[i][minIndex].ParentIndex;
            }
        }

        /// <summary>
        /// FindHorizontalSeam method for finding the horizontal seams of the image
        /// </summary>
        /// <param name="carvedPixels"></param>
        public void FindHorizontalSeam(ref Pixel[][] carvedPixels)
        {
            int width = carvedPixels[0].Length;
            int height = carvedPixels.Length;
            SeamItem[][] seamTable = Utility.Initialize2DArray<SeamItem>(width, height);

            for (int i = 0; i < height; i++)
            {
                seamTable[i][0] = new SeamItem()
                {
                    Value = carvedPixels[i][0].Energy,
                    ParentIndex = -1,
                };
            }

            FindHorizontalMinimumValues(ref seamTable, carvedPixels);
            SeamItem[] lastColumn = new SeamItem[height];
            for (int i = 0; i < height; i++)
                lastColumn[i] = seamTable[i][width - 1];

            (double min, int index) = FindMinimumIndex(lastColumn);
            CarveHorizontalSeam(carvedPixels, seamTable, index, width);
        }


        /// <summary>
        /// FindHorizontalMinimumValues method finds the minimum values of the DP table horizontally
        /// </summary>
        /// <param name="seamTable"></param>
        /// <param name="pixels"></param>
        private void FindHorizontalMinimumValues(ref SeamItem[][] seamTable, Pixel[][] pixels)
        {
            int width = pixels[0].Length;
            int height = pixels.Length;
            double min;
            int index;
            double top, left, bottom;
            for (int j = 1; j < width; j++)
            {
                for (int i = 0; i < height; i++)
                {
                    top = (Utility.ValidateCoordinate(j - 1, i - 1, width, height)) ?
                            pixels[i][j].Energy + seamTable[i - 1][j - 1].Value : double.MaxValue;
                    left = (Utility.ValidateCoordinate(j - 1, i, width, height)) ?
                            pixels[i][j].Energy + seamTable[i][j - 1].Value : double.MaxValue;
                    bottom = (Utility.ValidateCoordinate(j - 1, i + 1, width, height)) ?
                                pixels[i][j].Energy + seamTable[i + 1][j - 1].Value : double.MaxValue;
                    (min, index) = FindMinimumIndex(new double[] { top, left, bottom });
                    seamTable[i][j] = new SeamItem()
                    {
                        Value = min,
                        ParentIndex = i + index - 1
                    };
                }
            }
        }

        /// <summary>
        /// CarveHorizontalSeam method carves the horizontal seams of the image
        /// </summary>
        /// <param name="carvedPixels">Pixels of the carved image</param>
        /// <param name="seam">Seam table for finding the seam path to be deleted</param>
        /// <param name="minIndex">Minimum index of the pixel to deleted</param>
        /// <param name="width">Height of image</param>
        public void CarveHorizontalSeam(Pixel[][] carvedPixels, SeamItem[][] seam, int minIndex, int width)
        {
            int x, y;
            for (int i = width - 1; i >= 0; i--)
            {
                x = carvedPixels[minIndex][i].AbsoluteX;
                y = carvedPixels[minIndex][i].AbsoluteY;
                _initialPixels[x][y].Color = Color.Red;
                _initialPixels[x][y].IsSeamPixel = true;
                minIndex = seam[minIndex][i].ParentIndex;
            }
        }

        /// <summary>
        /// FindMinimumIndex method for finding the SeamItem with the minimum value
        /// </summary>
        /// <param name="array"></param>
        /// <returns>Minimum value and its index</returns>
        private (double, int) FindMinimumIndex(SeamItem[] array)
        {
            (double min, int index) = (double.MaxValue, 0);

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Value < min)
                {
                    min = array[i].Value;
                    index = i;
                }
            }

            return (min, index);
        }

        /// <summary>
        /// FindMinimumIndex method for finding the minimum value of double array 
        /// </summary>
        /// <param name="array"></param>
        /// <returns>Minimum value and its index</returns>
        private (double, int) FindMinimumIndex(double[] array)
        {
            (double min, int index) = (double.MaxValue, 0);

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < min)
                {
                    min = array[i];
                    index = i;
                }
            }

            return (min, index);
        }
    }
}
