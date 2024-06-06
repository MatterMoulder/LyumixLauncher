using System.Runtime.InteropServices;

namespace LyumixLauncher
{
    internal static class Program
    {
        public static LyumixLauncher launcher;

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();
            UtilMan.Init();
            if (!Directory.Exists(UtilMan.modulePath))
            {
                Directory.CreateDirectory(UtilMan.modulePath);
            }

            bool alpha = new ModuleInstaller().ReinstallAll().Result;
            List<string> activeModules = UtilMan.GetAllStartUp();
            if (activeModules != null)
            {
                foreach (string module in activeModules)
                {
                    if (ModuleInstallChecker.IsModuleInstalled(module))
                    {
                        ModuleStarter.StartModule(module);
                    } else
                    {
                        ModuleInstaller installer = new ModuleInstaller();
                        installer.InstallModule(module);
                    }
                        
                }
            }

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            UtilMan.shareItem = new ToolStripMenuItem("Open App");
            UtilMan.exitItem = new ToolStripMenuItem("Exit");

            UtilMan.shareItem.Click += OpenItem_Click;
            UtilMan.exitItem.Click += ExitItem_Click;

            UtilMan.CreateStripMenu(contextMenu);
            UtilMan.notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            bool startedByStartup = args.Contains("/startup");
            launcher = new LyumixLauncher();
            if (!startedByStartup)
            {
#if !DEBUG
                AddStartupShortcut();
#endif
                Application.Run(launcher);
            }
            Application.Run();
        }

        private static void OpenItem_Click(object sender, EventArgs e)
        {
            try
            {
                launcher!.Show();
                launcher!.Focus();
            }
            catch (Exception ex) when (ex is NullReferenceException || ex is InvalidOperationException)
            {
                launcher = new LyumixLauncher();
                launcher.Show();
                launcher.Focus();
            }
        }

        private static void ExitItem_Click(object sender, EventArgs e)
        {
            ModuleStarter.StopAllModules();
            Application.Exit();
        }

        private static void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                launcher!.Show();
                launcher!.Focus();
            }
            catch (Exception ex) when (ex is NullReferenceException || ex is InvalidOperationException)
            {
                launcher = new LyumixLauncher();
                launcher.Show();
                launcher.Focus();
            }
        }

        private static void AddStartupShortcut()
        {
            if (!Path.Exists(UtilMan.AppShortcutPath))
            {
                var t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")); // Windows Script Host Shell Object
                dynamic shell = Activator.CreateInstance(t!)!;
                try
                {
                    var lnk = shell.CreateShortcut(UtilMan.AppShortcutPath);
                    try
                    {
                        lnk.TargetPath = UtilMan.ExePath;
                        lnk.IconLocation = $"{UtilMan.ExePath}, 0";
                        lnk.Arguments = "/startup";
                        lnk.Save();
                    }
                    finally
                    {
                        Marshal.FinalReleaseComObject(lnk);
                    }
                }
                finally
                {
                    Marshal.FinalReleaseComObject(shell);
                }
            }
        }
    }
}