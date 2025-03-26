namespace Geometry_Invasion
{
    partial class UpgradeScreen
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
            this.nextLevelUpLabel = new System.Windows.Forms.Label();
            this.levelUpBar = new System.Windows.Forms.ProgressBar();
            this.levelUpLabel = new System.Windows.Forms.Label();
            this.progressLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // nextLevelUpLabel
            // 
            this.nextLevelUpLabel.AutoSize = true;
            this.nextLevelUpLabel.Font = new System.Drawing.Font("VT323", 20F);
            this.nextLevelUpLabel.Location = new System.Drawing.Point(53, 518);
            this.nextLevelUpLabel.Name = "nextLevelUpLabel";
            this.nextLevelUpLabel.Size = new System.Drawing.Size(215, 43);
            this.nextLevelUpLabel.TabIndex = 0;
            this.nextLevelUpLabel.Text = "Next Level-Up:";
            // 
            // levelUpBar
            // 
            this.levelUpBar.BackColor = System.Drawing.Color.Black;
            this.levelUpBar.ForeColor = System.Drawing.Color.White;
            this.levelUpBar.Location = new System.Drawing.Point(125, 650);
            this.levelUpBar.Name = "levelUpBar";
            this.levelUpBar.Size = new System.Drawing.Size(550, 30);
            this.levelUpBar.TabIndex = 1;
            // 
            // levelUpLabel
            // 
            this.levelUpLabel.BackColor = System.Drawing.Color.Lime;
            this.levelUpLabel.ForeColor = System.Drawing.Color.Black;
            this.levelUpLabel.Location = new System.Drawing.Point(360, 700);
            this.levelUpLabel.Name = "levelUpLabel";
            this.levelUpLabel.Size = new System.Drawing.Size(80, 30);
            this.levelUpLabel.TabIndex = 2;
            this.levelUpLabel.Text = "Level Up";
            this.levelUpLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // progressLabel
            // 
            this.progressLabel.Font = new System.Drawing.Font("VT323", 18F);
            this.progressLabel.Location = new System.Drawing.Point(125, 601);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(550, 46);
            this.progressLabel.TabIndex = 3;
            this.progressLabel.Text = "0/1000";
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // UpgradeScreen
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.levelUpLabel);
            this.Controls.Add(this.levelUpBar);
            this.Controls.Add(this.nextLevelUpLabel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("VT323", 12F);
            this.ForeColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "UpgradeScreen";
            this.Size = new System.Drawing.Size(800, 800);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nextLevelUpLabel;
        private System.Windows.Forms.ProgressBar levelUpBar;
        private System.Windows.Forms.Label levelUpLabel;
        private System.Windows.Forms.Label progressLabel;
    }
}
