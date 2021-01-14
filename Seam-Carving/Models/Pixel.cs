using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Seam_Carving.Models
{
    public class Pixel
    {
        public double Energy;
        public Color Color;
        public Color GrayColor;
        public bool IsSeamPixel;
        public int AbsoluteX;
        public int AbsoluteY;
    }
}
