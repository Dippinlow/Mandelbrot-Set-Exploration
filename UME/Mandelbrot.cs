using System.Diagnostics;
using Wacton.Unicolour;

namespace UME
{
    internal class Mandelbrot(Location location, Quality quality)
    {
        private Location location = location;
        private Quality quality = quality;
        private double[] reAxis, imAxis;
        private int[,] iterationMap;
        private Unicolour[,] colourMap;
        private int reCount, imCount;
        private Bitmap image;
        //private int brightLimit = quality.maxIteration / 4;

        public Bitmap getImage() { return image; }
        private void paintImage()
        {
            image = colourImage();
        }
        public void process(double pow = 2)
        {
            calculateLocations();

            iterationMap = new int[reCount, imCount];
            colourMap = new Unicolour[reCount, imCount];

            generateIterations(0, 0, reCount, imCount, pow, true);
            paintImage();
        }
        private void calculateLocations()
        {
            reCount = quality.width + 1;
            imCount = quality.height + 1;
            (reAxis, imAxis) = generateLocations(reCount, imCount);
        }
        private Color convertColour(Unicolour c)
        {
            c = c.MapToGamut();
            int r = (int)Math.Round(c.Rgb.R * 255);
            int g = (int)Math.Round(c.Rgb.G * 255);
            int b = (int)Math.Round(c.Rgb.B * 255);
            return Color.FromArgb(r, g, b);
        }
        private Color mixColours(Unicolour[] colours)
        {
            Unicolour c1 = colours[0];
            Unicolour c3 = colours[2];

            c1 = c1.Mix(ColourSpace.Hsb, colours[1]);
            c3 = c3.Mix(ColourSpace.Hsb, colours[3]);
            Unicolour c = c1.Mix(ColourSpace.Hsb, c3);

            return convertColour(c);
        }
        private Unicolour getColour(int iteration)
        {
            double hue = (double)iteration * 20 + 174;// / quality.maxIteration) * 3600;
            double saturation = 0.565;
            double brightness;
            if (iteration == quality.maxIteration)
            {
                brightness = 0;
            }
            else /*if (iteration < brightLimit)
            {
                brightness = iteration / ((double)brightLimit);
            }
            else*/
            {
                brightness = 0.721;
            }
            return new Unicolour(ColourSpace.Hsb, hue, saturation, brightness);
        }
        private Bitmap colourImage()
        {
            Bitmap img = new Bitmap(quality.width, quality.height);

            for (int x = 0; x < quality.width; x++)
            {
                for (int y = 0; y < quality.height; y++)
                {
                    Color c;
                    if (iterationMap[x, y] == iterationMap[x, y + 1]
                        && iterationMap[x, y] == iterationMap[x + 1, y]
                        && iterationMap[x, y] == iterationMap[x + 1, y + 1])
                    {
                        c = convertColour(colourMap[x, y]);
                    }
                    else
                    {
                        Unicolour[] colours =
                        {
                            colourMap[x, y],
                            colourMap[x + 1, y],
                            colourMap[x + 1, y + 1],
                            colourMap[x, y + 1]
                        };



                        c = mixColours(colours);
                    }

                    img.SetPixel(x, y, c);
                }
            }
            return img;
        }
        private void generateIterations(int reIndex, int imIndex, int reCount, int imCount, double pow = 2, bool f = false)
        {
            if (reCount < 3 || imCount < 3)
            {

                for (int i = 0; i < reCount; i++)
                {
                    for (int j = 0; j < imCount; j++)
                    {
                        int x = reIndex + i;
                        int y = imIndex + j;
                        Complex cNumber = new Complex(reAxis[x], imAxis[y]);
                        int it = Complex.Iterate(cNumber, quality.maxIteration, pow);
                        iterationMap[x, y] = it;
                        colourMap[x, y] = getColour(it);
                    }
                }

                return;
            }

            Complex c = new Complex(reAxis[reIndex], imAxis[imIndex]);
            int checkIteration = Complex.Iterate(c, quality.maxIteration, pow);
            Unicolour checkColour = getColour(checkIteration);
            iterationMap[reIndex, imIndex] = checkIteration;
            colourMap[reIndex, imIndex] = checkColour;

            bool valid = true;

            for (int i = 1; i < reCount; i++)
            {
                int x = reIndex + i;
                int y = imIndex;
                Complex c1 = new Complex(reAxis[x], imAxis[y]);
                int it = Complex.Iterate(c1, quality.maxIteration, pow);
                iterationMap[x, y] = it;
                if (it != checkIteration)
                {
                    valid = false;
                    colourMap[x, y] = getColour(it);
                }
                else
                {
                    colourMap[x, y] = checkColour;
                }
            }
            for (int i = 0; i < reCount; i++)
            {
                int x = reIndex + i;
                int y = imIndex + imCount - 1;
                Complex c1 = new Complex(reAxis[x], imAxis[y]);
                int it = Complex.Iterate(c1, quality.maxIteration, pow);
                iterationMap[x, y] = it;
                if (it != checkIteration)
                {
                    valid = false;
                    colourMap[x, y] = getColour(it);
                }
                else
                {
                    colourMap[x, y] = checkColour;
                }
            }
            for (int i = 1; i < imCount - 1; i++)
            {
                int x = reIndex;
                int y = imIndex + i;
                Complex c1 = new Complex(reAxis[x], imAxis[y]);
                int it = Complex.Iterate(c1, quality.maxIteration, pow);
                iterationMap[x, y] = it;
                if (it != checkIteration)
                {
                    valid = false;
                    colourMap[x, y] = getColour(it);
                }
                else
                {
                    colourMap[x, y] = checkColour;
                }
            }
            for (int i = 1; i < imCount - 1; i++)
            {
                int x = reIndex + reCount - 1;
                int y = imIndex + i;
                Complex c1 = new Complex(reAxis[x], imAxis[y]);
                int it = Complex.Iterate(c1, quality.maxIteration, pow);
                iterationMap[x, y] = it;
                if (it != checkIteration)
                {
                    valid = false;
                    colourMap[x, y] = getColour(it);
                }
                else
                {
                    colourMap[x, y] = checkColour;
                }
            }
            if (valid && !f)
            {
                for (int i = 1; i < reCount - 1; i++)
                {
                    int x = i + reIndex;
                    for (int j = 1; j < imCount - 1; j++)
                    {
                        int y = j + imIndex;
                        iterationMap[x, y] = checkIteration;
                        colourMap[x, y] = checkColour;
                    }
                }
                return;
            }
            int x1 = (reCount - 2) - (reCount - 2) / 2;
            int x2 = (reCount - 2) / 2;
            int y1 = (imCount - 2) - (imCount - 2) / 2;
            int y2 = (imCount - 2) / 2;


            Task tA = new Task(() => generateIterations(reIndex + 1, imIndex + 1, x1, y1, pow));
            tA.Start();
            Task tB = new Task(() => generateIterations(reIndex + 1 + x1, imIndex + 1, x2, y1, pow));
            tB.Start();
            Task tC = new Task(() => generateIterations(reIndex + 1 + x1, imIndex + 1 + y1, x2, y2, pow));
            tC.Start();
            Task tD = new Task(() => generateIterations(reIndex + 1, imIndex + 1 + y1, x1, y2, pow));
            tD.Start();

            Task.WaitAll([tA, tB, tC, tD]);

        }

        private (double[], double[]) generateLocations(int reals, int imaginaries)
        {
            double complexWidth, complexHeight;
            double shortSide = 4 / location.zoom;
            double[] realLocations = new double[reals];
            double[] imaginaryLocations = new double[imaginaries];

            if (quality.width > quality.height)
            {
                complexWidth = shortSide * quality.width / quality.height;
                complexHeight = shortSide;
            }
            else
            {
                complexWidth = shortSide;
                complexHeight = shortSide * quality.height / quality.width;
            }

            for (int i = 0; i < reals; i++)
            {
                double xRatio = (double)i / quality.width;
                double real = xRatio * complexWidth - complexWidth / 2;
                real += location.centre.real;
                realLocations[i] = real;
            }
            for (int i = 0; i < imaginaries; i++)
            {
                double yRatio = 1.0 - (double)i / quality.height;
                double imaginary = yRatio * complexHeight - complexHeight / 2;
                imaginary += location.centre.imaginary;
                imaginaryLocations[i] = imaginary;
            }
            return (realLocations, imaginaryLocations);
        }
    }
}
