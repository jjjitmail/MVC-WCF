using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace HttpWebManager
{
    public class SerializationManager<T> where T : class, new()
    {
        private string _fileName;
        public string FileName
        {
            get { return (string.IsNullOrEmpty(_fileName)) ? string.Format("{0}.xml", typeof(T).ToString()) : _fileName; }
            set { _fileName = value; }
        }
        private T _Content;
        public T Content
        {
            get { return _Content ?? new T(); }
            set { _Content = value; }
        }

        public void Save()
        {
            string filePath = Path.Combine(AssemblyManager.GetExecutionPath(), FileName);

            using (TextWriter writer = new StreamWriter(filePath))
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                xs.Serialize(writer, this.Content);
            }
        }
        public T Load()
        {
            string filePath = Path.Combine(AssemblyManager.GetExecutionPath(), FileName);

            using (TextReader reader = new StreamReader(filePath))
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                this.Content = xs.Deserialize(reader) as T;
            }

            return this.Content;
        }
        
    }
}
