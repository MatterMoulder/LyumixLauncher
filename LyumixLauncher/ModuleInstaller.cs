using System;
using System.Text.Json;

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
            string filePath = Path.Combine(UtilMan.modulePath, $"{moduleName}.json");
            if (Path.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var jsonObj = new UtilMan.ServiceJsonClass
            {
                Name = UtilMan.services.Find(s => s.srvc == moduleName).name,
                Version = UtilMan.services.Find(s => s.srvc == moduleName).version,
                Link = UtilMan.services.Find(s => s.srvc == moduleName).link
            };

            string jsonString = JsonSerializer.Serialize(jsonObj);
            
            await File.WriteAllTextAsync(filePath, jsonString);

            UtilMan.SendNotify($"{moduleName} is installed or updated!");

        }

        public async Task<bool> ReinstallAll()
        {
            ModuleInstaller installer = new ModuleInstaller();
            foreach (string item in UtilMan.GetAllInstalled())
            {
                Logger.Trace(item);
                string filePath = Path.Combine(UtilMan.modulePath, $"{UtilMan.services.Find(s => s.srvc == item).srvc}.json");
                if (Path.Exists(filePath))
                {
                    string jsonString = File.ReadAllText(filePath);

                    UtilMan.ServiceJsonClass jsonObj = JsonSerializer.Deserialize<UtilMan.ServiceJsonClass>(jsonString);

                    if (UtilMan.services.Find(s => s.srvc == item).version != jsonObj.Version)
                    {
                        InstallModule(item);
                    }
                } else
                {
                    InstallModule(item);
                }
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