namespace ClipPurSEditionBuilder.Sticks
{
    using System;
    using System.Net;

    public class INetConnect
    {
        /// <summary>
        /// Метод для проверки подключения к интернет ресурсу
        /// </summary>
        /// <param name="url">Ссылка на сайт</param>
        /// <returns>Значение true или false</returns>
        public static bool Inizialize(string url)
        {
            try
            {
                var Link = new Uri(url, UriKind.Absolute);
                using var Client = new WebClient
                {
                    Proxy = null
                };
                using (Client.OpenRead(Link)) return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Метод для получения ключа пользователя с расшифровкой 
        /// </summary>
        /// <param name="url">Ссылка на сайт</param>
        /// <returns>Расшифрованный ключ</returns>
        public static string GetOnlineKey(string url)
        {
            string result = string.Empty;
            if (string.IsNullOrWhiteSpace(url)) return result;

            try
            {
                var Link = new Uri(url, UriKind.Absolute);
                using var Client = new WebClient
                {
                    Proxy = null
                };
                return Client?.DownloadString(Link);
            }
            catch { return result; }
        }

        /// <summary>
        /// Метод для получения исходного кода билда с расшифровкой
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetOnlineBuild(string url)
        {
            string result = string.Empty;
            if (string.IsNullOrWhiteSpace(url)) return result;

            try
            {
                var Link = new Uri(url, UriKind.Absolute);
                using var Client = new WebClient
                {
                    Proxy = null
                };
                string sourcecode = Client?.DownloadString(url);
                return !sourcecode.StartsWith("#") ? result : EncryptKey.Decrypt(sourcecode, GlobalPath.SecretKey_Build);
            }
            catch { return result; }
        }
    }
}