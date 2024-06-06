namespace LyumixLauncher
{
    public partial class LyumixLauncher : Form
    {
        private int location = 0;

        public LyumixLauncher()
        {
            InitializeComponent();
            this.AutoScroll = true;
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            // Set the TextRenderingHint to AntiAliasGridFit for high-quality text
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Define the font to use
            Font font = new Font("Segoe UI", 16, FontStyle.Bold);

            // Define the text to draw
            string text = "This is a clear text";

            // Define the brush to draw the text
            Brush brush = Brushes.Black;

            // Draw the text
            e.Graphics.DrawString(text, font, brush, new PointF(10, 10));
        }

        private void Button_Click(object sender, MouseEventArgs e)
        {
            Button clickedButton = sender as Button;
            string moduleName = clickedButton.Name;
            bool resp = ModuleStarter.ModuleChecker(moduleName);
            if (e.Button == MouseButtons.Left)
            {
                if (!ModuleInstallChecker.IsModuleInstalled(moduleName))
                {
                    ModuleInstaller installer = new ModuleInstaller();
                    installer.InstallModule(moduleName);
                    return;
                }
                else
                {
                    if (resp)
                    {
                        ModuleStarter.StartModule(moduleName);
                        clickedButton.BackColor = Color.MediumSeaGreen;
                        return;
                    }
                    else
                    {
                        ModuleStarter.StopModule(moduleName);
                        clickedButton.BackColor = darkButtonBackground;
                        return;
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (ModuleInstallChecker.IsModuleInstalled(moduleName))
                {
                    if (resp)
                    {
                        ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
                        ToolStripMenuItem enableStartUp = new ToolStripMenuItem();
                        bool moduleState = UtilMan.GetInfoFromSysVars(moduleName);
                        enableStartUp = new ToolStripMenuItem("Start with a system");
                        enableStartUp.Checked = moduleState; // Set initial check state
                        enableStartUp.CheckOnClick = true; // Allow the item to be checked/unchecked on click
                        enableStartUp.Click += (s, ev) => StartWithSys_Click(s, ev, clickedButton);
                        contextMenuStrip.Items.Add(enableStartUp);

                        contextMenuStrip.Show(clickedButton, e.Location);
                        return;
                    }
                }
            }
        }

        private void Start_MouseHover(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            clickedButton.BackColor = hoverColor;
        }

        private void Start_MouseLeave(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            clickedButton.BackColor = darkButtonBackground;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.launcher.Hide();
        }

        private void StartWithSys_Click(object sender, EventArgs e, Button clickedButton)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                UtilMan.UpdateStartUp(clickedButton.Name, item.Checked);
            }
        }
    }
}
