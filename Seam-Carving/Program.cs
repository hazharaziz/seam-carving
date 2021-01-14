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
            Stopwatch stopwatch = Stopwatch.StartNew();
            string imageExtension = "png";
            int imageNumer = 15;
            string imageSource = $"../../../../../images/In_{imageNumer}.{imageExtension}";
            string imageOutputCarved = $"../../../../../images/Out_{imageNumer}_Carved.{imageExtension}";
            string imageOutputSeams = $"../../../../../images/Out_{imageNumer}_Seams.{imageExtension}";

            Bitmap bitmap = new Bitmap(imageSource);
            int width = bitmap.Width;
            int height = bitmap.Height;
            Pixel[][] pixels = Utility.GetImagePixels(bitmap);  
            Energy energy = new Energy(pixels, bitmap.Width, bitmap.Height);
            Pixel[][] carvedPixels = pixels;
            SeamCarver seamCarver = new SeamCarver(pixels);
            int number = 100;


            for (int k = 0; k < number; k++)
            {
                carvedPixels = energy.ComputeEnergy(k);
                seamCarver.FindSeam(ref carvedPixels);
            }


            Utility.DrawSeams(bitmap, pixels);
            bitmap.Save(imageOutputSeams);
            bitmap.Dispose();


            Bitmap newImage = new Bitmap(width - number, height);
            Utility.SetCarvedImage(newImage, carvedPixels);

            newImage.Save(imageOutputCarved);

            stopwatch.Stop();
            Console.WriteLine("Total time: " + stopwatch.ElapsedMilliseconds / 1000);

            Console.WriteLine("-------------------");


            Console.ReadKey();
        }



    }
}
