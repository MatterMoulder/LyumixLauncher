namespace LyumixLauncher
{
    internal class ModuleInstaller
    {
        private static HttpClient client = new HttpClient();
        public async Task InstallModule(string moduleName)
        {
            string url = UtilMan.services.Find(s => s.srvc == moduleName).link;
            string dllPath = Path.Combine(UtilMan.modulePath, $"{moduleName}.dll");
            if (!Path.Exists(dllPath))
            {
                using (HttpClient client = new HttpClient())
                {
                    byte[] dllBytes = await client.GetByteArrayAsync(url);
                    await File.WriteAllBytesAsync(dllPath, dllBytes);
                }
                UtilMan.UpdateInstalledServices(moduleName);
            }
            else
            {
                File.Delete(dllPath);
                using (HttpClient client = new HttpClient())
                {
                    byte[] dllBytes = await client.GetByteArrayAsync(url);
                    await File.WriteAllBytesAsync(dllPath, dllBytes);
                }
            }
        }

        public async Task<bool> ReinstallAll()
        {
            ModuleInstaller installer = new ModuleInstaller();
            foreach (string item in UtilMan.GetAllInstalled())
            {
                Logger.Trace(item);
                string url = UtilMan.services.Find(s => s.srvc == item).link;
                string dllPath = Path.Combine(UtilMan.modulePath, $"{item}.dll");

                
                byte[] dllBytes = await client.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(dllPath, dllBytes);
                Logger.Trace(item);
            }
            Logger.Trace("END");
            return true;
        }

        public static List<string> GetAllModules()
        {
            List<string> modules = new List<string>();
            foreach (UtilMan.LServices item in UtilMan.lServices)
            {
                modules.Add(item.name);
            }
            return modules;
        }
    }
}