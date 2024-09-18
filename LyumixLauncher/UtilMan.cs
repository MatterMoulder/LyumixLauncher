using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace LyumixLauncher
{
    internal class UtilMan
    {
        public static List<Service> services;
        public static List<LServices> lServices;
        public static NotifyIcon notifyIcon;
        public static List<string> installedSrvcs;
        public static string modulePath = Path.Combine(AppContext.BaseDirectory, "modules");
        public static ToolStripMenuItem shareItem = new ToolStripMenuItem("Open App");
        public static ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
        public static string WindowsStartupFolder => Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        public static string AppShortcutPath => Path.Join(WindowsStartupFolder, "LyumixLauncher.lnk");
        public static string? ExePath => Process.GetCurrentProcess().MainModule?.FileName;
        public class Service
        {
            public string srvc;
            public string name;
            public int version;
            public string link;
        }
        public class LServices
        {

            public string name;
            public int version;
            public bool state;
        }

        public static void Init()
        {
            var json = new WebClient().DownloadString("https://raw.githubusercontent.com/MatterMoulder/LLmodules/main/apps.json");
            services = JsonConvert.DeserializeObject<List<Service>>(json) ?? new List<Service>(null);
            notifyIcon = new NotifyIcon
            {
                Icon = Properties.Resources.LyumixICO,
                Text = "Lyumix Launcher",
                Visible = true
            };
            if (Properties.Settings.Default.AutoRun.Length > 0)
            {
                var lJson = Properties.Settings.Default.AutoRun;
                lServices = JsonConvert.DeserializeObject<List<LServices>>(lJson);
            } else
            {
                lServices = new List<LServices>();
            }
            if (Properties.Settings.Default.Installed.Length > 0)
            {
                var iJson = Properties.Settings.Default.Installed;
                installedSrvcs = JsonConvert.DeserializeObject<List<string>>(iJson);
            } else
            {
                installedSrvcs = new List<string>();
            }
        }

        public static void UpdateInstalledServices(string srv)
        {
            if (!installedSrvcs.Contains(srv))
            {
                installedSrvcs.Add(srv);
            }
            Properties.Settings.Default.Installed = JsonConvert.SerializeObject(installedSrvcs);
            Properties.Settings.Default.Save();
        }

        public static List<string> GetAllInstalled()
        {
            return installedSrvcs;
        }

        public static void CreateStripMenu(ContextMenuStrip contextMenu)
        {
            if (contextMenu.Items.Count > 0)
            {
                ToolStripSeparator toolSeparator = new ToolStripSeparator();
                contextMenu.Items.Add(toolSeparator);
            }

            contextMenu.Items.Add(shareItem);
            contextMenu.Items.Add(exitItem);
            notifyIcon.ContextMenuStrip = contextMenu;
        }

        public static void UpdateStartUp(string moduleName, bool moduleState)
        {
            if (lServices.Find(s => s.name == moduleName) != null)
            {
                lServices.Remove(lServices.Find(s => s.name == moduleName));
            }
            lServices.Add(new LServices { name = moduleName, state = moduleState });
            Properties.Settings.Default.AutoRun = JsonConvert.SerializeObject(lServices);
            Properties.Settings.Default.Save();
        }

        public static bool GetInfoFromSysVars(string moduleName)
        {
            if (lServices.Find(s => s.name == moduleName) != null)
            {
                return lServices.Find(s => s.name == moduleName).state;
            } else
            {
                return false;
            }
        }

        public static List<string> GetAllStartUp()
        {
            if (lServices.Count > 0)
            {
                List<string> lstr = new List<string>();
                foreach (var item in lServices)
                {
                    if (item.state)
                    {
                        lstr.Add(item.name);
                    }
                }
                if (lstr.Count > 0)
                {
                    return lstr;
                } else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static void SendNotify(string txt)
        {
            notifyIcon.BalloonTipTitle = "Lyumix Launcher";
            notifyIcon.BalloonTipText = txt;
            notifyIcon.ShowBalloonTip(3000);
        }

        public class ServiceJsonClass
        {
            public string Name { get; set; }
            public int Version { get; set; }
            public string Link { get; set; }
        }
    }
}
