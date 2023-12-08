using System.Diagnostics;
using Wacton.Unicolour;

namespace UME
{
    internal class Mandelbrot
    {
        private int[, , ] iterationMap;
        private int width, height, maxIterations;
        public Mandelbrot(double centreA, double centreB, double halfRange, int maxIterations, int pixelDepth, Size resolution)
        {
            width = resolution.Width; 
            height = resolution.Height;
            this.maxIterations = maxIterations;

            iterationMap = new int[width, height, pixelDepth * pixelDepth];
            double halfPx = halfRange / height / pixelDepth;
            double ratio = (double)width / height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    double a = map(x, 0, width - 1, -halfRange, halfRange) * ratio;
                    a += centreA;

                    double b = map(y, 0, height - 1, -halfRange, halfRange);
                    b += centreB;

                    if (pixelDepth == 1)
                    {
                        iterationMap[x, y, 0] = getIteration(a + halfPx, b + halfPx, maxIterations);
                    }
                    else
                    {
                        int[] PPIterations = getPPIteration(a, b, maxIterations, pixelDepth, halfPx);
                        for (int i = 0; i < PPIterations.Length; i++)
                        {
                            iterationMap[x, y, i] = PPIterations[i];
                        }
                        
                    }
                        
                }
                //Debug.WriteLine(Math.Round((float)x/width * 100,2) + "%");
            }
            
        }
        public Bitmap getImage(Unicolour[] colourWheel)
        {
            Bitmap img = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color winColour = getColour(x, y, colourWheel);
                    img.SetPixel(x,height - y - 1, winColour);
                }
            }

            return img;
        }

        public Color getColour(int x, int y, Unicolour[] colourWheel)
        {
            Color winCol;

            if (iterationMap.GetLength(2) == 1)
            {
                int iteration = iterationMap[x, y, 0];
                if (iteration == maxIterations)
                {
                    winCol = Color.Black;
                }
                else
                {
                    winCol= Color.White;
                }
            }
            else
            {
                
                int index = (int)map(iterationMap[x, y, 0], maxIterations, 0, 0, colourWheel.Length-1);
                Unicolour mixed = colourWheel[index];

                for (int i = 1; i < iterationMap.GetLength(2); i++)
                {
                    index = (int)map(iterationMap[x, y, i], maxIterations, 0, 0, colourWheel.Length-1);
                    mixed = mixed.Mix(ColourSpace.Rgb255, colourWheel[index]);
                }




                int red = (int)(mixed.Rgb.R * 255);
                int green = (int)(mixed.Rgb.G * 255);
                int blue = (int)(mixed.Rgb.B * 255);
                //Debug.WriteLine("Colour found");
                winCol = Color.FromArgb(red, green, blue);
            }

            return winCol;
        }
        private static int[] getPPIteration(double a, double b, int maxIterations, int pixelDepth, double halfPx)
        {
            int[] iterations = new int[pixelDepth * pixelDepth];

            for (int x = 0; x < pixelDepth; x++)
            {
                for (int y = 0; y < pixelDepth; y++)
                {
                    double aOff = x * halfPx * 2 + halfPx;
                    double bOff = y * halfPx * 2 + halfPx;

                    iterations[x + pixelDepth * y] = getIteration(a + aOff, b + bOff, maxIterations);
                }
            }

            //int averageIterations = (int)Math.Round((float)totalIterations / (pixelDepth * pixelDepth));
            //return averageIterations;
            return iterations;

        }
        private static int getIteration(double a, double b, int maxIterations)
        {
            double Za = 0;
            double Zb = 0;

            for (int i = 0; i < maxIterations; i++)
            {
                double newZa = Za * Za - Zb * Zb;
                double newZb = 2 * Za * Zb;

                Za = newZa + a;
                Zb = newZb + b;

                if (Za * Za + Zb * Zb > 4)
                {
                    return i;
                }
            }
            return maxIterations;
        }
        
        private static double map(double v, double s1, double f1, double s2, double f2)
        {
            double ratio = (v - s1) / (f1 - s1);
            return ratio * (f2 - s2) + s2;
        }
        
    }
}
