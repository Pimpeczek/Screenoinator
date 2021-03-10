using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Screenowycinacz
{
    public partial class Form2 : Form
    {
        private BackgroundWorker loadingWorker;
        private List<string> files;
        private Rectangle cutRectangle;
        private string outputFolder;
        bool doWork;
        public Form2()
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
        }

        private void Button_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button_stop_Click(object sender, EventArgs e)
        {
            doWork = false;
            
        }
    }
}
