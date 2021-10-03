
namespace OpenCV
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_VisORB = new System.Windows.Forms.Button();
            this.btn_DetectArucoInLowLight = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_VisORB
            // 
            this.btn_VisORB.Location = new System.Drawing.Point(12, 12);
            this.btn_VisORB.Name = "btn_VisORB";
            this.btn_VisORB.Size = new System.Drawing.Size(96, 23);
            this.btn_VisORB.TabIndex = 0;
            this.btn_VisORB.Text = "Visualize ORB";
            this.btn_VisORB.UseVisualStyleBackColor = true;
            this.btn_VisORB.Click += new System.EventHandler(this.btn_VisORB_Click);
            // 
            // btn_DetectArucoInLowLight
            // 
            this.btn_DetectArucoInLowLight.Location = new System.Drawing.Point(114, 12);
            this.btn_DetectArucoInLowLight.Name = "btn_DetectArucoInLowLight";
            this.btn_DetectArucoInLowLight.Size = new System.Drawing.Size(90, 23);
            this.btn_DetectArucoInLowLight.TabIndex = 1;
            this.btn_DetectArucoInLowLight.Text = "Detect Aruco";
            this.btn_DetectArucoInLowLight.UseVisualStyleBackColor = true;
            this.btn_DetectArucoInLowLight.Click += new System.EventHandler(this.btn_DetectArucoInLowLight_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(210, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Gabor Filter";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_DetectArucoInLowLight);
            this.Controls.Add(this.btn_VisORB);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_VisORB;
        private System.Windows.Forms.Button btn_DetectArucoInLowLight;
        private System.Windows.Forms.Button button1;
    }
}

