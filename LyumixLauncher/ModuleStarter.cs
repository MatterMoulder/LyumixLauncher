using System.Reflection;
using System.Text.Json.Nodes;

namespace LyumixLauncher
{
    internal class ModuleStarter
    {
        public class Srvc
        {
            public string name;
            public object obj;
        }

        public class TypeSrvc
        {
            public string name;
            public Type type;
        }

        private static List<Srvc> loadedModules = new List<Srvc>();
        private static List<TypeSrvc> type = new List<TypeSrvc>();

        public static void StartModule(string moduleName)
        {
            string dllPath = Path.Combine(UtilMan.modulePath, $"{moduleName}.dll");
            Assembly assembly = Assembly.LoadFrom(dllPath);
            type.Add(new TypeSrvc { name = moduleName, type = assembly.GetType($"{moduleName}.Main") });
            Type localType = type.Find(s => s.name == moduleName).type;
            MethodInfo method = localType.GetMethod("App");
            object loadedModuleInstance = Activator.CreateInstance(localType);
            loadedModules.Add(new Srvc { name = moduleName, obj = loadedModuleInstance });
            method.Invoke(loadedModules.Find(s => s.name == moduleName).obj, null);
        }

        public static void StopModule(string moduleName)
        {
            if (loadedModules.Find(s => s.name == moduleName).obj != null)
            {
                // Assuming there's a method named "Stop" in your module
                Type localType = type.Find(s => s.name == moduleName).type;
                MethodInfo stopMethod = localType.GetMethod("Stop");
                stopMethod.Invoke(loadedModules.Find(s => s.name == moduleName).obj, null);
                loadedModules.Remove(loadedModules.Find(s => s.name == moduleName));
                type.Remove(type.Find(s => s.name == moduleName));
            }
            else
            {
                Console.WriteLine("Module not loaded.");
            }
        }

        public static bool ModuleChecker(string moduleName)
        {
            if (!(loadedModules.Count == 0))
            {
                if (loadedModules.Find(s => s.name == moduleName) != null)
                {
                    Console.WriteLine("Module already loaded.");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        public static void StopAllModules()
        {
            foreach (Srvc module in loadedModules)
            {
                Type localType = type.Find(s => s.name == module.name).type;
                MethodInfo stopMethod = localType.GetMethod("Stop");
                stopMethod.Invoke(module.obj, null);
            }
            loadedModules.Clear();
        }
    }
}
