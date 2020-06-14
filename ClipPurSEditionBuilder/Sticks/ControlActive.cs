namespace ClipPurSEditionBuilder.Sticks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    internal class ControlActive
    {
        /// <summary>
        /// Метод для временного показа элемента Label
        /// </summary>
        /// <param name="l">Имя Label</param>
        /// <param name="visible"></param>
        private static void Active(Label l, bool visible = false)
        {
            try
            {
                Task.Run(() =>
                {
                    Thread.Sleep(2000);
                    l.Invoke((Action)(() => l.Visible = visible));
                });
            }
            catch { }
        }

        /// <summary>
        /// Метод для записи текста и вывод этого сообщения
        /// </summary>
        /// <param name="MessageShow"></param>
        /// <param name="Text"></param>
        /// <param name="True"></param>
        public static void CheckMessage(Label MessageShow, string Text, bool True = true)
        {
            MessageShow.Visible = True;
            Active(MessageShow);
            MessageShow.Text = Text;
        }
        
        /// <summary>
        /// Метод для показа элементов Panel для UserControl'ов
        /// </summary>
        /// <param name="Panl">Имя Панели</param>
        /// <param name="Uc">Имя Пользовательского интерфейса</param>
        public static void ControlVisible(Panel Panl, UserControl Uc)
        {
            try
            {
               Panl.Controls.Add(Uc);
               Uc.BringToFront();
            }
            catch (Exception) { }
        }
        /// <summary>
        /// Метод для выполнения анимации формы
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="time"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool ShowAnima(IntPtr hWnd, int time, Enums.AnimateWindowFlags flags) => NativeMethods.AnimateWindow(hWnd, time, flags);

        /// <summary>
        /// Метод для очистки всех элементов TextBox
        /// </summary>
        /// <param name="parent">this элемент</param>
        public static void CleanAllTextBoxesIn(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                    c.Text = string.Empty;

                if (c.GetType() == typeof(GroupBox))
                    CleanAllTextBoxesIn(c);
            }
        }
    }
}