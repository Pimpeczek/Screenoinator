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
    }
}
