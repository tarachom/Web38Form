using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Net.Sockets;

namespace Web38Form
{
    static class Program
    {
        #region ГЛОБАЛЬНІ ЗМІННІ

        public static List<Socket> WorkSocketList = new List<Socket>();           //Список підключених сокетів
        public static AutoResetEvent AutoReset = new AutoResetEvent(false);       //Автоматика для регулювання потоків
        public static V83.COMConnector V83COMConnector = null;                    //Менеджер COM-соединений (COM connector)

        public static Config ConfigServer = null;

        public static bool ThreadsRun = true; //Признак запуску потоків

        ////Потік для прийому повідомлень
        //static Thread threadListener = null;

        ////Строка підключення до 1С
        //static string _1C_ConnectString = "";

        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
