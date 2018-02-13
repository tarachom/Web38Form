using System.Xml.XPath;
using System.IO;

namespace Web38Form
{
    /// <summary>
    /// Конфігураційний файл
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="configPath">Шлях до файлу конфігурації</param>
        public Config(string configPath)
        {
            IP = Port = CountThread = ConnectString = "";

            if (!File.Exists(configPath))
            {
                State = -1;
                Error = "Не знайдений конфігураційний файл";
                return;
            }

            XPathDocument xmlDoc = new XPathDocument(configPath);
            XPathNavigator docNavigator = xmlDoc.CreateNavigator();

            // IP
            XPathNavigator IPNode = docNavigator.SelectSingleNode("/Web/IPSocketWork");
            if (IPNode != null) IP = IPNode.Value;

            // Порт
            XPathNavigator PortNode = docNavigator.SelectSingleNode("/Web/PortSocketWork");
            if (PortNode != null) Port = PortNode.Value;

            // Кількість робочих потоків
            XPathNavigator CountThreadNode = docNavigator.SelectSingleNode("/Web/CountWorkThread");
            if (CountThreadNode != null) CountThread = CountThreadNode.Value;

            // Вітка з шляхом до бази даних 1С
            XPathNavigator ConnectStringNode = docNavigator.SelectSingleNode("/Web/ConnectString");
            if (ConnectStringNode != null) ConnectString = ConnectStringNode.Value;

            State = 1;
        }

        /// <summary>
        /// Стан
        /// </summary>
        public int State { get; private set; }

        /// <summary>
        /// Інформація про помилки
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// ІР
        /// </summary>
        public string IP { get; private set; }

        /// <summary>
        /// Порт
        /// </summary>
        public string Port { get; private set; }

        /// <summary>
        /// Кількість робочих потоків
        /// </summary>
        public string CountThread { get; private set; }

        /// <summary>
        /// Строка підключення до 1С
        /// </summary>
        public string ConnectString { get; private set; }
    }
}
