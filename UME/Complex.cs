using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UME
{
    internal class Complex(double real = 0, double imaginary = 0)
    {
        public readonly double real = real, imaginary = imaginary;

        public static double ModulusSquared(Complex c)
        {
            return c.real * c.real + c.imaginary * c.imaginary;
        }

        public static Complex operator +(Complex c1, Complex c2)
        {
            return new Complex(c1.real + c2.real, c1.imaginary + c2.imaginary);
        }

        public static Complex Square(Complex c)
        {
            double newReal = c.real * c.real - c.imaginary * c.imaginary;
            double newImaginary = (c.real + c.real) * c.imaginary;// * 2;
            return new Complex(newReal, newImaginary);
        }
        public static Complex Pow(Complex c, double n)
        {
            double r = Math.Sqrt(ModulusSquared(c));
            double rn = Math.Pow(r, n);
            double theta = Math.Atan(Math.Abs(c.imaginary / c.real));
            double arg;
            if (c.real > 0 && c.imaginary > 0)
            {
                arg = theta;
            }
            else if (c.real > 0 && c.imaginary < 0)
            {
                arg = -theta;
            }
            else if (c.real < 0 && c.imaginary > 0)
            {
                arg = Math.PI - theta;
            }
            else
            {
                arg = -Math.PI + theta;
            }
            return new Complex(rn * Math.Cos(n * arg), rn * Math.Sin(n * arg));
        }
        public static int Iterate(Complex c, int maxIt, double n = 2)
        {
            if (n != 2)
            {
                Complex z = new(c.real, c.imaginary);
                for (int i = 1; i < maxIt; i++)
                {
                    z = Pow(z, n) + c;
                    if (ModulusSquared(z) > 4)
                    {
                        return i;
                    }
                }
                return maxIt;
            }
            else
            {
                Complex z = new();
                for (int i = 0; i < maxIt; i++)
                {
                    z = Square(z) + c;
                    if (ModulusSquared(z) > 4)
                    {
                        return i;
                    }
                }
                return maxIt;
            }



        }
    }
}
