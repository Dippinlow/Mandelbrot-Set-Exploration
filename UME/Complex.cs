using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UME
{
    internal class Complex
    {
        public double real, imaginary;

        public Complex(double real = 0, double imaginary = 0)
        {
            this.real = real;
            this.imaginary = imaginary;
        }

        public Complex(Complex c)
        {
            real = c.real;
            imaginary = c.imaginary;
        }

        public static double SquareModulus(Complex c)
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
            double newImaginary = c.real * c.imaginary * 2;
            return new Complex(newReal, newImaginary);
        }

    }
}
