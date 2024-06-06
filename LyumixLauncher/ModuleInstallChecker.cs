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
