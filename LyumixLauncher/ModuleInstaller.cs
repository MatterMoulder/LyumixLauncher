using Swan.Formatters;
using System;
using System.Reflection;
using System.Text.Json;
using static LyumixLauncher.ModuleStarter;

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

        public List<PropertyItem> DiscoverProperties(string moduleName)
        {
            string dllPath = Path.Combine(UtilMan.modulePath, $"{moduleName}.dll");
            Assembly assembly = Assembly.LoadFrom(dllPath);
            /*
            foreach (Type type in assembly.GetTypes())
            {
                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.PropertyType.ToString() == "System.Boolean")
                    {
                        Logger.Trace(property.Name + " - " + property.PropertyType + " - " + type.GetProperty(property.Name).);
                    }                    
                }
            }*/

            Type moduleType = assembly.GetType($"{moduleName}.Main");
            object loadedModuleInstance = Activator.CreateInstance(moduleType);
            MethodInfo getJsonSettingsMethod = moduleType.GetMethod("GetProperties");

            if (getJsonSettingsMethod != null)
            {
                string propertiesRaw = (string)getJsonSettingsMethod.Invoke(loadedModuleInstance, null);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // To ignore case differences between JSON and C# properties
                };

                List<PropertyItem> properties = JsonSerializer.Deserialize<List<PropertyItem>>(propertiesRaw, options);

                return properties;
            }
            else
            {
                Console.WriteLine($"GetJsonSettings method not found in {moduleName}.Main");
            }
            return null;
        }

        public void ChangeSettings(string moduleName, Dictionary<string, object> properties)
        {
            string dllPath = Path.Combine(UtilMan.modulePath, $"{moduleName}.dll");
            Assembly assembly = Assembly.LoadFrom(dllPath);
            Type localType = assembly.GetType($"{moduleName}.Main");
            object loadedModuleInstance = Activator.CreateInstance(localType);

            MethodInfo updateMethod = localType.GetMethod("UpdateProperties");
            updateMethod.Invoke(loadedModuleInstance, new object[] { properties });
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