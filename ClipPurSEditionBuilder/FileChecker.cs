namespace ClipPurSEditionBuilder
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Sticks;

    public static class FileChecker
    {
        public static bool GetDllCheck(string dllname, string ver)
        {
            try
            {
                string GetNameDll = FileControl.CombinePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dllname);
                return FileVersionInfo.GetVersionInfo(GetNameDll).ProductVersion.Equals(ver, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception) { return false; }
        }
    }
}