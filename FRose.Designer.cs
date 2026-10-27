namespace ReverseEngineer {
	partial class FRose {
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			button1 = new Button();
			openFileDialog1 = new OpenFileDialog();
			saveFileDialog1 = new SaveFileDialog();
			progressBar1 = new ProgressBar();
			SuspendLayout();
			// 
			// button1
			// 
			button1.Location = new Point(0, 35);
			button1.Name = "button1";
			button1.Size = new Size(137, 37);
			button1.TabIndex = 0;
			button1.Text = "Открыть DLL";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// openFileDialog1
			// 
			openFileDialog1.FileName = "openFileDialog1";
			// 
			// saveFileDialog1
			// 
			saveFileDialog1.FileOk += saveFileDialog1_FileOk;
			// 
			// progressBar1
			// 
			progressBar1.Dock = DockStyle.Top;
			progressBar1.Location = new Point(0, 0);
			progressBar1.Name = "progressBar1";
			progressBar1.Size = new Size(800, 29);
			progressBar1.TabIndex = 1;
			// 
			// FRose
			// 
			AutoScaleDimensions = new SizeF(8F, 20F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 81);
			Controls.Add(progressBar1);
			Controls.Add(button1);
			Name = "FRose";
			Text = "Rational Rose Gen";
			ResumeLayout(false);
		}

		#endregion

		private Button button1;
		private OpenFileDialog openFileDialog1;
		private SaveFileDialog saveFileDialog1;
		private ProgressBar progressBar1;
	}
}
