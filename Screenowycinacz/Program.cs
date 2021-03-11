using System;
using System.Drawing;
using System.Windows.Forms;

namespace Screenoinator
{
    static class Program
    {
        public static MainForm MainWindow { get; private set; }
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainWindow = new MainForm();
            Application.Run(MainWindow);
        }

        public static Bitmap CropImage(Bitmap source, Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }
        public static Bitmap TakeScreenshot()
        {
            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            int screenWidth = SystemInformation.VirtualScreen.Width;
            int screenHeight = SystemInformation.VirtualScreen.Height;
            return TakeScreenshot(new Rectangle(screenLeft, screenTop, screenWidth, screenHeight));
        }

        public static Bitmap TakeScreenshot(Rectangle rectangle)
        {
            //Console.WriteLine($"{screenLeft} {screenTop} {screenWidth} {screenHeight}");
            Bitmap bmp = new Bitmap(rectangle.Width, rectangle.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rectangle.X, rectangle.Y, 0, 0, bmp.Size);
            }
            return bmp;
        }

        public static int CompareBitmaps(Bitmap b1, Bitmap b2)
        {
            if (b1.Size != b2.Size)
            {
                return 100;
            }
            int counter = 0;
            for (int w = 0; w < b1.Width; w++)
            {
                for (int h = 0; h < b1.Height; h++)
                {
                    if (b1.GetPixel(w, h) != b2.GetPixel(w, h))
                        counter += 1;
                }
            }
            Console.WriteLine(counter);
            return counter * 100 / (b1.Width * b1.Height);
        }
        public static Bitmap StreachBitmapToSize(Bitmap bitmap, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bitmap, new Rectangle(0, 0, width, height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);
            }

            return result;
        }
    }
}
