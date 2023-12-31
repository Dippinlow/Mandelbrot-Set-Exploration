using System.Diagnostics;
using Wacton.Unicolour;

namespace UME
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Mandelbrot showMandel, printMandel;

        //Unicolour[] printColours = new Unicolour[1000];

        //Unicolour[] showColours = new Unicolour[] {new Unicolour(ColourSpace.Rgb, 1, 1, 1)};
        Quality displayQuality, printQuality;
        Location currentLocation, nextLocation;
        
        ShowForm sf;
        Size printSize;
        Bitmap overlay, mandelImage, printImage;
        int maxIt, mouseX, mouseY, printIndex;
        float zoom, relativeScale;
        double halfRange;
        Complex centre, newCentre;
        Task displayTask, printTask;
        private async void Form1_Load(object sender, EventArgs e)
        {
            ClientSize = new Size(1920 / 2, 1080 / 2);
            printSize = new Size(1920, 1080);

            centre = new Complex();
            newCentre = new Complex();

            mouseX = ClientSize.Width / 2;
            mouseY = ClientSize.Height / 2;

            maxIt = 100;
            halfRange = 2;
            relativeScale = 1;
            zoom = 1;
            printIndex = 0;
            currentLocation = new Location(centre, zoom);
            nextLocation = new Location(centre, zoom);
            displayQuality = new Quality(ClientSize.Width, ClientSize.Height, maxIt);
            printQuality = new Quality(printSize.Width, printSize.Height, maxIt);

            /*
            for (int i = 0; i < printColours.Length; i++)
            {
                double hue = map(i, 0, printColours.Length-1, 0, 360);
                hue += 240;

                double brightness = map(i, printColours.Length * 2/3, printColours.Length - 1, 1, 0);
                printColours[i] = new Unicolour(ColourSpace.Hsb, hue, 1, brightness);
            }

            printColours[0] = new Unicolour(ColourSpace.Hsb, 0, 0, 0);
            */
            displayTask = new Task(processDisplay);
            displayTask.Start();

            displayTask.Wait();
        }

        private void updateOverlay()
        {
            overlay = new Bitmap(ClientSize.Width, ClientSize.Height);
            Graphics g = Graphics.FromImage(overlay);
            Brush b = new SolidBrush(Color.Red);
            Pen p = new Pen(Color.Red);
            Font f = new Font("Arial", 16);

            string s = $"Centre r: {newCentre.real}\n" +
                       $"Centre i: {newCentre.imaginary}\n" +
                       $"Max Iterations: {maxIt}\n" +
                       $"Zoom: {zoom}\n" +
                       $"Height Range: {halfRange * 2}";

            g.DrawString(s, f, b, 0, 0);
            g.FillEllipse(b, mouseX - 4, mouseY - 4, 8, 8);

            if (relativeScale <= 1)
            {
                g.DrawRectangle(p, mouseX - ClientSize.Width / 2 * relativeScale, mouseY - ClientSize.Height / 2 * relativeScale, ClientSize.Width * relativeScale, ClientSize.Height * relativeScale);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(mandelImage, 0, 0);
            e.Graphics.DrawImage(overlay, 0, 0);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            double ratio = (double)ClientSize.Width / ClientSize.Height;
            double realMap = map(e.X, 0, ClientSize.Width, -halfRange, halfRange) * ratio;
            double imaginaryMap = map(e.Y, 0, ClientSize.Height, -halfRange, halfRange);

            newCentre = centre + new Complex(realMap, -imaginaryMap);
            mouseX = e.X;
            mouseY = e.Y;

            updateOverlay();
            Invalidate();
        }



        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    e.IsInputKey = true;
                    break;

                case Keys.Up:
                    e.IsInputKey = true;
                    break;
            }
        }

        private void processDisplay()
        {
            centre = newCentre;
            currentLocation = new Location(newCentre, zoom);
            showMandel = new Mandelbrot(currentLocation, displayQuality);
            showMandel.process(1.5);
            mandelImage = showMandel.getImage();
            mouseX = ClientSize.Width / 2;
            mouseY = ClientSize.Height / 2;
            relativeScale = 1;
            updateOverlay();
            Invalidate();
        }
        private void processPrint()
        {
            Debug.WriteLine("Building iteration map...");
            printMandel = new Mandelbrot(currentLocation, printQuality);
            printMandel.process(1.5);
            printImage = printMandel.getImage();
            printImage.Save($"Prints\\{printIndex}, {(float)centre.real},{(float)centre.imaginary}Mandelbrot.png");
            Debug.WriteLine("Print Saved");
            printIndex++;
            Debug.WriteLine(printIndex);
        }

        private void showPrint()
        {
            Debug.WriteLine("Showing Print");
            sf = new ShowForm()
            {
                ClientSize = printSize,
                //WindowState = FormWindowState.Maximized
            };
            sf.setPicture(printImage);
            sf.Show();
            Console.Beep();
        }

        private async void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    displayTask = new Task(processDisplay);
                    displayTask.Start();

                    break;

                case Keys.P:
                    printTask = new Task(processPrint);
                    printTask.Start();

                    printTask.GetAwaiter().OnCompleted(showPrint);


                    break;

                case Keys.S:
                    StreamWriter fileWriter = new StreamWriter("data.txt");
                    fileWriter.WriteLine(centre.real);
                    fileWriter.WriteLine(centre.imaginary);
                    fileWriter.WriteLine(halfRange);
                    fileWriter.WriteLine(maxIt);
                    fileWriter.Close();
                    Debug.WriteLine("Data saved!");
                    break;

                case Keys.Up:
                    halfRange /= 2;
                    relativeScale /= 2;
                    zoom *= 2;
                    break;

                case Keys.Down:
                    halfRange *= 2;
                    relativeScale *= 2;
                    zoom /= 2;
                    break;

                case Keys.Right:
                    maxIt += 20;
                    break;

                case Keys.Left:
                    maxIt -= 20;
                    break;

                case Keys.Q:
                    maxIt -= 1;
                    break;

                case Keys.E:
                    maxIt += 1;
                    break;
            }
            updateOverlay();
            Invalidate();
        }

        private static double map(double v, double s1, double f1, double s2, double f2)
        {
            double ratio = (v - s1) / (f1 - s1);
            return ratio * (f2 - s2) + s2;
        }
    }
}