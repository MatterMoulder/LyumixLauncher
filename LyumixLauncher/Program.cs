using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Policy;

namespace LyumixLauncher
{
    internal static class Program
    {
        public static LyumixLauncher launcher;
        static string jsonLib = "https://github.com/MatterMoulder/LLupdate/raw/main/Newtonsoft.Json.dll";
        static string versionFileUrl = "https://raw.githubusercontent.com/MatterMoulder/LLupdate/main/version.txt";
        static string repoUrl = "https://api.github.com/repos/MatterMoulder/LLupdate/contents";
        static string currentVersion = "0.6.0";

        [STAThread]
        static async Task Main(string[] args)
        {
            Thread.Sleep(500);
            Process[] old_process = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location));
            Process[] old_updater_process = System.Diagnostics.Process.GetProcessesByName("LyumixLauncherUpdater.exe");
            if ((old_process.Count() > 1) || (old_updater_process.Count() > 1))
            {
                return;
            }

            ApplicationConfiguration.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            UtilMan.InitNotify();

            bool startedInOffline = args.Contains("/offline");
#if !DEBUG
            bool isUpdated = args.Contains("/updated");
            if (isUpdated)
            {
                UtilMan.SendNotify("Update complete!");
            }
            if (!startedInOffline || !isUpdated)
            {
                try
                {
                    string latestVersion = GetLatestVersion();
                    
                    if (latestVersion != currentVersion)
                    {
                        UtilMan.SendNotify("Starting update...");
                        Console.WriteLine("Updating application...");
                        Process.Start("LyumixLauncherUpdater.exe");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking for updates: {ex.Message}");
                }
            }
#endif
            UtilMan.Init();
            if (!startedInOffline)
            {
                await new ModuleInstaller().ReinstallAll();
            }

            if (!Directory.Exists(UtilMan.modulePath))
            {
                Directory.CreateDirectory(UtilMan.modulePath);
            }
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
            launcher = new LyumixLauncher(currentVersion);
            if (!startedByStartup)
            {
#if !DEBUG
                AddStartupShortcut();
#endif
                Application.Run(launcher);
            }
            Application.Run();
        }

        private static string GetLatestVersion()
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "request"); // GitHub API requires a User-Agent header
                return client.DownloadString(versionFileUrl).Trim();
            }
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
                launcher = new LyumixLauncher(currentVersion);
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
                launcher = new LyumixLauncher(currentVersion);
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