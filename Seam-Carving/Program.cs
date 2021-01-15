using Seam_Carving;
using Seam_Carving.Models;
using Seam_Carving.Util;
using SixLabors.ImageSharp.Processing.Processors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Seam_Carving
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 3) throw new Exception("The number of arguments is less than required");
                string[] image = args[0].Split('.');
                string extension = image[image.Length - 1];
                string imageName = "";
                for (int i = 0; i < image.Length - 1; i++) imageName += image[i];
                string imageSource = $"../images/{imageName}.{extension}";
                string carvedImageOutput = $"../images/{imageName}_Carved_Image.{extension}";
                string verticalSeamsOutputImage = $"../images/{imageName}_Vertical_Seams.{extension}";
                string horizontalSeamsOutputImage = $"../images/{imageName}_Horizontal_Seams.{extension}";
                if (!File.Exists(imageSource))
                {
                    throw new Exception("The input file does not exist");
                }
                int verticalDeletingCount = int.Parse(args[1]);
                int horizontalDeletingCount = int.Parse(args[2]);

                Bitmap bitmap = new Bitmap(imageSource);
                int width = bitmap.Width;
                int height = bitmap.Height;

                if (verticalDeletingCount > width || horizontalDeletingCount > height)
                {
                    throw new Exception("The input vertical or horizontal seam is out of range");
                }

                // initialize the values
                Pixel[][] pixels = Utility.GetImagePixels(bitmap);
                Energy energy = new Energy(pixels, bitmap.Width, bitmap.Height);
                Pixel[][] verticallyCarvedPixels = pixels;
                Pixel[][] horizontallyCarvedPixels = pixels;
                SeamCarver seamCarver = new SeamCarver(pixels);

                // start the timer
                Stopwatch stopwatch = Stopwatch.StartNew();

                if (verticalDeletingCount > 0)
                {
                    // set vertical seams
                    for (int k = 0; k < verticalDeletingCount; k++)
                    {
                        verticallyCarvedPixels = energy.ComputeVerticalEnergy(k);
                        seamCarver.FindVerticalSeam(ref verticallyCarvedPixels);
                    }

                    // draw vertical seams and save it
                    Utility.DrawVerticalSeams(bitmap, pixels, bitmap.Width, bitmap.Height);
                    bitmap.Save(verticalSeamsOutputImage);
                    bitmap.Dispose();

                    // decrease the width of the image verticalDeletedCount times
                    width -= verticalDeletingCount;
                }

                if (horizontalDeletingCount > 0)
                {
                    // initialize the values based on the vertically carved image
                    horizontallyCarvedPixels = verticallyCarvedPixels;
                    energy = new Energy(verticallyCarvedPixels, width, height);
                    seamCarver = new SeamCarver(verticallyCarvedPixels);

                    // set horizontal seams
                    for (int k = 0; k < horizontalDeletingCount; k++)
                    {
                        horizontallyCarvedPixels = energy.ComputeHorizontalEnergy(k);
                        seamCarver.FindHorizontalSeam(ref horizontallyCarvedPixels);
                    }

                    // draw horizontal seams and save it
                    bitmap = new Bitmap(width, height);
                    Utility.DrawHorizontalSeams(bitmap, verticallyCarvedPixels, width, height);
                    bitmap.Save(horizontalSeamsOutputImage);
                    bitmap.Dispose();

                    // decrease the height of the image horizontalDeletedCount times
                    height -= horizontalDeletingCount;
                }

                // save the final image
                Bitmap newImage = new Bitmap(width, height);
                Utility.SetCarvedImage(newImage, horizontallyCarvedPixels);
                newImage.Save(carvedImageOutput);

                // stop the timer
                stopwatch.Stop();

                // print the total time to carve the image
                Console.WriteLine("Total time: " + stopwatch.ElapsedMilliseconds / 1000);
                Console.WriteLine("-------------------");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            Console.ReadKey();
        }
    }
}
