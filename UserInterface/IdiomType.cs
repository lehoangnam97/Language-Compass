using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UserInterface
{

    class IdiomType
    {
        public List<string> items = new List<string>();
        private string data;

        public IdiomType(string link)
        {
            ReadData(link);
            SetUpIdiom();
        }



        private void ReadData(string link)
        {
            try
            {
                using (StreamReader read = new StreamReader(link))
                {
                    data = read.ReadToEnd();
                }
            }
            catch
            {

            }
        }

        private void SetUpIdiom()
        {
            string[] allIdioms = data.Split('\n');

            foreach (string item in allIdioms)
            {
                items.Add(item);
            }
        }
    }
}

