using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using SpeechLib;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace UserInterface
{
    public class DictionaryType
    {
        private static string dataDict;

        public List<string> suggestKey = new List<string>();

        public Dictionary<string, string> hashDict = new Dictionary<string, string>();

        //Constructor
        public DictionaryType(string datalink)
        {
            ReadData(datalink);
            SetupForDictionary();
        }

        //Read Data from Link
        public void ReadData(string link)
        {
            try
            {
                using (StreamReader read = new StreamReader(link))
                {
                    dataDict = read.ReadToEnd();
                }
            }
            catch (Exception exceptionWhenRead)
            {
                MessageBox.Show(exceptionWhenRead.Message);
            }
        }
        // Set up Word for Dictionary Type (hashDict)b
        public void SetupForDictionary()
        {
            string[] fullWords = dataDict.Split(new string[] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Regex regex = new Regex("(?<=@)[\\S\\D ]+?(?= /)");
            foreach (string oneWord in fullWords)
            {
                try
                {
                    Match match;
                    string name;
                    match = regex.Match(oneWord);
                    if (match.Success == false)
                    {
                        continue;
                    }
                    name = match.Value;
                    suggestKey.Add(name);
                    if (hashDict.ContainsKey(name) == false)
                        hashDict.Add(name, oneWord);
                }
                catch { }
            }

        }

        public void WordOutput(ref RichTextBox sender, string wordInput)
        {

            Paragraph outputParagraph = new Paragraph();
            InlineUIContainer inlineOutput = new InlineUIContainer();
            EnabledFlowDocument flowDockOutput = new EnabledFlowDocument();
            outputParagraph.LineHeight = 30;
            outputParagraph.FontFamily = new FontFamily("Calibri");
            Paragraph.SetFontSize(outputParagraph, 20);

            Button btnVoice = new Button()

            {
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,

                Width = 40,
                Height = 40,
                Background = System.Windows.Media.Brushes.Transparent,
                BorderBrush = System.Windows.Media.Brushes.Transparent,
                Content = new Image()
                {
                    Source = new BitmapImage(new Uri(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + @"\Resources\pronun.png"))
                }

            };


            btnVoice.Click += (SENDER, E) =>
            {
                SpVoice voice = new SpVoice();
                voice.Speak(wordInput, SpeechVoiceSpeakFlags.SVSFDefault);
            };

            inlineOutput.Child = btnVoice;

            outputParagraph.Inlines.Add(new Run(wordInput + " ") { Foreground = Brushes.Red });

            outputParagraph.Inlines.Add(inlineOutput);

            outputParagraph.Inlines.Add("\n");




            string[] parts = hashDict[wordInput].Split('\n');
            foreach (string m in parts)
            {
                string check = "";
                if (m.Length >= 2)
                {
                    check = m.Substring(0, 2);
                }
                switch (check)
                {

                    case "* ": outputParagraph.Inlines.Add(new Run("\n" + m) { Foreground = Brushes.Brown }); break;
                    case "- ": outputParagraph.Inlines.Add(new Run("  " + m.Replace("$","")) { Foreground = Brushes.Black }); break;
                    case "!$": outputParagraph.Inlines.Add(new Run("          " + m.Substring(2).Replace("$", "")) { Foreground = new SolidColorBrush(Color.FromRgb(9, 138, 116)) }); break;
                    case "=$": outputParagraph.Inlines.Add(new Run("       " + m.Substring(2).Replace("$", "")) { Foreground = new SolidColorBrush(Color.FromRgb(152,31,38)) }); break;
                    default:
                        outputParagraph.Inlines.Add(new Run(m.Replace("@", "").Replace("$", "")) { Foreground = new SolidColorBrush(Color.FromRgb(69, 135, 161))}); break;

                }
            }




            flowDockOutput.Blocks.Add(outputParagraph);
            sender.Document = flowDockOutput;
        }
        public string strFirstDefinition(string wordInput)
        {

            string[] parts = hashDict[wordInput].Split('\n');
            foreach (string line in parts)
            {
                string check = "";
                if (line.Length >= 2)
                {
                    check = line.Substring(0, 2);
                }
                if (check == "- ")
                {
                    return line;
                }
            }
            return "";

        }
    }


    public class EnabledFlowDocument : FlowDocument
    {
        protected override bool IsEnabledCore
        {
            get
            {
                return true;
            }
        }
    }
}
