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
        private readonly BackgroundWorker loadingWorker;
        private List<string> files;
        private Rectangle cutRectangle;
        private string outputFolder;
        bool doWork;
        public CroppingProgressForm()
        {
            InitializeComponent();
            loadingWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            loadingWorker.DoWork +=
                new DoWorkEventHandler(loadingWorker_DoWork);
            loadingWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            loadingWorker_RunWorkerCompleted);
            loadingWorker.ProgressChanged +=
                new ProgressChangedEventHandler(
            loadingWorker_ProgressChanged);
        }

        public void Go(List<string> files, Rectangle cutRectangle, string outputFolder)
        {
            doWork = true;
            this.files = files;
            this.cutRectangle = cutRectangle;
            this.outputFolder = outputFolder;
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;
            progressBar1.Maximum = files.Count;
            loadingWorker.RunWorkerAsync();

        }
        

        private void loadingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < files.Count; i++)
            {
                if (doWork)
                {
                    using (var cropped = Program.CropImage(new Bitmap(files[i]), cutRectangle))
                        cropped.Save(Path.Combine(outputFolder, $"{i}.png"));
                    loadingWorker.ReportProgress(0);
                }
                else
                {
                    loadingWorker.Dispose();
                }
            }
        }

        private void loadingWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
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
        private void loadingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button_exit.Enabled = true;
            button_stop.Enabled = false;
        }

        private void Button_exit_Click(object sender, EventArgs e)
        {
            Program.MainWindow.Enabled = true;
            this.Close();
        }

        private void Button_stop_Click(object sender, EventArgs e)
        {
            doWork = false;
            
        }
    }
}
