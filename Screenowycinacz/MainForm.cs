using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Screenoinator
{
    public partial class MainForm : Form
    {
        private OpenFileDialog openFileDialog1;
        private List<string> files;
        private Rectangle cutRectangle;
        private Bitmap currentBitmap;
        private Stopwatch stopwatchX;
        private Stopwatch stopwatchY;
        private Stopwatch stopwatchWidth;
        private Stopwatch stopwatchHeight;
        private int counterX = 0;
        private int counterY = 0;
        private int counterWidth = 0;
        private int counterHeight = 0;
        private int incrementDivider = 5;
        private int patience = 300;
        private int speedDivider = 200;
        private int speedPart = 2;
        private string outputFolder = null;
        private float scale;
        bool mouseDown;
        bool mouseOverPicturebox;
        private Point beginPoint;
        private Point endPoint;
        private float zoom;
        bool screenshotsRunning;
        private BackgroundWorker screeningWorker;
        private Bitmap previousSmlScreen;
        private Bitmap previousScreen;
        bool hasBasicScreenshot;
        Point virtualScreenOffset;
        int screensTaken;
        int screensSaved;
        public MainForm()
        {
            scale = 1;
            mouseDown = false;
            stopwatchX = new Stopwatch();
            stopwatchX.Restart();
            stopwatchY = new Stopwatch();
            stopwatchY.Restart();
            stopwatchWidth = new Stopwatch();
            stopwatchWidth.Restart();
            stopwatchHeight = new Stopwatch();
            stopwatchHeight.Restart();
            cutRectangle = new Rectangle(0,0,100,100);
            files = new List<string>();
            InitializeOpenFileDialog();
            InitializeComponent();
            screeningWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            screeningWorker.DoWork +=
                new DoWorkEventHandler(screeningWorker_DoWork);
            screeningWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(screeningWorker_RunWorkerCompleted);
            screeningWorker.ProgressChanged +=
                new ProgressChangedEventHandler(screeningWorker_ProgressChanged);
            toolTip1.SetToolTip(numUD_treshold, "Percent of different pixels (after downsampling)\nthat the screenshots have to differ to be saved.\nNote:\nEven the marginal changes are accounted for:\ni.e. colors #000000 and #000001 are considered different");
            toolTip1.SetToolTip(numUD_interval, "The amount of seconds between each screenshot.");
            toolTip1.SetToolTip(checkBox_shade, "Draws a shade around selected area.\n(May cause lag)");
            toolTip1.SetToolTip(button_output, "Folder to store the cropped screenshots.");
            toolTip1.SetToolTip(button_outputAuto, "Folder to store the cropped screenshots.");
            toolTip1.SetToolTip(button_baseScreen, "Base screenshot is used for selecting the observed region.");

            screenshotsRunning = false;

            UpdateButtons();
        }
        private void InitializeOpenFileDialog()
        {
            this.openFileDialog1 = new OpenFileDialog();
            // Set the file dialog to filter for graphics files.
            this.openFileDialog1.Filter =
                "Images (*.BMP;*.JPG;*.JPEG;*.PNG)|*.BMP;*.JPG;*.JPEG;*.PNG|" +
                "All files (*.*)|*.*";

            // Allow the user to select multiple images.
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "My Image Browser";
        }

        private void UpdateRectangle()
        {
            cutRectangle.X = (int)numUD_X.Value;
            cutRectangle.Y = (int)numUD_Y.Value;
            cutRectangle.Width = (int)numUD_width.Value;
            cutRectangle.Height = (int)numUD_height.Value;
        }

        private void Button_select_Click(object sender, EventArgs e)
        {
            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                files = new List<string>(openFileDialog1.FileNames);
                UpdateButtons();
                this.label_filecount.Text = $"Files: {files.Count}";
            }
            else
            {
                return;
            }
            Bitmap b = new Bitmap(files[0]);
            Size baseSize = b.Size;
            this.label_screensize.Text = $"Screen size: {baseSize.Width} x {baseSize.Height}";
            foreach (var f in files)
            {
                b = new Bitmap(f);
                if(baseSize != b.Size)
                {
                    MessageBox.Show("Wszystkie obrazy muszą mieć takie same wymiary.", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ClearFiles();
                    break;
                }
            }
            currentBitmap = b;

            ShowCurrentImage();
        }

        void ShowCurrentImage()
        {
            if (currentBitmap == null)
                return;
            numUD_width.Maximum = currentBitmap.Width;
            numUD_height.Maximum = currentBitmap.Height;

            UpdatePositions();

            ShowImage(currentBitmap);
        }

        private void UpdateButtons()
        {
            button_process.Enabled = outputFolder != null && files.Count > 0;
            button_enablescreenshots.Enabled = hasBasicScreenshot && !screenshotsRunning && outputFolder != null;
            button_stopscreenshots.Enabled = hasBasicScreenshot && screenshotsRunning;
            button_baseScreen.Enabled = !screenshotsRunning;
        }

        private void ClearImages()
        {
            pb_overview.Image = null;
            pb_overview.BackgroundImage = null;
            currentBitmap = null;
        }

        private void UpdatePositions()
        {
            numUD_X.Maximum = currentBitmap.Width - numUD_width.Value;
            numUD_Y.Maximum = currentBitmap.Height - numUD_height.Value;
        }

        public Bitmap ResizeToFit(Bitmap bitmap, int width, int height)
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

        private void ShowImage(Bitmap b)
        {
            if (b == null)
                return;
            try
            {
                this.pb_overview.BackgroundImage = ResizeToFit(b, this.pb_overview.Width, this.pb_overview.Height);
                ApplyFrame();
            }
            catch
            {

            }
        }

        private void ApplyFrame()
        {
            Bitmap b = new Bitmap(pb_overview.BackgroundImage.Width, pb_overview.BackgroundImage.Height);
            scale = b.Width / (float)currentBitmap.Width;
            int penWidth = Math.Max(1, Math.Max(pb_overview.BackgroundImage.Width, pb_overview.BackgroundImage.Height) / 400);
            Pen pen = new Pen(Color.Red, penWidth);
            SolidBrush outsideBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
            SolidBrush eraser = new SolidBrush(Color.FromArgb(0, 255, 255, 255));
            SolidBrush insideBrush = new SolidBrush(Color.FromArgb(24, 0, 0, 0));
            Rectangle fRec = new Rectangle(
                    (int)(cutRectangle.X * scale),
                    (int)(cutRectangle.Y * scale),
                    (int)(cutRectangle.Width * scale) + penWidth,
                    (int)(cutRectangle.Height * scale) + penWidth);
            using (Graphics g = Graphics.FromImage(b))
            {
                if (checkBox_shade.Checked || screenshotsRunning)
                {
                    g.FillRectangle(outsideBrush, 0, 0, b.Width, fRec.Y);
                    g.FillRectangle(outsideBrush, 0, fRec.Y + fRec.Height, b.Width, b.Height - fRec.Y - fRec.Height);
                    g.FillRectangle(outsideBrush, 0, fRec.Y, fRec.X, fRec.Height);
                    g.FillRectangle(outsideBrush, fRec.X + fRec.Width, fRec.Y, b.Width - fRec.X - fRec.Width, fRec.Height);
                }
                //g.FillRectangle(insideBrush, fRec);
                g.DrawRectangle(pen, fRec);
            }
            pb_overview.Image = b;
            label_spos.Text = $"Region position: {cutRectangle.X}x{cutRectangle.Y}";
            label_ssize.Text = $"Region size: {cutRectangle.Width}x{cutRectangle.Height}";
        }

        private void Button_clear_Click(object sender, EventArgs e)
        {
            ClearFiles();
        }

        private void ClearFiles()
        {
            files.Clear();
            this.label_filecount.Text = $"Files: {files.Count}";
            this.label_screensize.Text = $"Screen size: (?)";
            ClearImages();
        }

        private void Button_output_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    outputFolder = fbd.SelectedPath;
                    textBox_outputFolder.Text = outputFolder;
                    textBox_outputAuto.Text = outputFolder;
                    UpdateButtons();
                }
            }
        }

        

        private void Button_process_Click(object sender, EventArgs e)
        {
            var m = new CroppingProgressForm();
            m.Show();
            m.Go(files, cutRectangle, outputFolder);
            this.Enabled = false;
        }

        private void NumUD_width_ValueChanged(object sender, EventArgs e)
        {
            if (mouseDown || currentBitmap == null)
                return;
            if (stopwatchWidth.ElapsedMilliseconds < patience)
            {
                counterWidth += Math.Max(currentBitmap.Width / speedDivider, 1);
            }
            else
            {
                counterWidth = 0;
            }
            numUD_width.Increment = 1 + Math.Min(counterWidth / incrementDivider, currentBitmap.Width / speedPart);
            UpdatePositions();
            UpdateRectangle();
            ShowImage(currentBitmap);
            stopwatchWidth.Restart();
            Console.WriteLine(numUD_width.Increment);
        }

        private void NumUD_height_ValueChanged(object sender, EventArgs e)
        {
            if (mouseDown || currentBitmap == null)
                return;
            if (stopwatchHeight.ElapsedMilliseconds < patience)
            {
                counterHeight += Math.Max(currentBitmap.Height / speedDivider, 1);
            }
            else
            {
                counterHeight = 0;
            }
            numUD_height.Increment = 1 + Math.Min(counterHeight / incrementDivider, currentBitmap.Height / speedPart);
            UpdatePositions();
            UpdateRectangle();
            ShowImage(currentBitmap);
            stopwatchHeight.Restart();
            Console.WriteLine(currentBitmap.Height / speedPart);
            Console.WriteLine(numUD_height.Increment);
        }

        private void NumUD_X_ValueChanged(object sender, EventArgs e)
        {
            if (mouseDown || currentBitmap == null)
                return;
            if (stopwatchX.ElapsedMilliseconds < patience)
            {
                counterX += Math.Max(currentBitmap.Width / speedDivider, 1);
            }
            else
            {
                counterX = 0;
            }
            numUD_X.Increment = 1 + Math.Min(counterX / incrementDivider, currentBitmap.Width / speedPart);
            UpdateRectangle();
            ShowImage(currentBitmap);
            stopwatchX.Restart();
        }

        private void NumUD_Y_ValueChanged(object sender, EventArgs e)
        {
            if (mouseDown || currentBitmap == null)
                return;
            if (stopwatchY.ElapsedMilliseconds < patience)
            {
                counterY += Math.Max(currentBitmap.Height / speedDivider, 1);
            }
            else
            {
                counterY = 0;
            }
            numUD_Y.Increment = 1 + Math.Min(counterY / incrementDivider, currentBitmap.Height / speedPart);
            UpdateRectangle();
            ShowImage(currentBitmap);
            stopwatchY.Restart();
        }

        private void Pb_overview_SizeChanged(object sender, EventArgs e)
        {
            ShowImage(currentBitmap);
        }

        private void Pb_overview_MouseDown(object sender, MouseEventArgs e)
        {
            if (currentBitmap == null || !IsMouseOver() || screenshotsRunning)
                return;
            mouseDown = true;
            beginPoint = e.Location;
            numUD_X.Value = Math.Min((int)(e.X / scale), numUD_X.Maximum);
            numUD_Y.Value = Math.Min((int)(e.Y / scale), numUD_Y.Maximum);
            numUD_width.Value = 1;
            numUD_height.Value = 1;
            UpdatePositions();
            UpdateRectangle();
            ApplyFrame();
        }

        private void Pb_overview_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseDown != true || currentBitmap == null)
                return;
            MouseOverPictureUp(e.Location);
            mouseDown = false;
        }

        void MouseOverPictureUp(Point e)
        {
            endPoint = e;
            if (e.X >= (int)beginPoint.X)
            {
                numUD_width.Value = Math.Min((int)((e.X - beginPoint.X) / scale), numUD_width.Maximum - (int)(beginPoint.X / scale));
            }
            else
            {
                numUD_X.Value = Math.Min(Math.Max(0, (int)(endPoint.X / scale)), numUD_X.Maximum);
                numUD_width.Value = Math.Min((int)((beginPoint.X - e.X) / scale), numUD_width.Maximum);
            }

            if (e.Y>= (int)beginPoint.Y)
            {
                numUD_height.Value = Math.Min((int)((e.Y - beginPoint.Y) / scale), numUD_height.Maximum - (int)(beginPoint.Y / scale));
            }
            else
            {
                numUD_Y.Value = Math.Min(Math.Max(0, (int)(endPoint.Y / scale)), numUD_Y.Maximum);
                numUD_height.Value = Math.Min((int)((beginPoint.Y - e.Y) / scale), numUD_height.Maximum);
            }
            UpdateRectangle();
            ApplyFrame();
        }

        private void Pb_overview_MouseLeave(object sender, EventArgs e)
        {
            mouseOverPicturebox = false;
            if (mouseDown)
            {
                MouseOverPictureUp(pb_overview.PointToClient(MousePosition));
                mouseDown = false;
            }
        }

        bool IsMouseOver()
        {
            Point p = pb_overview.PointToClient(MousePosition);
            return mouseOverPicturebox && p.X >= 0 && p.Y >= 0 && p.X < currentBitmap.Width * scale && p.Y < currentBitmap.Height * scale;
        }

        private void Pb_overview_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown )
            {
                MouseOverPictureUp(e.Location);
                
            }

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void CheckBox_shade_CheckedChanged(object sender, EventArgs e)
        {
            ApplyFrame();
        }

        private void Button_enablescreenshots_Click(object sender, EventArgs e)
        {
            screenshotsRunning = true;
            UpdateButtons();
            currentBitmap = TakeScreenshot();
            
            ShowCurrentImage();
            screensTaken = 0;
            screensSaved = 0;
            label_taken.Text = $"Screenshots taken: {screensTaken}";
            label_saved.Text = $"Screenshots saved: {screensSaved}";
            
            pb_cropped.Visible = true;
            while (screeningWorker.IsBusy) { }
            screeningWorker.RunWorkerAsync();
        }

        private void Button_stopscreenshots_Click(object sender, EventArgs e)
        {
            StopRunningScreenshots();
        }

        void StopRunningScreenshots()
        {
            if(screeningWorker.IsBusy)
            {
                screeningWorker.CancelAsync();
            }
            screenshotsRunning = false;
            ShowCurrentImage();
            UpdateButtons();
            pb_cropped.Visible = false;
            
        }

        Bitmap TakeScreenshot()
        {
            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            int screenWidth = SystemInformation.VirtualScreen.Width;
            int screenHeight = SystemInformation.VirtualScreen.Height;
            return TakeScreenshot(new Rectangle(screenLeft, screenTop, screenWidth, screenHeight));
        }

        Bitmap TakeScreenshot(Rectangle rectangle)
        {
            //Console.WriteLine($"{screenLeft} {screenTop} {screenWidth} {screenHeight}");
            Bitmap bmp = new Bitmap(rectangle.Width, rectangle.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rectangle.X, rectangle.Y, 0, 0, bmp.Size);
            }
            return bmp;
        }

        public Bitmap StreachToSize(Bitmap bitmap, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bitmap, new Rectangle(0, 0, width, height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);
            }

            return result;
        }

        private void screeningWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while(screenshotsRunning)
            {
                Console.WriteLine("Print");
                if (screenshotsRunning)
                {
                    //var bmp = Program.CropImage(TakeScreenshot(), cutRectangle);
                    var bmp = TakeScreenshot(
                        new Rectangle(
                            cutRectangle.X - virtualScreenOffset.X, 
                            cutRectangle.Y - virtualScreenOffset.Y, 
                            cutRectangle.Width, 
                            cutRectangle.Height)
                        );
                    var sml = StreachToSize(bmp, bmp.Width / 100, bmp.Height / 100);
                    screensTaken += 1;
                    if (previousSmlScreen == null || Compare(sml, previousSmlScreen) > numUD_treshold.Value)
                    {

                        if (previousSmlScreen != null)
                            previousSmlScreen.Dispose();
                        if (previousScreen != null)
                            previousScreen.Dispose();
                        previousScreen = bmp;
                        previousSmlScreen = sml;
                        screensSaved += 1;
                        var timeStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                        bmp.Save(Path.Combine(outputFolder, $"{timeStr}.png"));
                    }
                    

                    
                }
                screeningWorker.ReportProgress(0);
                for(int i = (int)numUD_interval.Value * 10; screenshotsRunning && i >= 0; i--)
                    System.Threading.Thread.Sleep(100);
            }

            screeningWorker.Dispose();
        }

        private int Compare(Bitmap b1, Bitmap b2)
        {
            if(b1.Size != b2.Size)
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

        private void screeningWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label_taken.Text = $"Screenshots taken: {screensTaken}";
            label_saved.Text = $"Screenshots saved: {screensSaved}";
            pb_cropped.Image = previousScreen;
        }

        // This event handler deals with the results of the background operation.
        private void screeningWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void Button_baseScreen_Click(object sender, EventArgs e)
        {
            
            currentBitmap = TakeScreenshot();
            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            virtualScreenOffset = new Point(-screenLeft, -screenTop);
            ShowCurrentImage();
            hasBasicScreenshot = true;
            UpdateButtons();
        }


        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                StopRunningScreenshots();
                pb_cropped.Image = null;
                if(previousScreen != null)
                    previousScreen.Dispose();
                if (previousScreen != null)
                    previousSmlScreen.Dispose();
                
            }
        }

        private void Pb_overview_MouseEnter(object sender, EventArgs e)
        {
            mouseOverPicturebox = true;
        }

        private void Button_outputAuto_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    outputFolder = fbd.SelectedPath;
                    textBox_outputAuto.Text = outputFolder;
                    textBox_outputFolder.Text = outputFolder;
                    UpdateButtons();
                }

            }
        }
    }
}
