﻿namespace ClipPurSEditionBuilder.ControlsSE
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;
    using Sticks;

    public partial class InfoSE : UserControl
    {
        public InfoSE() => InitializeComponent();

        private void BitWallet_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText("1CFk2hvtYWPKsyqLHjtidJ71qNsrHeAt3U");
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase))
                {
                    ControlActive.CheckMessage(this.ShowMessage, Language.GlobalMessageStrings_en_US.BitCopy);
                }
                else
                {
                    ControlActive.CheckMessage(this.ShowMessage, Language.GlobalMessageStrings_ru_RU.BitCopy);
                }
            }
            catch (Exception)
            {
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase))
                {
                    ControlActive.CheckMessage(this.ShowMessage, Language.GlobalMessageStrings_en_US.ErrorBitCopy);
                }
                else
                {
                    ControlActive.CheckMessage(this.ShowMessage, Language.GlobalMessageStrings_ru_RU.ErrorBitCopy);
                }
            }
        }

        private void InfoTwo_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start("tg://resolve?domain=r3xq1");
            }
            catch (Exception) { }
        }
    }
}