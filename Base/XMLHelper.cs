using System;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ThongPanelFrame.Base
{
    public class XMLHelper
    {
        private static object objLoadLock = new object();

        private static object objSaveLock = new object();

        private string _filePath = string.Empty;

        private XmlDocument _xml;

        private XmlElement _element;

        public static object LoadObjectFromXml(string path, Type type)
        {
            lock (objLoadLock)
            {
                object result = null;
                using (StreamReader streamReader = new StreamReader(path))
                {
                    string xml = streamReader.ReadToEnd();
                    result = XmlConvertor.XmlToObject(xml, type);
                }

                return result;
            }
        }

        public static void SaveObjectToXml(string path, object obj)
        {
            lock (objSaveLock)
            {
                string value = XmlConvertor.ObjectToXml(obj, toBeIndented: true);
                using StreamWriter streamWriter = new StreamWriter(path);
                streamWriter.Write(value);
            }
        }

        public XMLHelper(string xmlFilePath)
        {
            _filePath = xmlFilePath;
        }

        private void CreateXMLElement()
        {
            _xml = new XmlDocument();
            if (File.Exists(_filePath))
            {
                _xml.Load(_filePath);
            }

            _element = _xml.DocumentElement;
        }

        public XmlNode GetNode(string xPath)
        {
            CreateXMLElement();
            return _element.SelectSingleNode(xPath);
        }

        public string GetValue(string xPath)
        {
            CreateXMLElement();
            return _element.SelectSingleNode(xPath).InnerText;
        }

        public string GetAttributeValue(string xPath, string attributeName)
        {
            CreateXMLElement();
            return _element.SelectSingleNode(xPath).Attributes[attributeName].Value;
        }

        public void AppendNode(XmlNode xmlNode)
        {
            CreateXMLElement();
            XmlNode newChild = _xml.ImportNode(xmlNode, deep: true);
            _element.AppendChild(newChild);
        }

        public void AppendNode(DataSet ds)
        {
            XmlDataDocument xmlDataDocument = new XmlDataDocument(ds);
            XmlNode firstChild = xmlDataDocument.DocumentElement.FirstChild;
            AppendNode(firstChild);
        }

        public void RemoveNode(string xPath)
        {
            CreateXMLElement();
            XmlNode oldChild = _xml.SelectSingleNode(xPath);
            _element.RemoveChild(oldChild);
        }

        public void Save()
        {
            CreateXMLElement();
            _xml.Save(_filePath);
        }

        private static XmlElement CreateRootElement(string xmlFilePath)
        {
            string text = "";
            text = xmlFilePath;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(text);
            return xmlDocument.DocumentElement;
        }

        public static string GetValue(string xmlFilePath, string xPath)
        {
            XmlElement xmlElement = CreateRootElement(xmlFilePath);
            return xmlElement.SelectSingleNode(xPath).InnerText;
        }

        public static string GetAttributeValue(string xmlFilePath, string xPath, string attributeName)
        {
            XmlElement xmlElement = CreateRootElement(xmlFilePath);
            return xmlElement.SelectSingleNode(xPath).Attributes[attributeName].Value;
        }

        private XmlDocument XMLLoad()
        {
            string filePath = _filePath;
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                string text = filePath;
                if (File.Exists(text))
                {
                    xmlDocument.Load(text);
                }
            }
            catch (Exception)
            {
            }

            return xmlDocument;
        }

        private static XmlDocument XMLLoad(string strPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                if (File.Exists(strPath))
                {
                    xmlDocument.Load(strPath);
                }
            }
            catch (Exception)
            {
            }

            return xmlDocument;
        }

        public string Read(string node)
        {
            string result = "";
            try
            {
                XmlDocument xmlDocument = XMLLoad();
                XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
                result = xmlNode.InnerText;
            }
            catch
            {
            }

            return result;
        }

        public static string Read(string path, string node)
        {
            string result = "";
            try
            {
                XmlDocument xmlDocument = XMLLoad(path);
                XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
                result = xmlNode.InnerText;
            }
            catch
            {
            }

            return result;
        }

        public static string Read(string path, string node, string attribute)
        {
            string result = "";
            try
            {
                XmlDocument xmlDocument = XMLLoad(path);
                XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
                result = (attribute.Equals("") ? xmlNode.InnerText : xmlNode.Attributes[attribute].Value);
            }
            catch
            {
            }

            return result;
        }

        public string[] ReadAllChildallValue(string node)
        {
            int num = 0;
            string[] array = new string[0];
            XmlDocument xmlDocument = XMLLoad();
            XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
            XmlNodeList childNodes = xmlNode.ChildNodes;
            if (childNodes.Count > 0)
            {
                array = new string[childNodes.Count];
                foreach (XmlElement item in childNodes)
                {
                    array[num] = item.Value;
                    num++;
                }
            }

            return array;
        }

        public XmlNodeList ReadAllChild(string node)
        {
            XmlDocument xmlDocument = XMLLoad();
            XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
            return xmlNode.ChildNodes;
        }

        public DataView GetDataViewByXml(string strWhere, string strSort)
        {
            try
            {
                string filePath = _filePath;
                string fileName = filePath;
                DataSet dataSet = new DataSet();
                dataSet.ReadXml(fileName);
                DataView dataView = new DataView(dataSet.Tables[0]);
                if (strSort != null)
                {
                    dataView.Sort = strSort;
                }

                if (strWhere != null)
                {
                    dataView.RowFilter = strWhere;
                }

                return dataView;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DataSet GetDataSetByXml(string strXmlPath)
        {
            try
            {
                DataSet dataSet = new DataSet();
                dataSet.ReadXml(strXmlPath);
                if (dataSet.Tables.Count > 0)
                {
                    return dataSet;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void Insert(string path, string node, string element, string attribute, string value)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(path);
                XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
                if (element.Equals(""))
                {
                    if (!attribute.Equals(""))
                    {
                        XmlElement xmlElement = (XmlElement)xmlNode;
                        xmlElement.SetAttribute(attribute, value);
                    }
                }
                else
                {
                    XmlElement xmlElement2 = xmlDocument.CreateElement(element);
                    if (attribute.Equals(""))
                    {
                        xmlElement2.InnerText = value;
                    }
                    else
                    {
                        xmlElement2.SetAttribute(attribute, value);
                    }

                    xmlNode.AppendChild(xmlElement2);
                }

                xmlDocument.Save(path);
            }
            catch
            {
            }
        }

        public static void Insert(string path, string node, string element, string[][] strList)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(path);
                XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
                XmlElement xmlElement = xmlDocument.CreateElement(element);
                string text = "";
                string text2 = "";
                for (int i = 0; i < strList.Length; i++)
                {
                    for (int j = 0; j < strList[i].Length; j++)
                    {
                        if (j == 0)
                        {
                            text = strList[i][j];
                        }
                        else
                        {
                            text2 = strList[i][j];
                        }
                    }

                    if (text.Equals(""))
                    {
                        xmlElement.InnerText = text2;
                    }
                    else
                    {
                        xmlElement.SetAttribute(text, text2);
                    }
                }

                xmlNode.AppendChild(xmlElement);
                xmlDocument.Save(path);
            }
            catch
            {
            }
        }

        public static bool WriteXmlByDataSet(string strXmlPath, string[] Columns, string[] ColumnValue)
        {
            try
            {
                string fileName = strXmlPath.Substring(0, strXmlPath.IndexOf(".")) + ".xsd";
                DataSet dataSet = new DataSet();
                dataSet.ReadXmlSchema(fileName);
                dataSet.ReadXml(strXmlPath);
                DataTable dataTable = dataSet.Tables[0];
                DataRow dataRow = dataTable.NewRow();
                for (int i = 0; i < Columns.Length; i++)
                {
                    dataRow[Columns[i]] = ColumnValue[i];
                }

                dataTable.Rows.Add(dataRow);
                dataTable.AcceptChanges();
                dataSet.AcceptChanges();
                dataSet.WriteXml(strXmlPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Update(string node, string value)
        {
            try
            {
                XmlDocument xmlDocument = XMLLoad();
                XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
                xmlNode.InnerText = value;
                xmlDocument.Save(_filePath);
            }
            catch
            {
            }
        }

        public static void Update(string path, string node, string value)
        {
            try
            {
                XmlDocument xmlDocument = XMLLoad(path);
                XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
                xmlNode.InnerText = value;
                xmlDocument.Save(path);
            }
            catch
            {
            }
        }

        public static void Update(string path, string node, string attribute, string value)
        {
            try
            {
                XmlDocument xmlDocument = XMLLoad(path);
                XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
                XmlElement xmlElement = (XmlElement)xmlNode;
                if (attribute.Equals(""))
                {
                    xmlElement.InnerText = value;
                }
                else
                {
                    xmlElement.SetAttribute(attribute, value);
                }

                xmlDocument.Save(path);
            }
            catch
            {
            }
        }

        public static bool UpdateXmlRow(string strXmlPath, string[] Columns, string[] ColumnValue, string strWhereColumnName, string strWhereColumnValue)
        {
            try
            {
                string fileName = strXmlPath.Substring(0, strXmlPath.IndexOf(".")) + ".xsd";
                DataSet dataSet = new DataSet();
                dataSet.ReadXmlSchema(fileName);
                dataSet.ReadXml(strXmlPath);
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        if (dataSet.Tables[0].Rows[i][strWhereColumnName].ToString().Trim().Equals(strWhereColumnValue))
                        {
                            for (int j = 0; j < Columns.Length; j++)
                            {
                                dataSet.Tables[0].Rows[i][Columns[j]] = ColumnValue[j];
                            }

                            dataSet.AcceptChanges();
                            dataSet.WriteXml(strXmlPath);
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void Delete(string path, string node)
        {
            try
            {
                XmlDocument xmlDocument = XMLLoad(path);
                XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
                xmlNode.ParentNode.RemoveChild(xmlNode);
                xmlDocument.Save(path);
            }
            catch
            {
            }
        }

        public static void Delete(string path, string node, string attribute)
        {
            try
            {
                XmlDocument xmlDocument = XMLLoad(path);
                XmlNode xmlNode = xmlDocument.SelectSingleNode(node);
                XmlElement xmlElement = (XmlElement)xmlNode;
                if (attribute.Equals(""))
                {
                    xmlNode.ParentNode.RemoveChild(xmlNode);
                }
                else
                {
                    xmlElement.RemoveAttribute(attribute);
                }

                xmlDocument.Save(path);
            }
            catch
            {
            }
        }

        public static bool DeleteXmlAllRows(string strXmlPath)
        {
            try
            {
                DataSet dataSet = new DataSet();
                dataSet.ReadXml(strXmlPath);
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    dataSet.Tables[0].Rows.Clear();
                }

                dataSet.WriteXml(strXmlPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DeleteXmlRowByIndex(string strXmlPath, int iDeleteRow)
        {
            try
            {
                DataSet dataSet = new DataSet();
                dataSet.ReadXml(strXmlPath);
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    dataSet.Tables[0].Rows[iDeleteRow].Delete();
                }

                dataSet.WriteXml(strXmlPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DeleteXmlRows(string strXmlPath, string strColumn, string[] ColumnValue)
        {
            try
            {
                DataSet dataSet = new DataSet();
                dataSet.ReadXml(strXmlPath);
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    if (ColumnValue.Length > dataSet.Tables[0].Rows.Count)
                    {
                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                        {
                            for (int j = 0; j < ColumnValue.Length; j++)
                            {
                                if (dataSet.Tables[0].Rows[i][strColumn].ToString().Trim().Equals(ColumnValue[j]))
                                {
                                    dataSet.Tables[0].Rows[i].Delete();
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int k = 0; k < ColumnValue.Length; k++)
                        {
                            for (int l = 0; l < dataSet.Tables[0].Rows.Count; l++)
                            {
                                if (dataSet.Tables[0].Rows[l][strColumn].ToString().Trim().Equals(ColumnValue[k]))
                                {
                                    dataSet.Tables[0].Rows[l].Delete();
                                }
                            }
                        }
                    }

                    dataSet.WriteXml(strXmlPath);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public sealed class XmlConvertor
    {
        private XmlConvertor()
        {
        }

        public static object XmlToObject(string xml, Type type)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            if (null == type)
            {
                throw new ArgumentNullException("type");
            }

            object result = null;
            XmlSerializer xmlSerializer = new XmlSerializer(type);
            StringReader input = new StringReader(xml);
            XmlReader xmlReader = new XmlTextReader(input);
            try
            {
                result = xmlSerializer.Deserialize(xmlReader);
            }
            catch (InvalidOperationException innerException)
            {
                throw new InvalidOperationException("Can not convert xml to object", innerException);
            }
            finally
            {
                xmlReader.Close();
            }

            return result;
        }

        public static string ObjectToXml(object obj, bool toBeIndented)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            UTF8Encoding uTF8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
            MemoryStream memoryStream = new MemoryStream();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, uTF8Encoding);
            xmlTextWriter.Formatting = (toBeIndented ? Formatting.Indented : Formatting.None);
            try
            {
                xmlSerializer.Serialize(xmlTextWriter, obj);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Can not convert object to xml." + ex.InnerException);
            }
            finally
            {
                xmlTextWriter.Close();
            }

            return uTF8Encoding.GetString(memoryStream.ToArray());
        }
    }
}