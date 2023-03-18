namespace UME
{
    internal class HSBColour
    {
        public float hue, saturation, brightness;

        public HSBColour(float hue, float saturation, float brightness)
        {

            this.hue = hue;
            this.saturation = saturation;
            this.brightness = brightness;

        }

        public Color getRGB()
        {
            double chroma = brightness * saturation;
            double hueDash = hue / 60.0;
            double x = chroma * (1 - Math.Abs(hueDash % 2 - 1));
            double r, g, b;

            if (hueDash < 1)
            {
                r = chroma;
                g = x;
                b = 0;
            }
            else if (hueDash < 2)
            {
                r = x;
                g = chroma;
                b = 0;
            }
            else if (hueDash < 3)
            {
                r = 0;
                g = chroma;
                b = x;
            }
            else if (hueDash < 4)
            {
                r = 0;
                g = x;
                b = chroma;
            }
            else if (hueDash < 5)
            {
                r = x;
                g = 0;
                b = chroma;
            }
            else
            {
                r = chroma;
                g = 0;
                b = x;
            }

            double m = brightness - chroma;

            return Color.FromArgb(
                (int)Math.Round((r + m) * 255),
                (int)Math.Round((g + m) * 255),
                (int)Math.Round((b + m) * 255));
        }
    }
}
