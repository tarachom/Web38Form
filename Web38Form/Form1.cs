using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace Web38Form
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Лог

        delegate void SetTextCallback(string text);

        /// <summary>
        /// Добавити заголовок в ЛОГ
        /// </summary>
        /// <param name="text">Заголовок</param>
        private void AppendCaption(string text)
        {
            try
            {
                if (richTextBox1.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(AppendCaption);
                    this.Invoke(d, new object[] { text });
                }
                else
                {
                    richTextBox1.SelectionBackColor = Color.Beige;
                    richTextBox1.AppendText("\n" + text + "\n");
                    richTextBox1.ScrollToCaret();
                }
            }
            catch { }
        }

        /// <summary>
        /// Добавити текст в ЛОГ
        /// </summary>
        /// <param name="text">Текст</param>
        private void AppendText(string text)
        {
            try
            {
                if (richTextBox1.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(AppendText);

                    this.Invoke(d, new object[] { text });
                }
                else
                {
                    richTextBox1.ForeColor = Color.Black;
                    richTextBox1.AppendText(DateTime.Now.ToString("[dd.MM HH:mm:ss] ") + text + "\n");
                    richTextBox1.ScrollToCaret();

                    if (richTextBox1.Lines.Length > 1000)
                    {
                        richTextBox1.Clear();
                        richTextBox1.ScrollToCaret();
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Запис тексту із richTextBox1 у файл
        /// </summary>
        private void DISABLE_SaveToLogFile()
        {
            string logDir = AppDomain.CurrentDomain.BaseDirectory + "log";
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            else
            {
                /* видалення старих логів */
                //DeleteOldLog(logDir);
            }

            string logTextFile = Path.Combine(logDir, "log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt");

            File.AppendAllLines(logTextFile, richTextBox1.Lines);
            richTextBox1.Clear();
            richTextBox1.ScrollToCaret();
        }

        /// <summary>
        /// Видалення старих логів
        /// </summary>
        private void DISABLE_DeleteOldLog(string dirPath)
        {
            string[] log_files = Directory.GetFiles(dirPath);

            DateTime old_date = DateTime.Now.AddDays(-7);

            foreach (string log_file in log_files)
            {
                DateTime log_file_create = File.GetCreationTime(log_file);
                string extension = Path.GetExtension(log_file);

                if (extension == ".txt")
                {
                    if (log_file_create <= old_date)
                        try
                        {
                            File.Delete(log_file);
                        }
                        catch { }
                }
            }
        }

        #endregion

        #region Listener

        /// <summary>
        /// Основний потік для прийому вхідних повідомлень
        /// </summary>
        private void StartListenerThread()
        {
            Thread threadListener = new Thread(new ThreadStart(ListenerWorker));
            threadListener.IsBackground = true;
            threadListener.Start();

            AppendText("Listener - Start");
        }

        /// <summary>
        /// Функція прийому вхідних повідомлень
        /// </summary>
        void ListenerWorker()
        {
            //IP сервера на який будуть приходити повідомлення
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(Program.ConfigServer.IP), int.Parse(Program.ConfigServer.Port));

            //Створення основного сокета типу Stream на протоколі Tcp/IP
            Socket soketWork = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //Привязка до IP
                soketWork.Bind(localEndPoint);

                //Максимальна кількість підключень
                soketWork.Listen(1000);

                AppendText("Listener Listen");

                while (Program.ThreadsRun)
                {
                    if (checkBoxVisibleLogXml.Checked)
                        AppendText("Listener Waiting");

                    //Очікування підключення
                    Socket soketAccept = soketWork.Accept();
                    soketAccept.ReceiveTimeout = 5000;

                    if (checkBoxVisibleLogXml.Checked)
                        AppendText("Accept [" + soketAccept.RemoteEndPoint.ToString() + "]");

                    //Добавлення підключеного сокета в список для обробки на окремих потоках
                    lock (Program.WorkSocketList)
                        Program.WorkSocketList.Add(soketAccept);

                    //Команда СТАРТ обробки списку повідомлень для окремих потоків
                    Program.AutoReset.Set();
                }
            }
            catch (Exception ex)
            {
                AppendText("Listener Exception - " + ex.Message);
                Program.ThreadsRun = false;
            }
            finally
            {
                //Закриваю підключення основного сокета
                soketWork.Close();
            }

            AppendText("Listener - Close");
        }

        #endregion

        #region WorkerThread

        /// <summary>
        /// Потоки для обробки повідомлень
        /// </summary>
        /// <param name="count">Кількість фонових потоків</param>
        private void CreateWorkerThread(int count)
        {
            AppendText("Потоки - Створення");

            for (int i = 0; i < count; i++)
            {
                if (Program.ThreadsRun)
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(Work));
                    thread.IsBackground = true;
                    thread.Start((object)i);

                    AppendText("Поток [" + i.ToString() + "] - Старт");
                }
            }
        }

        /// <summary>
        /// Функція для обробки повідомлень
        /// </summary>
        /// <param name="p">Номер потоку</param>
        public void Work(object p)
        {
            object v83Base = null;

            try
            {
                lock (Program.V83COMConnector)
                    v83Base = Program.V83COMConnector.Connect(Program.ConfigServer.ConnectString);

                AppendText("Поток [" + p.ToString() + "][Connect] - OK");
            }
            catch (Exception ex)
            {
                AppendText("Поток [" + p.ToString() + "][Error] - " + ex.Message);
                return;
            }

            while (Program.ThreadsRun)
            {
                //Сокет
                Socket soketAccept = null;

                //Перевірка списку підключених сокетів
                lock (Program.WorkSocketList)
                    if (Program.WorkSocketList.Count != 0)
                    {
                        soketAccept = Program.WorkSocketList[0];
                        Program.WorkSocketList.RemoveAt(0);
                    }

                //Обробка підключеного сокета
                if (soketAccept != null)
                {
                    //буфер для сокета
                    Byte[] buffer = new Byte[1024];

                    try
                    {
                        int receiveByte = 0;
                        string receiveXmlText = "";

                        //Зчитую дані
                        do
                        {
                            Console.WriteLine(soketAccept.Available);
                            receiveByte = soketAccept.Receive(buffer);
                            receiveXmlText += Encoding.GetEncoding(1251).GetString(buffer, 0, receiveByte);
                        }
                        while (soketAccept.Available > 0);

                        AppendText("<-- Поток [" + p.ToString() + "][receive: " + receiveXmlText.Length.ToString() + "] " + (checkBoxVisibleLogXml.Checked ? receiveXmlText : ""));

                        IncomingPackage InPackage = new IncomingPackage(receiveXmlText);

                        //Перевірка параметрів сервера (кешування)
                        //...

                        //Виклик функції/методу в 1с-ці
                        object returnValue = v83Base.GetType().InvokeMember(InPackage.Function, 
                            BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, v83Base, InPackage.GetFunctionParams());

                        //Відправка результату
                        if (returnValue != null)
                        {
                            string sendText = returnValue.ToString();
                            soketAccept.Send(Encoding.GetEncoding(1251).GetBytes(Convert.ToString(returnValue)));

                            //Видалення даних base64, щоб не засоряти лог
                            int openBase64Tag = 0;
                            int closeBase64Tag = 0;
                            while ((openBase64Tag = sendText.IndexOf("<base64>", closeBase64Tag)) > 0)
                            {
                                // + <base64>
                                int startRemove = openBase64Tag + 8;

                                closeBase64Tag = sendText.IndexOf("</base64>", startRemove);
                                if (closeBase64Tag > startRemove)
                                {
                                    sendText = sendText.Remove(startRemove, closeBase64Tag - startRemove);

                                    // + </base64>
                                    closeBase64Tag = startRemove + 9;
                                }
                            }

                            AppendText("--> Поток [" + p.ToString() + "][send: " + sendText.Length.ToString() + "] " + (checkBoxVisibleLogXml.Checked ? sendText : ""));
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendText("-> Поток [" + p.ToString() + "][Exception] - " + ex.Message + "\n" + ex.StackTrace);
                        soketAccept.Send(Encoding.GetEncoding(1251).GetBytes(ex.Message + "\n" + ex.StackTrace));
                    }
                    finally
                    {
                        soketAccept.Close();
                    }
                }
                else
                {
                    if (checkBoxVisibleLogXml.Checked)
                        AppendText("-> Поток [" + p.ToString() + "] - Pause");

                    Program.AutoReset.WaitOne();
                }
            }

            v83Base = null;

            AppendText("Поток [" + p.ToString() + "] - Close");
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }

        private void Start()
        {
            AppendCaption("КОНФІГУРАЦІЯ");
            Program.ConfigServer = new Config(AppDomain.CurrentDomain.BaseDirectory + "config.xml");

            if (Program.ConfigServer.State == -1)
            {
                AppendText(Program.ConfigServer.Error);
                return;
            }

            AppendText("IP: " + Program.ConfigServer.IP);
            AppendText("Port: " + Program.ConfigServer.Port);
            AppendText("Count thread: " + Program.ConfigServer.CountThread);
            AppendText("Connect string: " + Program.ConfigServer.ConnectString);

            //Менеджер COM-соединений (COM connector)
            Program.V83COMConnector = new V83.COMConnector();
            Program.V83COMConnector.PoolCapacity = 3;
            Program.V83COMConnector.PoolTimeout = 10;

            AppendCaption("ЗАПУСК ПОТОКІВ");

            // 1
            StartListenerThread();

            // 2
            CreateWorkerThread(int.Parse(Program.ConfigServer.CountThread));
        }

        private void Stop()
        {

            Program.ThreadsRun = false;

            try
            {
                //Зупинка Лістенера
                using (Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    sender.Connect(Program.ConfigServer.IP, int.Parse(Program.ConfigServer.Port));
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
            }
            catch { }

            try
            {
                //Зупинка робочих потоків
                for (int i = 0; i < int.Parse(Program.ConfigServer.CountThread); i++)
                    Program.AutoReset.Set();
            }
            catch { }

            Program.AutoReset.Dispose();

            Thread.Sleep(1000);

            try
            {
                //Очистка
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch { }

            try
            {
                if (Program.V83COMConnector != null)
                {
                    Marshal.ReleaseComObject(Program.V83COMConnector);
                    Program.V83COMConnector = null;
                }
            }
            catch { }
        }

        private void notifyIconWeb38Form_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // делаем нашу иконку скрытой
            notifyIconWeb38Form.Visible = false;

            //разворачиваем окно
            WindowState = FormWindowState.Normal;

            // возвращаем отображение окна в панели
            this.ShowInTaskbar = true;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // проверяем наше окно, и если оно было свернуто, делаем событие        
            if (WindowState == FormWindowState.Minimized)
            {
                // прячем наше окно из панели
                this.ShowInTaskbar = false;

                // делаем нашу иконку в трее активной
                notifyIconWeb38Form.Visible = true;
            }
        }
    }
}
