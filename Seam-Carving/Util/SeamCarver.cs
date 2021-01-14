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
        /// FindSeam method for finding the seams of the image
        /// </summary>
        /// <param name="carvedPixels"></param>
        public void FindSeam(ref Pixel[][] carvedPixels)
        {
            int width = carvedPixels[0].Length;
            int height = carvedPixels.Length;
            SeamItem[][] seam = Utility.Initialize2DArray<SeamItem>(width, height);

            for (int i = 0; i < width; i++)
            {
                seam[0][i] = new SeamItem()
                {
                    Value = carvedPixels[0][i].Energy,
                    ParentIndex = -1,
                };
            }

            FindSeamValues(ref seam, carvedPixels);
            (double min, int index) = FindMinimumIndex(seam[height - 1]);
            Carve(carvedPixels, seam, index, height);
        }


        /// <summary>
        /// FindSeamValues method finds the minimum values of the DP table
        /// </summary>
        /// <param name="seam"></param>
        /// <param name="pixels"></param>
        private void FindSeamValues(ref SeamItem[][] seam, Pixel[][] pixels)
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
                            pixels[i][j].Energy + seam[i - 1][j - 1].Value : double.MaxValue;
                    top = (Utility.ValidateCoordinate(j, i - 1, width, height)) ?
                            pixels[i][j].Energy + seam[i - 1][j].Value : double.MaxValue;
                    right = (Utility.ValidateCoordinate(j + 1, i - 1, width, height)) ?
                                pixels[i][j].Energy + seam[i - 1][j + 1].Value : double.MaxValue;
                    (min, index) = FindMinimumIndex(new double[] { left, top, right });
                    seam[i][j] = new SeamItem()
                    {
                        Value = min,
                        ParentIndex = j + index - 1
                    };
                }
            }
        }

        /// <summary>
        /// Carve method carves the seams of the image
        /// </summary>
        /// <param name="carvedPixels">Pixels of the carved image</param>
        /// <param name="seam">Seam table for finding the seam path to be deleted</param>
        /// <param name="minIndex">Minimum index of the pixel to deleted</param>
        /// <param name="height">Height of image</param>
        public void Carve(Pixel[][] carvedPixels, SeamItem[][] seam, int minIndex, int height)
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
