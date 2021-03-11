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
        private OpenFileDialog ofd;
        private List<string> files;
        private Rectangle cutRectangle;
        private Bitmap currentBitmap;
        private int incrementDivider = 5;
        private int patience = 300;
        private int speedDivider = 200;
        private int speedPart = 2;
        private string outputFolder = null;
        private float scale;
        bool mouseDown;
        bool mouseOverPicturebox;
        private Point selectionBeginPoint;
        private Point selectionEndPoint;
        bool screenshotWorkerDoWork;
        private BackgroundWorker screenshotWorker;
        private Bitmap previousSmlScreen;
        private Bitmap previousScreen;
        bool hasBasicScreenshot;
        Point virtualScreenOffset;
        int screenshotsTaken;
        int screenshotsSaved;
        public MainForm()
        {
            InitializeComponent();
            virtualScreenOffset = new Point();
            screenshotsTaken = 0;
            screenshotsSaved = 0;
            hasBasicScreenshot = false;
            mouseDown = false;
            cutRectangle = new Rectangle(0, 0, 100, 100);
            files = new List<string>();
            InitializeOpenFileDialog();
            InitializeWorker();
            InitializeTooltips();
            InitializeNumUDs();
            UpdateButtons();
        }

        private void InitializeNumUDs()
        {
            NumericUpDownAcceleration[] numericUpDownAccelerations = new NumericUpDownAcceleration[4]
            {
                new NumericUpDownAcceleration(1, 10),
                new NumericUpDownAcceleration(1, 20),
                new NumericUpDownAcceleration(1, 40),
                new NumericUpDownAcceleration(1, 80)
            };
            numUD_height.Accelerations.AddRange(numericUpDownAccelerations);
            numUD_width.Accelerations.AddRange(numericUpDownAccelerations);
            numUD_X.Accelerations.AddRange(numericUpDownAccelerations);
            numUD_Y.Accelerations.AddRange(numericUpDownAccelerations);
        }

        private void InitializeWorker()
        {
            screenshotWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            screenshotWorker.DoWork +=
                new DoWorkEventHandler(screenshotWorker_DoWork);
            screenshotWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(screenshotWorker_RunWorkerCompleted);
            screenshotWorker.ProgressChanged +=
                new ProgressChangedEventHandler(screenshotWorker_ProgressChanged);
            screenshotWorkerDoWork = false;
        }

        private void InitializeTooltips()
        {
            toolTip1.SetToolTip(numUD_treshold, "Percent of different pixels (after downsampling)\nthat the screenshots have to differ to be saved.\nNote:\nEven the marginal changes are accounted for:\ni.e. colors #000000 and #000001 are considered different");
            toolTip1.SetToolTip(numUD_interval, "The amount of seconds between each screenshot.");
            toolTip1.SetToolTip(checkBox_shade, "Draws a shade around selected area.\n(May cause lag)");
            toolTip1.SetToolTip(button_output, "Folder to store the cropped screenshots.");
            toolTip1.SetToolTip(button_outputAuto, "Folder to store the cropped screenshots.");
            toolTip1.SetToolTip(button_baseScreen, "Base screenshot is used for selecting the observed region.");
        }

        private void InitializeOpenFileDialog()
        {
            this.ofd = new OpenFileDialog
            {
                Filter =
                "Images (*.BMP;*.JPG;*.JPEG;*.PNG)|*.BMP;*.JPG;*.JPEG;*.PNG|" +
                "All files (*.*)|*.*",
                Multiselect = true,
                Title = "Select files"
            };
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
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                files = new List<string>(ofd.FileNames);
                UpdateButtons();
                this.label_filecount.Text = $"Files: {files.Count}";
            }
            else
            {
                return;
            }
            Bitmap b = new Bitmap(files[0]);
            Size baseSize = b.Size;
            label_screensize.Text = $"Screen size: {baseSize.Width} x {baseSize.Height}";
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
            button_enablescreenshots.Enabled = hasBasicScreenshot && !screenshotWorkerDoWork && outputFolder != null;
            button_stopscreenshots.Enabled = hasBasicScreenshot && screenshotWorkerDoWork;
            button_baseScreen.Enabled = !screenshotWorkerDoWork;
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
                if (checkBox_shade.Checked || screenshotWorkerDoWork)
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

        private void ClearFiles()
        {
            files.Clear();
            this.label_filecount.Text = $"Files: {files.Count}";
            this.label_screensize.Text = $"Screen size: (?)";
            ClearImages();
        }

        bool IsMouseOver()
        {
            Point p = pb_overview.PointToClient(MousePosition);
            return mouseOverPicturebox && p.X >= 0 && p.Y >= 0 && p.X < currentBitmap.Width * scale && p.Y < currentBitmap.Height * scale;
        }

        void MouseOverPictureUp(Point e)
        {
            selectionEndPoint = e;
            if (e.X >= (int)selectionBeginPoint.X)
            {
                numUD_width.Value = Math.Min((int)((e.X - selectionBeginPoint.X) / scale), numUD_width.Maximum - (int)(selectionBeginPoint.X / scale));
            }
            else
            {
                numUD_X.Value = Math.Min(Math.Max(0, (int)(selectionEndPoint.X / scale)), numUD_X.Maximum);
                numUD_width.Value = Math.Min((int)((selectionBeginPoint.X - e.X) / scale), numUD_width.Maximum);
            }

            if (e.Y >= (int)selectionBeginPoint.Y)
            {
                numUD_height.Value = Math.Min((int)((e.Y - selectionBeginPoint.Y) / scale), numUD_height.Maximum - (int)(selectionBeginPoint.Y / scale));
            }
            else
            {
                numUD_Y.Value = Math.Min(Math.Max(0, (int)(selectionEndPoint.Y / scale)), numUD_Y.Maximum);
                numUD_height.Value = Math.Min((int)((selectionBeginPoint.Y - e.Y) / scale), numUD_height.Maximum);
            }
            UpdateRectangle();
            ApplyFrame();
        }

        private void Button_clear_Click(object sender, EventArgs e)
        {
            ClearFiles();
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
            var m = new CroppingProgressForm(files, cutRectangle, outputFolder);
            m.Show();
            Enabled = false;
        }

        private void Button_enablescreenshots_Click(object sender, EventArgs e)
        {
            screenshotWorkerDoWork = true;
            UpdateButtons();
            currentBitmap = Program.TakeScreenshot();

            ShowCurrentImage();
            screenshotsTaken = 0;
            screenshotsSaved = 0;
            label_taken.Text = $"Screenshots taken: {screenshotsTaken}";
            label_saved.Text = $"Screenshots saved: {screenshotsSaved}";

            pb_cropped.Visible = true;
            while (screenshotWorker.IsBusy) { }
            screenshotWorker.RunWorkerAsync();
        }

        private void Button_stopscreenshots_Click(object sender, EventArgs e)
        {
            StopRunningScreenshots();
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

        private void Button_baseScreen_Click(object sender, EventArgs e)
        {

            currentBitmap = Program.TakeScreenshot();
            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            virtualScreenOffset = new Point(-screenLeft, -screenTop);
            ShowCurrentImage();
            hasBasicScreenshot = true;
            UpdateButtons();
        }

        private void NumUD_width_ValueChanged(object sender, EventArgs e)
        {
            if (mouseDown || currentBitmap == null)
                return;
            UpdatePositions();
            UpdateRectangle();
            ShowImage(currentBitmap);
        }

        private void NumUD_height_ValueChanged(object sender, EventArgs e)
        {
            if (mouseDown || currentBitmap == null)
                return;
            UpdatePositions();
            UpdateRectangle();
            ShowImage(currentBitmap);
        }

        private void NumUD_X_ValueChanged(object sender, EventArgs e)
        {
            if (mouseDown || currentBitmap == null)
                return;
            UpdateRectangle();
            ShowImage(currentBitmap);
        }

        private void NumUD_Y_ValueChanged(object sender, EventArgs e)
        {
            if (mouseDown || currentBitmap == null)
                return;
            UpdateRectangle();
            ShowImage(currentBitmap);
        }

        private void Pb_overview_SizeChanged(object sender, EventArgs e)
        {
            ShowImage(currentBitmap);
        }

        private void Pb_overview_MouseDown(object sender, MouseEventArgs e)
        {
            if (currentBitmap == null || !IsMouseOver() || screenshotWorkerDoWork)
                return;
            mouseDown = true;
            selectionBeginPoint = e.Location;
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

        private void Pb_overview_MouseEnter(object sender, EventArgs e)
        {
            mouseOverPicturebox = true;
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

        private void Pb_overview_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown )
            {
                MouseOverPictureUp(e.Location);
                
            }

        }

        private void CheckBox_shade_CheckedChanged(object sender, EventArgs e)
        {
            ApplyFrame();
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                StopRunningScreenshots();
                pb_cropped.Image = null;
                if (previousScreen != null)
                    previousScreen.Dispose();
                if (previousScreen != null)
                    previousSmlScreen.Dispose();

            }
        }

        void StopRunningScreenshots()
        {
            if(screenshotWorker.IsBusy)
            {
                screenshotWorker.CancelAsync();
            }
            screenshotWorkerDoWork = false;
            ShowCurrentImage();
            UpdateButtons();
            pb_cropped.Visible = false;
            
        }

        private void screenshotWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while(screenshotWorkerDoWork)
            {
                Console.WriteLine("Print");
                if (screenshotWorkerDoWork)
                {
                    //var bmp = Program.CropImage(TakeScreenshot(), cutRectangle);
                    var bmp = Program.TakeScreenshot(
                        new Rectangle(
                            cutRectangle.X - virtualScreenOffset.X, 
                            cutRectangle.Y - virtualScreenOffset.Y, 
                            cutRectangle.Width, 
                            cutRectangle.Height)
                        );
                    var sml = Program.StreachBitmapToSize(bmp, bmp.Width / 100, bmp.Height / 100);
                    screenshotsTaken += 1;
                    if (previousSmlScreen == null || Program.CompareBitmaps(sml, previousSmlScreen) > numUD_treshold.Value)
                    {

                        if (previousSmlScreen != null)
                            previousSmlScreen.Dispose();
                        if (previousScreen != null)
                            previousScreen.Dispose();
                        previousScreen = bmp;
                        previousSmlScreen = sml;
                        screenshotsSaved += 1;
                        var timeStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                        bmp.Save(Path.Combine(outputFolder, $"{timeStr}.png"));
                    }
                    

                    
                }
                screenshotWorker.ReportProgress(0);
                for(int i = (int)numUD_interval.Value * 10; screenshotWorkerDoWork && i >= 0; i--)
                    System.Threading.Thread.Sleep(100);
            }

            screenshotWorker.Dispose();
        }

        private void screenshotWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label_taken.Text = $"Screenshots taken: {screenshotsTaken}";
            label_saved.Text = $"Screenshots saved: {screenshotsSaved}";
            pb_cropped.Image = previousScreen;
        }

        private void screenshotWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }
    }
}
