namespace Screenowycinacz
{
    partial class Form1
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button_process = new System.Windows.Forms.Button();
            this.label_screensize = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label_filecount = new System.Windows.Forms.Label();
            this.button_clear = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.button_select = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button_output = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numUD_Y = new System.Windows.Forms.NumericUpDown();
            this.numUD_height = new System.Windows.Forms.NumericUpDown();
            this.numUD_width = new System.Windows.Forms.NumericUpDown();
            this.numUD_X = new System.Windows.Forms.NumericUpDown();
            this.checkBox_shade = new System.Windows.Forms.CheckBox();
            this.pb_overview = new System.Windows.Forms.PictureBox();
            this.numUD_treshold = new System.Windows.Forms.NumericUpDown();
            this.numUD_interval = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_outputFolder = new System.Windows.Forms.TextBox();
            this.button_enablescreenshots = new System.Windows.Forms.Button();
            this.button_stopscreenshots = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button_baseScreen = new System.Windows.Forms.Button();
            this.textBox_outputAuto = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button_outputAuto = new System.Windows.Forms.Button();
            this.label_taken = new System.Windows.Forms.Label();
            this.label_saved = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_Y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_height)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_width)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_X)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_overview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_treshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_interval)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_process
            // 
            this.button_process.Location = new System.Drawing.Point(6, 240);
            this.button_process.Name = "button_process";
            this.button_process.Size = new System.Drawing.Size(155, 23);
            this.button_process.TabIndex = 13;
            this.button_process.Text = "Process";
            this.button_process.UseVisualStyleBackColor = true;
            this.button_process.Click += new System.EventHandler(this.Button_process_Click);
            // 
            // label_screensize
            // 
            this.label_screensize.AutoSize = true;
            this.label_screensize.Location = new System.Drawing.Point(3, 51);
            this.label_screensize.Name = "label_screensize";
            this.label_screensize.Size = new System.Drawing.Size(80, 13);
            this.label_screensize.TabIndex = 12;
            this.label_screensize.Text = "Screen size: (?)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(83, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Y position";
            // 
            // label_filecount
            // 
            this.label_filecount.AutoSize = true;
            this.label_filecount.Location = new System.Drawing.Point(3, 32);
            this.label_filecount.Name = "label_filecount";
            this.label_filecount.Size = new System.Drawing.Size(40, 13);
            this.label_filecount.TabIndex = 11;
            this.label_filecount.Text = "Files: 0";
            // 
            // button_clear
            // 
            this.button_clear.Location = new System.Drawing.Point(87, 6);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(75, 23);
            this.button_clear.TabIndex = 2;
            this.button_clear.Text = "Clear files";
            this.button_clear.UseVisualStyleBackColor = true;
            this.button_clear.Click += new System.EventHandler(this.Button_clear_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "X position";
            // 
            // button_select
            // 
            this.button_select.Location = new System.Drawing.Point(4, 6);
            this.button_select.Name = "button_select";
            this.button_select.Size = new System.Drawing.Size(75, 23);
            this.button_select.TabIndex = 1;
            this.button_select.Text = "Select files";
            this.button_select.UseVisualStyleBackColor = true;
            this.button_select.Click += new System.EventHandler(this.Button_select_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Width";
            // 
            // button_output
            // 
            this.button_output.Location = new System.Drawing.Point(5, 175);
            this.button_output.Name = "button_output";
            this.button_output.Size = new System.Drawing.Size(157, 23);
            this.button_output.TabIndex = 0;
            this.button_output.Text = "Select output";
            this.button_output.UseVisualStyleBackColor = true;
            this.button_output.Click += new System.EventHandler(this.Button_output_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(84, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Height";
            // 
            // numUD_Y
            // 
            this.numUD_Y.Location = new System.Drawing.Point(86, 126);
            this.numUD_Y.Name = "numUD_Y";
            this.numUD_Y.Size = new System.Drawing.Size(75, 20);
            this.numUD_Y.TabIndex = 8;
            this.numUD_Y.ValueChanged += new System.EventHandler(this.NumUD_Y_ValueChanged);
            // 
            // numUD_height
            // 
            this.numUD_height.Location = new System.Drawing.Point(87, 87);
            this.numUD_height.Name = "numUD_height";
            this.numUD_height.Size = new System.Drawing.Size(75, 20);
            this.numUD_height.TabIndex = 4;
            this.numUD_height.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numUD_height.ValueChanged += new System.EventHandler(this.NumUD_height_ValueChanged);
            // 
            // numUD_width
            // 
            this.numUD_width.Location = new System.Drawing.Point(6, 87);
            this.numUD_width.Name = "numUD_width";
            this.numUD_width.Size = new System.Drawing.Size(75, 20);
            this.numUD_width.TabIndex = 3;
            this.numUD_width.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numUD_width.ValueChanged += new System.EventHandler(this.NumUD_width_ValueChanged);
            // 
            // numUD_X
            // 
            this.numUD_X.Location = new System.Drawing.Point(5, 126);
            this.numUD_X.Name = "numUD_X";
            this.numUD_X.Size = new System.Drawing.Size(75, 20);
            this.numUD_X.TabIndex = 7;
            this.numUD_X.ValueChanged += new System.EventHandler(this.NumUD_X_ValueChanged);
            // 
            // checkBox_shade
            // 
            this.checkBox_shade.AutoSize = true;
            this.checkBox_shade.Location = new System.Drawing.Point(5, 152);
            this.checkBox_shade.Name = "checkBox_shade";
            this.checkBox_shade.Size = new System.Drawing.Size(83, 17);
            this.checkBox_shade.TabIndex = 3;
            this.checkBox_shade.Text = "Draw shade";
            this.checkBox_shade.UseVisualStyleBackColor = true;
            this.checkBox_shade.CheckedChanged += new System.EventHandler(this.CheckBox_shade_CheckedChanged);
            // 
            // pb_overview
            // 
            this.pb_overview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_overview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pb_overview.Location = new System.Drawing.Point(181, 25);
            this.pb_overview.Name = "pb_overview";
            this.pb_overview.Size = new System.Drawing.Size(733, 544);
            this.pb_overview.TabIndex = 0;
            this.pb_overview.TabStop = false;
            this.pb_overview.SizeChanged += new System.EventHandler(this.Pb_overview_SizeChanged);
            this.pb_overview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Pb_overview_MouseDown);
            this.pb_overview.MouseEnter += new System.EventHandler(this.Pb_overview_MouseEnter);
            this.pb_overview.MouseLeave += new System.EventHandler(this.Pb_overview_MouseLeave);
            this.pb_overview.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Pb_overview_MouseMove);
            this.pb_overview.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Pb_overview_MouseUp);
            // 
            // numUD_treshold
            // 
            this.numUD_treshold.Location = new System.Drawing.Point(9, 48);
            this.numUD_treshold.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numUD_treshold.Name = "numUD_treshold";
            this.numUD_treshold.Size = new System.Drawing.Size(70, 20);
            this.numUD_treshold.TabIndex = 15;
            this.numUD_treshold.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // numUD_interval
            // 
            this.numUD_interval.Location = new System.Drawing.Point(9, 87);
            this.numUD_interval.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numUD_interval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numUD_interval.Name = "numUD_interval";
            this.numUD_interval.Size = new System.Drawing.Size(70, 20);
            this.numUD_interval.TabIndex = 16;
            this.numUD_interval.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Treshold:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Interval:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 201);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Outputing to:";
            // 
            // textBox_outputFolder
            // 
            this.textBox_outputFolder.Enabled = false;
            this.textBox_outputFolder.Location = new System.Drawing.Point(6, 216);
            this.textBox_outputFolder.Name = "textBox_outputFolder";
            this.textBox_outputFolder.Size = new System.Drawing.Size(155, 20);
            this.textBox_outputFolder.TabIndex = 16;
            // 
            // button_enablescreenshots
            // 
            this.button_enablescreenshots.Enabled = false;
            this.button_enablescreenshots.Location = new System.Drawing.Point(6, 180);
            this.button_enablescreenshots.Name = "button_enablescreenshots";
            this.button_enablescreenshots.Size = new System.Drawing.Size(156, 23);
            this.button_enablescreenshots.TabIndex = 17;
            this.button_enablescreenshots.Text = "Begin";
            this.button_enablescreenshots.UseVisualStyleBackColor = true;
            this.button_enablescreenshots.Click += new System.EventHandler(this.Button_enablescreenshots_Click);
            // 
            // button_stopscreenshots
            // 
            this.button_stopscreenshots.Enabled = false;
            this.button_stopscreenshots.Location = new System.Drawing.Point(6, 209);
            this.button_stopscreenshots.Name = "button_stopscreenshots";
            this.button_stopscreenshots.Size = new System.Drawing.Size(156, 23);
            this.button_stopscreenshots.TabIndex = 19;
            this.button_stopscreenshots.Text = "Stop";
            this.button_stopscreenshots.UseVisualStyleBackColor = true;
            this.button_stopscreenshots.Click += new System.EventHandler(this.Button_stopscreenshots_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Location = new System.Drawing.Point(3, 3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(176, 297);
            this.tabControl.TabIndex = 2;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBox_outputFolder);
            this.tabPage1.Controls.Add(this.button_process);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.button_output);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.checkBox_shade);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.button_select);
            this.tabPage1.Controls.Add(this.button_clear);
            this.tabPage1.Controls.Add(this.numUD_Y);
            this.tabPage1.Controls.Add(this.numUD_X);
            this.tabPage1.Controls.Add(this.label_filecount);
            this.tabPage1.Controls.Add(this.label_screensize);
            this.tabPage1.Controls.Add(this.numUD_height);
            this.tabPage1.Controls.Add(this.numUD_width);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(168, 271);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Manual";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label_saved);
            this.tabPage2.Controls.Add(this.label_taken);
            this.tabPage2.Controls.Add(this.textBox_outputAuto);
            this.tabPage2.Controls.Add(this.button_baseScreen);
            this.tabPage2.Controls.Add(this.button_stopscreenshots);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.button_outputAuto);
            this.tabPage2.Controls.Add(this.button_enablescreenshots);
            this.tabPage2.Controls.Add(this.numUD_treshold);
            this.tabPage2.Controls.Add(this.numUD_interval);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(168, 271);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Auto";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button_baseScreen
            // 
            this.button_baseScreen.Location = new System.Drawing.Point(6, 6);
            this.button_baseScreen.Name = "button_baseScreen";
            this.button_baseScreen.Size = new System.Drawing.Size(156, 23);
            this.button_baseScreen.TabIndex = 20;
            this.button_baseScreen.Text = "Take base screenshot";
            this.button_baseScreen.UseVisualStyleBackColor = true;
            this.button_baseScreen.Click += new System.EventHandler(this.Button_baseScreen_Click);
            // 
            // textBox_outputAuto
            // 
            this.textBox_outputAuto.Enabled = false;
            this.textBox_outputAuto.Location = new System.Drawing.Point(7, 154);
            this.textBox_outputAuto.Name = "textBox_outputAuto";
            this.textBox_outputAuto.Size = new System.Drawing.Size(155, 20);
            this.textBox_outputAuto.TabIndex = 23;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 139);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Outputing to:";
            // 
            // button_outputAuto
            // 
            this.button_outputAuto.Location = new System.Drawing.Point(6, 113);
            this.button_outputAuto.Name = "button_outputAuto";
            this.button_outputAuto.Size = new System.Drawing.Size(157, 23);
            this.button_outputAuto.TabIndex = 21;
            this.button_outputAuto.Text = "Select output";
            this.button_outputAuto.UseVisualStyleBackColor = true;
            this.button_outputAuto.Click += new System.EventHandler(this.Button_outputAuto_Click);
            // 
            // label_taken
            // 
            this.label_taken.AutoSize = true;
            this.label_taken.Location = new System.Drawing.Point(6, 235);
            this.label_taken.Name = "label_taken";
            this.label_taken.Size = new System.Drawing.Size(99, 13);
            this.label_taken.TabIndex = 24;
            this.label_taken.Text = "Screenshots taken:";
            // 
            // label_saved
            // 
            this.label_saved.AutoSize = true;
            this.label_saved.Location = new System.Drawing.Point(6, 253);
            this.label_saved.Name = "label_saved";
            this.label_saved.Size = new System.Drawing.Size(101, 13);
            this.label_saved.TabIndex = 25;
            this.label_saved.Text = "Screenshots saved:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 303);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(160, 65);
            this.label9.TabIndex = 26;
            this.label9.Text = "Select some part of the screen.\r\nYou can do it either by\r\ndragging selection on t" +
    "he image \r\nor by providing position and size \r\nmanually.";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 581);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.pb_overview);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Screenoinator";
            ((System.ComponentModel.ISupportInitialize)(this.numUD_Y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_height)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_width)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_X)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_overview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_treshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUD_interval)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button_process;
        private System.Windows.Forms.Label label_screensize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_filecount;
        private System.Windows.Forms.Button button_clear;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button_select;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_output;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numUD_Y;
        private System.Windows.Forms.NumericUpDown numUD_height;
        private System.Windows.Forms.NumericUpDown numUD_width;
        private System.Windows.Forms.NumericUpDown numUD_X;
        private System.Windows.Forms.CheckBox checkBox_shade;
        private System.Windows.Forms.PictureBox pb_overview;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numUD_treshold;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numUD_interval;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox textBox_outputFolder;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button_stopscreenshots;
        private System.Windows.Forms.Button button_enablescreenshots;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button_baseScreen;
        private System.Windows.Forms.TextBox textBox_outputAuto;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button_outputAuto;
        private System.Windows.Forms.Label label_saved;
        private System.Windows.Forms.Label label_taken;
        private System.Windows.Forms.Label label9;
    }
}

