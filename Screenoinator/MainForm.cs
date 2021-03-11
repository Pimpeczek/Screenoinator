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
        #region Class variables
        private Rectangle selectionRectangle;
        private Point selectionBeginPoint;
        private Point selectionEndPoint;
        private Point virtualScreenOffset;
        private OpenFileDialog ofd;
        private BackgroundWorker screenshotWorker;
        private Bitmap previousSmlScreen;
        private Bitmap previousScreen;
        private Bitmap currentBitmap;
        private List<string> filesToCrop;
        private string outputFolder;
        private float overviewScale;
        private int screenshotsTaken;
        private int screenshotsSaved;
        private bool screenshotWorkerDoWork;
        private bool mouseDown;
        private bool mouseOverPicturebox;
        private bool hasBasicScreenshot;
        #endregion

        #region Initialization
        public MainForm()
        {
            InitializeComponent();
            virtualScreenOffset = new Point();
            
            
            selectionRectangle = new Rectangle(0, 0, 100, 100);
            filesToCrop = new List<string>();
            outputFolder = null;
            overviewScale = 1;
            screenshotsTaken = 0;
            screenshotsSaved = 0;
            screenshotWorkerDoWork = false;
            mouseDown = false;
            mouseOverPicturebox = false;
            hasBasicScreenshot = false;
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

        #endregion

        #region Methods

        private void UpdateRectangle()
        {
            selectionRectangle.X = (int)numUD_X.Value;
            selectionRectangle.Y = (int)numUD_Y.Value;
            selectionRectangle.Width = (int)numUD_width.Value;
            selectionRectangle.Height = (int)numUD_height.Value;
        }

        private void ShowCurrentImage()
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
            button_process.Enabled = outputFolder != null && filesToCrop.Count > 0;
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

        private void ShowImage(Bitmap b)
        {
            if (b == null)
                return;
            try
            {
                this.pb_overview.BackgroundImage = Program.ResizeToFit(b, this.pb_overview.Width, this.pb_overview.Height);
                ApplyFrame();
            }
            catch
            {

            }
        }

        private void ApplyFrame()
        {
            Bitmap b = new Bitmap(pb_overview.BackgroundImage.Width, pb_overview.BackgroundImage.Height);
            overviewScale = b.Width / (float)currentBitmap.Width;
            int penWidth = Math.Max(1, Math.Max(pb_overview.BackgroundImage.Width, pb_overview.BackgroundImage.Height) / 400);
            Pen pen = new Pen(Color.Red, penWidth);
            SolidBrush outsideBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
            SolidBrush eraser = new SolidBrush(Color.FromArgb(0, 255, 255, 255));
            SolidBrush insideBrush = new SolidBrush(Color.FromArgb(24, 0, 0, 0));
            Rectangle fRec = new Rectangle(
                    (int)(selectionRectangle.X * overviewScale),
                    (int)(selectionRectangle.Y * overviewScale),
                    (int)(selectionRectangle.Width * overviewScale) + penWidth,
                    (int)(selectionRectangle.Height * overviewScale) + penWidth);
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
            label_spos.Text = $"Region position: {selectionRectangle.X}x{selectionRectangle.Y}";
            label_ssize.Text = $"Region size: {selectionRectangle.Width}x{selectionRectangle.Height}";
        }

        private void ClearFiles()
        {
            filesToCrop.Clear();
            this.label_filecount.Text = $"Files: {filesToCrop.Count}";
            this.label_screensize.Text = $"Screen size: (?)";
            ClearImages();
        }

        private void StopRunningScreenshots()
        {
            if (screenshotWorker.IsBusy)
            {
                screenshotWorker.CancelAsync();
            }
            screenshotWorkerDoWork = false;
            ShowCurrentImage();
            UpdateButtons();
            pb_cropped.Visible = false;

        }

        private bool IsMouseOver()
        {
            Point p = pb_overview.PointToClient(MousePosition);
            return mouseOverPicturebox && p.X >= 0 && p.Y >= 0 && p.X < currentBitmap.Width * overviewScale && p.Y < currentBitmap.Height * overviewScale;
        }

        private void MouseOverPictureUp(Point e)
        {
            selectionEndPoint = e;
            if (e.X >= (int)selectionBeginPoint.X)
            {
                numUD_width.Value = Math.Min((int)((e.X - selectionBeginPoint.X) / overviewScale), numUD_width.Maximum - (int)(selectionBeginPoint.X / overviewScale));
            }
            else
            {
                numUD_X.Value = Math.Min(Math.Max(0, (int)(selectionEndPoint.X / overviewScale)), numUD_X.Maximum);
                numUD_width.Value = Math.Min((int)((selectionBeginPoint.X - e.X) / overviewScale), numUD_width.Maximum);
            }

            if (e.Y >= (int)selectionBeginPoint.Y)
            {
                numUD_height.Value = Math.Min((int)((e.Y - selectionBeginPoint.Y) / overviewScale), numUD_height.Maximum - (int)(selectionBeginPoint.Y / overviewScale));
            }
            else
            {
                numUD_Y.Value = Math.Min(Math.Max(0, (int)(selectionEndPoint.Y / overviewScale)), numUD_Y.Maximum);
                numUD_height.Value = Math.Min((int)((selectionBeginPoint.Y - e.Y) / overviewScale), numUD_height.Maximum);
            }
            UpdateRectangle();
            ApplyFrame();
        }

        #endregion

        #region Controls
        #region Buttons
        private void Button_select_Click(object sender, EventArgs e)
        {
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                filesToCrop = new List<string>(ofd.FileNames);
                UpdateButtons();
                this.label_filecount.Text = $"Files: {filesToCrop.Count}";
            }
            else
            {
                return;
            }
            Bitmap b = new Bitmap(filesToCrop[0]);
            Size baseSize = b.Size;
            label_screensize.Text = $"Screen size: {baseSize.Width} x {baseSize.Height}";
            foreach (var f in filesToCrop)
            {
                b = new Bitmap(f);
                if (baseSize != b.Size)
                {
                    MessageBox.Show("Wszystkie obrazy muszą mieć takie same wymiary.", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ClearFiles();
                    break;
                }
            }
            currentBitmap = b;

            ShowCurrentImage();
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
            var m = new CroppingProgressForm(filesToCrop, selectionRectangle, outputFolder);
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
        #endregion

        #region NumericUpDowns
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
        #endregion

        #region UI Overview
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
            numUD_X.Value = Math.Min((int)(e.X / overviewScale), numUD_X.Maximum);
            numUD_Y.Value = Math.Min((int)(e.Y / overviewScale), numUD_Y.Maximum);
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
        #endregion

        #region UI Misc
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
        #endregion
        #endregion

        #region Worker
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
                            selectionRectangle.X - virtualScreenOffset.X, 
                            selectionRectangle.Y - virtualScreenOffset.Y, 
                            selectionRectangle.Width, 
                            selectionRectangle.Height)
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
        #endregion
    }
}
