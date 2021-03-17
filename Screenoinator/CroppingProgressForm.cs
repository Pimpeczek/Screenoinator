using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Screenoinator
{
    public partial class CroppingProgressForm : Form
    {
        #region Class variables
        private readonly BackgroundWorker croppingWorker;
        private List<string> files;
        private Rectangle selectionRectangle;
        private string outputFolder;
        private bool doWork;
        #endregion

        #region Initialization
        public CroppingProgressForm(List<string> files, Rectangle cutRectangle, string outputFolder)
        {
            InitializeComponent();
            croppingWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            croppingWorker.DoWork +=
                new DoWorkEventHandler(croppingWorker_DoWork);
            croppingWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            croppingWorker_RunWorkerCompleted);
            croppingWorker.ProgressChanged +=
                new ProgressChangedEventHandler(
            croppingWorker_ProgressChanged);
            doWork = true;
            this.files = files;
            this.selectionRectangle = cutRectangle;
            this.outputFolder = outputFolder;
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;
            progressBar1.Maximum = files.Count;
            croppingWorker.RunWorkerAsync();
        }
        #endregion

        #region Worker
        private void croppingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var timeStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            for (int i = 0; i < files.Count; i++)
            {
                if (doWork)
                {
                    using (var cropped = Program.CropImage(new Bitmap(files[i]), selectionRectangle))
                    {
                        if(Program.ApplyWatermarkFlag)
                            Program.ApplyWatermark(cropped);
                        
                        cropped.Save(Path.Combine(outputFolder, $"{timeStr}_{i}.png"));
                    }
                    croppingWorker.ReportProgress(0);
                }
                else
                {
                    croppingWorker.Dispose();
                }
            }
        }

        private void croppingWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (doWork)
            {
                progressBar1.Value += 1;
                label1.Text = $"Files: [{progressBar1.Value}/{files.Count}]";
            }
            else
            {
                label1.Text = $"Files: [{progressBar1.Value}/{files.Count}] (Process aborted!)";
                progressBar1.Value = progressBar1.Maximum;
            }
        }

        // This event handler deals with the results of the background operation.
        private void croppingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button_exit.Enabled = true;
            button_stop.Enabled = false;
        }
        #endregion

        #region Controls
        private void Button_exit_Click(object sender, EventArgs e)
        {
            Program.MainWindow.Enabled = true;
            this.Close();
        }

        private void Button_stop_Click(object sender, EventArgs e)
        {
            doWork = false;
            
        }
        #endregion
    }
}
