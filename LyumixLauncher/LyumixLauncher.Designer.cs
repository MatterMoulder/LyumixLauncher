using System.Runtime.InteropServices;

namespace LyumixLauncher
{
    partial class LyumixLauncher
    {
        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool ShouldSystemUseDarkMode();

        private Color darkBackground = Color.FromArgb(0, 0, 0);
        private Color darkForeground = Color.FromArgb(200, 200, 200);
        private Color darkButtonBackground = Color.FromArgb(30, 30, 30);
        private Color darkButtonForeground = Color.FromArgb(200, 200, 200);
        private Color darkButtonBorder = Color.FromArgb(60, 60, 60);
        private Color hoverColor = Color.FromArgb(50, 50, 50);
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

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LyumixLauncher));
            SuspendLayout();
            // 
            // startAppleMusicRPC
            // 
            /*resources.ApplyResources(startAppleMusicRPC, "startAppleMusicRPC");
            startAppleMusicRPC.BackColor = Color.Black;
            startAppleMusicRPC.Cursor = Cursors.Hand;
            startAppleMusicRPC.FlatAppearance.BorderColor = Color.Black;
            startAppleMusicRPC.FlatAppearance.BorderSize = 0;
            startAppleMusicRPC.ForeColor = SystemColors.ControlLightLight;
            startAppleMusicRPC.Name = "startAppleMusicRPC";
            startAppleMusicRPC.UseVisualStyleBackColor = false;
            startAppleMusicRPC.MouseLeave += StartAppleMusicRPC_MouseLeave;
            startAppleMusicRPC.MouseHover += StartAppleMusicRPC_MouseHover;
            startAppleMusicRPC.MouseDown += Button_Click;*/

            // 
            // LyumixLauncher
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = darkBackground;
            ForeColor = darkForeground;
            FormClosing += MainForm_FormClosing;
            GenerateButtons();
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LyumixLauncher";
            TransparencyKey = Color.Transparent;
            ResumeLayout(false);
            PerformLayout();
        }

        private void GenerateButtons()
        {
            int btnCount = 0;
            int xPos = 10;
            int yPos = 10;
            foreach (var item in UtilMan.services)
            {
                // Create a new button
                Button serviceButton = new Button();
                serviceButton.Text = item.name;
                serviceButton.Size = new Size(250, 60);
                serviceButton.Location = new Point(xPos, yPos);

                // Add Click event handler (optional)
                serviceButton.MouseDown += Button_Click;
                /*button.MouseHover += Start_MouseHover;
                button.MouseLeave += Start_MouseLeave;*/
                serviceButton.UseVisualStyleBackColor = false;
                serviceButton.FlatAppearance.BorderColor = darkButtonBorder;
                serviceButton.FlatAppearance.BorderSize = 1;
                serviceButton.ForeColor = darkButtonForeground;
                serviceButton.Name = item.srvc;
                serviceButton.FlatStyle = FlatStyle.Flat;
                serviceButton.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                serviceButton.Visible = true;

                if (!ModuleStarter.ModuleChecker(item.srvc))
                {
                    serviceButton.BackColor = Color.MediumSeaGreen;
                }
                else
                {
                    serviceButton.BackColor = darkButtonBackground;
                }

                this.Controls.Add(serviceButton);

                // Update the position for the next button
                yPos += 80; // Move down for the next button
                btnCount++;
            }
            if (btnCount > 5)
            {
                this.MaximumSize = new Size(300, 470);
                this.MinimumSize = new Size(300, 470);
                this.Size = new Size(300, 470);
            } else
            {
                this.MaximumSize = new Size(289, 470);
                this.MinimumSize = new Size(289, 470);
                this.Size = new Size(289, 470);
            }
        }

        #endregion

    }
}
