using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyumixLauncher
{
    internal class ModuleInstallChecker
    {
        public static bool IsModuleInstalled(string moduleName)
        {
            string path = Path.Combine(UtilMan.modulePath, $"{moduleName}.dll");
            Logger.Trace(path);
            if (Path.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
