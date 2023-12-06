using System.Diagnostics;
using Wacton.Unicolour;

namespace UME
{
    internal class Mandelbrot
    {
        private int[,] iterationMap;
        private int width, height, maxIterations;
        public Mandelbrot(double centreA, double centreB, double halfRange, int maxIterations, int pixelDepth, Size resolution)
        {
            width = resolution.Width; 
            height = resolution.Height;
            this.maxIterations = maxIterations;

            iterationMap = new int[width, height];
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

                    if (pixelDepth < 2)
                    {
                        iterationMap[x, y] = getIteration(a + halfPx, b + halfPx, maxIterations);
                    }
                    else
                    {
                        iterationMap[x, y] = getAverageIteration(a, b, maxIterations, pixelDepth, halfPx);
                    }
                        
                }
                Debug.WriteLine(Math.Round((float)x/width * 100,2) + "%");
            }
            
        }
        public Bitmap getImage(Unicolour[] colourWheel)
        {
            Bitmap img = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int iterations = iterationMap[x, y];
                    Unicolour c;
                    Color winColour;

                    if (iterations == maxIterations)
                    {
                        winColour = Color.Black;
                    }
                    else
                    {
                        int index = (int)map(iterations, maxIterations, 0, 0, colourWheel.Length-1);
                        c = colourWheel[index];
                        int red = (int)(c.Rgb.R * 255);
                        int green = (int)(c.Rgb.G * 255);
                        int blue = (int)(c.Rgb.B * 255);

                        winColour = Color.FromArgb(red, green, blue);
                    }
                    img.SetPixel(x,height - y - 1, winColour);
                }
            }

            return img;
        }
        private static int getAverageIteration(double a, double b, int maxIterations, int pixelDepth, double halfPx)
        {
            int totalIterations = 0;

            for (int x = 0; x < pixelDepth; x++)
            {
                for (int y = 0; y < pixelDepth; y++)
                {
                    double aOff = x * halfPx * 2 + halfPx;
                    double bOff = y * halfPx * 2 + halfPx;

                    totalIterations += getIteration(a + aOff, b + bOff, maxIterations);
                }
            }

            int averageIterations = (int)Math.Round((float)totalIterations / (pixelDepth * pixelDepth));
            return averageIterations;

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
