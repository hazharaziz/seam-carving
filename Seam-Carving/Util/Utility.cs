using Seam_Carving.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Seam_Carving.Util
{
    public enum Axis { X, Y };
    public enum RGB { R, G, B };

    public static class Utility
    {
        /// <summary>
        /// ValidateCoordinate method for validating the coordinates according to the width and height
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>True if the coordinates are valid</returns>
        public static bool ValidateCoordinate(int x, int y, int width, int height) => 
            x >= 0 && x <= width - 1 && y >= 0 && y <= height - 1;

        /// <summary>
        /// Initialize2DArray method initializes a 2D array of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>A 2D array of type T</returns>
        public static T[][] Initialize2DArray<T>(int width, int height)
        {
            T[][] array = new T[height][];
            for (int i = 0; i < height; i++)
            {
                array[i] = new T[width];
            }

            return array;
        }

        /// <summary>
        /// GetImagePixels method extracts the pixels of the image bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>An array of image pixels</returns>
        public static Pixel[][] GetImagePixels(Bitmap bitmap)
        {
            Pixel[][] pixels = new Pixel[bitmap.Height][];
            for (int i = 0; i < bitmap.Height; i++)
                pixels[i] = new Pixel[bitmap.Width];

            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    pixels[i][j] = new Pixel()
                    {
                        Energy = 0,
                        Color = bitmap.GetPixel(j, i),
                        IsSeamPixel = false,
                        AbsoluteX = i,
                        AbsoluteY = j,
                    };
                }
            }

            return pixels;
        }

        /// <summary>
        /// DrawVerticalSeams method sets the pixels of the vertical seams in the image bitmap
        /// </summary>
        /// <param name="bitmap">Image bitmap</param>
        /// <param name="pixels">Array of pixels containing the vertical seams</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void DrawVerticalSeams(Bitmap bitmap, Pixel[][] pixels, int width, int height)
        {
            int grayColor;
            Color color;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    bool isSeamPixel = pixels[i][j].IsSeamPixel;        
                    color = pixels[i][j].Color;
                    grayColor = (int)Math.Floor(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    bitmap.SetPixel(j, i, (isSeamPixel) ? Color.Red : Color.FromArgb(grayColor, grayColor, grayColor));
                }
            }
        }

        /// <summary>
        /// DrawHorizontalSeams method sets the pixels of the horizontal seams in the image bitmap
        /// </summary>
        /// <param name="bitmap">Image bitmap</param>
        /// <param name="pixels">Array of pixels containing the horizontal seams</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void DrawHorizontalSeams(Bitmap bitmap, Pixel[][] pixels, int width, int height)
        {
            int grayColor;
            Color color;

            for (int j = 0; j < width; j++)
            {
                for (int i = 0; i < height; i++)
                {
                    bool isSeamPixel = pixels[i][j].IsSeamPixel;
                    color = pixels[i][j].Color;
                    grayColor = (int)Math.Floor(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    bitmap.SetPixel(j, i, (isSeamPixel) ? Color.Red : Color.FromArgb(grayColor, grayColor, grayColor));
                }
            }
        }

        /// <summary>
        /// SetCarvedImage method sets the pixels of the carved image into the image bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="carvedPixels"></param>
        public static void SetCarvedImage(Bitmap bitmap, Pixel[][] carvedPixels)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    bitmap.SetPixel(j, i, carvedPixels[i][j].Color);
                }
            }
        }

    }
}
