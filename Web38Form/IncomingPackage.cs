using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace Web38Form
{
    /// <summary>
    /// Вхідний пакет
    /// </summary>
    public class IncomingPackage
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="xmlData">ХМЛ дані</param>
        public IncomingPackage(string xmlData)
        {
            ServerParams = new Dictionary<int, string>();
            FunctionParams = new Dictionary<int, string>();

            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                xmlDoc.LoadXml(xmlData);
            }
            catch (Exception)
            {
                return;
            }

            XPathNavigator docNavigator = xmlDoc.CreateNavigator();

            // Функція
            XPathNavigator functionName = docNavigator.SelectSingleNode("/root/function");
            if (functionName != null)
            {
                Function = functionName.Value;
            }
            else
            {
                //Помилка в пакеті
                return;
            }

            // Параметри сервера
            XPathNodeIterator serverParamNodes = docNavigator.Select("/root/server/item");
            while (serverParamNodes.MoveNext())
            {
                ServerParams.Add(serverParamNodes.CurrentPosition - 1, serverParamNodes.Current.Value);
            }

            // Параметри функції 1С
            XPathNodeIterator functionParamNodes = docNavigator.Select("/root/params/item");
            while (functionParamNodes.MoveNext())
            {
                FunctionParams.Add(functionParamNodes.CurrentPosition - 1, functionParamNodes.Current.Value);
            }
        }

        /// <summary>
        /// Назва функції
        /// </summary>
        public string Function { get; private set; }

        /// <summary>
        /// Список параметрів сервера
        /// </summary>
        public Dictionary<int, string> ServerParams { get; private set; }

        /// <summary>
        /// Список параметрів функції
        /// </summary>
        public Dictionary<int, string> FunctionParams { get; private set; }

        /// <summary>
        /// Функція повертає масив параметрів для виклику 1С
        /// </summary>
        /// <returns>Масив типу object[]</returns>
        public object[] GetFunctionParams()
        {
            object[] functionPatamsArray = new object[FunctionParams.Count];

            foreach (KeyValuePair<int, string> itemFunctionPatams in FunctionParams)
                functionPatamsArray[itemFunctionPatams.Key] = itemFunctionPatams.Value;

            return functionPatamsArray;
        }
    }
}
