namespace Geometry_Invasion
{
    partial class TitleScreen
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.titleLabel = new System.Windows.Forms.Label();
            this.playButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.upgradesButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.Font = new System.Drawing.Font("VT323", 50F);
            this.titleLabel.Location = new System.Drawing.Point(0, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(800, 333);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Geometry Invasion";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // playButton
            // 
            this.playButton.BackColor = System.Drawing.Color.Lime;
            this.playButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.playButton.Font = new System.Drawing.Font("VT323", 30F);
            this.playButton.ForeColor = System.Drawing.Color.Black;
            this.playButton.Location = new System.Drawing.Point(275, 305);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(250, 85);
            this.playButton.TabIndex = 1;
            this.playButton.Text = "PLAY";
            this.playButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.playButton.UseVisualStyleBackColor = false;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.BackColor = System.Drawing.Color.Red;
            this.exitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitButton.Font = new System.Drawing.Font("VT323", 30F);
            this.exitButton.ForeColor = System.Drawing.Color.Black;
            this.exitButton.Location = new System.Drawing.Point(275, 505);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(250, 85);
            this.exitButton.TabIndex = 2;
            this.exitButton.Text = "EXIT";
            this.exitButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.exitButton.UseVisualStyleBackColor = false;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // upgradesButton
            // 
            this.upgradesButton.BackColor = System.Drawing.Color.Yellow;
            this.upgradesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.upgradesButton.Font = new System.Drawing.Font("VT323", 30F);
            this.upgradesButton.ForeColor = System.Drawing.Color.Black;
            this.upgradesButton.Location = new System.Drawing.Point(275, 405);
            this.upgradesButton.Name = "upgradesButton";
            this.upgradesButton.Size = new System.Drawing.Size(250, 85);
            this.upgradesButton.TabIndex = 3;
            this.upgradesButton.Text = "UPGRADES";
            this.upgradesButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.upgradesButton.UseVisualStyleBackColor = false;
            this.upgradesButton.Click += new System.EventHandler(this.upgradesButton_Click);
            // 
            // TitleScreen
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.upgradesButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.titleLabel);
            this.Font = new System.Drawing.Font("VT323", 18F);
            this.ForeColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            this.Name = "TitleScreen";
            this.Size = new System.Drawing.Size(800, 800);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button upgradesButton;
    }
}
