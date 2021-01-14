using Seam_Carving.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Seam_Carving.Util
{
    public static class Utility
    {
        public static bool ValidateCoordinate(int x, int y, int width, int height) => 
            x >= 0 && x <= width - 1 && y >= 0 && y <= height - 1;

        public static T[] InitializeArray<T>(int width)
        {
            T[] array = new T[width];
            return array;
        }

        public static T[][] Initialize2DArray<T>(int width, int height)
        {
            T[][] array = new T[height][];
            for (int i = 0; i < height; i++)
            {
                array[i] = new T[width];
            }

            return array;
        }

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
                        GrayColor = bitmap.GetPixel(j, i),
                        IsSeamPixel = false,
                        AbsoluteX = i,
                        AbsoluteY = j,
                    };
                }
            }

            return pixels;
        }

        public static void DrawSeams(Bitmap bitmap, Pixel[][] pixels)
        {
            int width = pixels[0].Length;
            int height = pixels.Length;
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
