using DiscordRPC.Logging;
using System;
using System.IO;
using System.Linq;

namespace LyumixLauncher {
    internal class Logger {
        public static void Trace(string message) {
#if DEBUG
            System.Diagnostics.Trace.WriteLine(message);
#endif
        }
    }
}
