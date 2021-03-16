using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Screenoinator
{
    static class Program
    {
        public static MainForm MainWindow { get; private set; }
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        private static readonly SolidBrush watermarkBlackBrush = new SolidBrush(Color.FromArgb(128, 16, 16, 16));
        private static readonly SolidBrush watermarkWhiteBrush = new SolidBrush(Color.FromArgb(128, Color.White));
        private static GraphicsPath watermarkPath;
        private static Size pathBitmapSize;
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

        public static Bitmap ResizeToFit(Bitmap bitmap, int width, int height)
        {
            if (bitmap == null)
            {
                return null;
            }
            float dRatio = (float)width / (float)height;
            float bRatio = (float)bitmap.Width / (float)bitmap.Height;
            if (dRatio < bRatio)
            {
                return new Bitmap(bitmap, width, (int)(width / bRatio));
            }
            else
            {
                return new Bitmap(bitmap, (int)(height * bRatio), (int)(height));
            }

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

        public static void ApplyWatermark(Bitmap bitmap)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                StringFormat format = new StringFormat()
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Far
                };
                RectangleF rectf = new RectangleF(0, 0, bitmap.Width, bitmap.Height);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                
                
                g.FillPath(watermarkBlackBrush, CreateWatermarkPath(bitmap));
                g.DrawString("Screenoinator", new Font("Consolas", 9), watermarkWhiteBrush, rectf, format);
            }
        }

        private static GraphicsPath CreateWatermarkPath(Bitmap bitmap)
        {
            if (pathBitmapSize != null && pathBitmapSize != bitmap.Size)
            {
                return Program.watermarkPath;
            }
            pathBitmapSize = new Size(bitmap.Width, bitmap.Height);
            Size size = new Size(105, 18);
            Rectangle r = new Rectangle(bitmap.Width - size.Width, bitmap.Height - size.Height, size.Width, size.Height);
            GraphicsPath newWatermarkPath = new GraphicsPath();
            newWatermarkPath.AddArc(new RectangleF(r.X, r.Y, r.Height * 2, r.Height * 2), 180, 90);
            newWatermarkPath.AddLine(r.X + r.Height, r.Y, r.X + r.Width, r.Y);
            newWatermarkPath.AddLine(r.X + r.Width, r.Y, r.X + r.Width, r.Y + r.Height);
            newWatermarkPath.AddLine(r.X, r.Y + r.Height, r.X + r.Width, r.Y + r.Height);
            newWatermarkPath.CloseFigure();
            return newWatermarkPath;
        }
    }
}
