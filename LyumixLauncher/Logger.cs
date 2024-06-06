namespace LyumixLauncher {
    internal class Logger {
        public static void Trace(string message) {
#if DEBUG
            System.Diagnostics.Trace.WriteLine(message);
#endif
        }
    }
}
