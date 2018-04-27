using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace UserInterface
{
    class TopicType
    {
        private string dataTopic;
        public string name;
        public List<String> items = new List<string>();

        public TopicType()
        {
        }

        public TopicType(string datalink)
        {
            ReadData(datalink);
            SetUpForTopic();
        }

        public void ReadData(string link)
        {
            try
            {
                using (StreamReader read = new StreamReader(link))
                {
                    dataTopic = read.ReadToEnd();
                }
            }
            catch (Exception exceptionWhenRead)
            {
               System.Windows.MessageBox.Show(exceptionWhenRead.Message);
            }
        }

        public void SetUpForTopic()
        {
            name = dataTopic.Substring(1, dataTopic.IndexOf("\t\r\n") - 1);
            MatchCollection matchCollection;
            Regex regex = new Regex("(?<=-)[\\S\\D ]+?(?=\t\r)");
            matchCollection = regex.Matches(dataTopic);
            foreach (Match m in matchCollection)
            {
                try
                {
                    items.Add(m.Value);
                }
                catch { }
            }
        }
    }
}
