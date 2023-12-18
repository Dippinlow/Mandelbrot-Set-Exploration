using System.Diagnostics;
using Wacton.Unicolour;

namespace UME
{
    internal class Mandelbrot
    {
        private int[, , ] iterationMap;
        private int width, height, maxIterations;
        private Complex centre;
        private double halfRange;
        public Mandelbrot(Complex centre, double halfRange, int maxIterations, int pixelDepth, Size resolution)
        {
            width = resolution.Width; 
            height = resolution.Height;
            this.centre = centre;
            this.maxIterations = maxIterations;
            this.halfRange = halfRange;
            iterationMap = new int[width, height, pixelDepth * pixelDepth];
            double halfPx = halfRange / height / pixelDepth;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    Complex c = canvasToComplex(x, y);

                    if (pixelDepth == 1)
                    {
                        iterationMap[x, y, 0] = getIteration(c + new Complex(halfPx, halfPx), maxIterations);
                    }
                    else
                    {
                        int[] PPIterations = getPPIteration(c, maxIterations, pixelDepth, halfPx);
                        for (int i = 0; i < PPIterations.Length; i++)
                        {
                            iterationMap[x, y, i] = PPIterations[i];
                        }
                        
                    }
                        
                }
                Debug.WriteLine(Math.Round((float)x/width * 100,2) + "%");
            }
            
        }
        private Complex canvasToComplex(float x, float y)
        {
            double complexWidth, complexHeight;
            //double shortSide = (double)4 / zoom;
            double shortSide = 2 * halfRange;
            if (width > height)
            {
                complexWidth = shortSide * width / height;
                complexHeight = shortSide;
            }
            else
            {
                complexWidth = shortSide;
                complexHeight = shortSide * height / width;
            }

            double xRatio = x / width;
            double yRatio = 1 - y / height;

            double real = xRatio * complexWidth;
            double imaginary = yRatio * complexHeight;

            real -= complexWidth / 2;
            imaginary -= complexHeight / 2;


            Complex c = new Complex(real, imaginary);
            c += centre;
            

            return c;
        }


        public Bitmap getImage(Unicolour[] colourWheel)
        {
            Bitmap img = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color winColour = getColour(x, y, colourWheel);
                    img.SetPixel(x,y, winColour);
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
                winCol = Color.FromArgb(red, green, blue);
            }

            return winCol;
        }
        private static int[] getPPIteration(Complex c, int maxIterations, int pixelDepth, double halfPx)
        {
            int[] iterations = new int[pixelDepth * pixelDepth];

            for (int x = 0; x < pixelDepth; x++)
            {
                for (int y = 0; y < pixelDepth; y++)
                {
                    double aOff = x * halfPx * 2 + halfPx;
                    double bOff = y * halfPx * 2 + halfPx;

                    iterations[x + pixelDepth * y] = getIteration(c + new Complex(aOff, bOff), maxIterations);
                }
            }
            return iterations;

        }
        private static int getIteration(Complex c, int maxIterations)
        {
            Complex z = new Complex();

            for (int i = 0; i < maxIterations; i++)
            {
                z = Complex.Square(z) + c;

                if (Complex.SquareModulus(z) > 4)
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
