using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Xceed.Wpf.Toolkit;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;
using System.Runtime.InteropServices;

using System.Net;
using System.Speech.Recognition;
using System.Windows.Threading;
using System.Threading;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Microsoft.Win32;
using GlobalKeyBoardHook;
using System.Windows.Xps.Packaging;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region declarations
        private MiniMode miniMode;

        //create notification icon
        private static System.Windows.Forms.NotifyIcon notiIcon = new System.Windows.Forms.NotifyIcon();

        //test source for combobox at advance tab
        string[] testSource2;

        private LowLevelKeyboardListener lowKeyboard = new LowLevelKeyboardListener();

        //latest searched word for history function
        string lastSearchedWord = "";

        //Datapath link
        private static string historyPath;
        private static string solutionPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + @"\";

        //language pair : en|vi, vi|en
        private static string languagePair;
        //Current using dictionary 
        private static DictionaryType ev;
        //string capture from hook 
        private static string strCapture;
        #endregion


        public MainWindow()
        {
            InitializeComponent();

            //create a hiding window minimode;
            miniMode = new MiniMode();
 
            //databinding for 
            DataContext = viewModel;
            viewModel.BadgeValue = "Welcome!";


            //create source for advance tab 's combobox
            testSource2 = new string[2];
            testSource2[0] = "English";
            testSource2[1] = "Tiếng Việt";
            cmbx_from.ItemsSource = testSource2;
            cmbx_to.ItemsSource = testSource2;

            //loaded textbox favorite

            listTag.Visibility = Visibility.Collapsed;

            listTag.SelectionChanged += chooseTag;

            //hook keyboard for fast key
            lowKeyboard.OnKeyPressed += TranslateHighlightedText;
            lowKeyboard.OnKeyPressed += TranslateCapturedImage;
            lowKeyboard.HookKeyboard();


            MouseDown += Window_MouseDown;

            //procedire when form load
            Loaded += CheckConnect;
            Loaded += tbfavLoad;
            Loaded += LoadTopic;


        }

        private void CheckConnect(object sender, RoutedEventArgs e)
        {
            CheckForInternetConnection();
        }

       

        private void LoadTopic(object sender, RoutedEventArgs e)
        {
            LoadTabGrammar();
            LoadTabTopic();
            Notification();
            
        }


        //Drag borderless window


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();

        }




        #region Event
        //Change selected combobox
        private void tbfavLoad(object sender, RoutedEventArgs e)
        {
            int i = 0;
            using (var n = new StreamReader(@".\Resources\favorites.txt"))
            {
                string favList = n.ReadToEnd();
                string[] pool = favList.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in pool)
                {
                    i++;

                    if (i % 3 != 0)
                    {
                        tb_fav.Text += "*" + word + "\t";
                    }
                    else tb_fav.Text += "\n" + "    " + word + "\t";

                }
            }
        }


        //Choose Tag for searched word
        private void chooseTag(object sender, SelectionChangedEventArgs e)
        {
            if (listTag.SelectedItem != null)
            {
                string read;
                string tagPath = @".\Resources\Tags\Content\topic" + listTag.SelectedItem.ToString() + ".txt";
                using (StreamReader readTag = new StreamReader(tagPath, true))
                {
                    read = readTag.ReadToEnd();
                };
                if (read.Contains(lastSearchedWord) == false)
                    using (StreamWriter writeTag = new StreamWriter(tagPath, true))
                    {
                        writeTag.WriteLine("-" + lastSearchedWord + "\t");
                    };

                listTag.Visibility = Visibility.Collapsed;
            }

        }

        //change input key in autocompletebox
        private void autotxtInput_EnterKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                SearchWord();
        }

        //press button search
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchWord();
        }
        private void menuitem_tag_Click(object sender, RoutedEventArgs e)
        {
            var template = menuitem_tag.Template;
            var myImage = (Image)template.FindName("img_icontag", menuitem_tag);
            var bitmap = new BitmapImage(new Uri(@".\Resources\summer-15.png", UriKind.Relative));
            myImage.Source = bitmap;
            listTag.Visibility = Visibility.Visible;
            using (var n = new StreamReader(@".\Resources\Tags\TagName.txt"))
            {
                string tagList = n.ReadToEnd();
                listTag.ItemsSource = tagList.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }
            listTag.SelectedItem = null;

        }




        //Load image with null source
        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            image_Word.Source = null;
        }

        //Translate with hook key is Alt+C
        void TranslateHighlightedText(object sender, KeyPressedArgs e)
        {

            if (((Keyboard.GetKeyStates(Key.LeftAlt) & KeyStates.Down) > 0 || (Keyboard.GetKeyStates(Key.RightAlt) & KeyStates.Down) > 0)
                && e.KeyPressed == Key.C && Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
            {
                if (CheckForInternetConnection() == true)
                {
                    strCapture = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString();
                    miniMode.Translate.KeyWord = strCapture;
                    miniMode.Translate.ContentKeyWord = TranslateGoogle(strCapture, miniMode.LanguagePair);
                    miniMode.Show();
                    Clipboard.Clear();
                }
            }
        }


        #endregion



        #region Functions
        public string TranslateGoogle(string text, string lgPair)
        {
            // normalize the culture in case something like en-us was passed 
            // retrieve only en since Google doesn't support sub-locales
            text = text.Replace(" ", "+");
            if (languagePair != "vi|en")
            {
                text = Regex.Replace(text, @"[^0-9a-zA-Z]+", "+");
            }
            string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", text, lgPair);
            // Retrieve Translation with HTTP GET call
            string result = null;
            try
            {
                WebClient web = new WebClient();
                // MUST add a known browser user agent or else response encoding doen't return UTF-8 ?
                web.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
                web.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8,UTF-16,Unicode,ISO-8859-1,ASCII,UTF-32/UCS-4,windows-1258");

                // Make sure we have response encoding to UTF-8
                web.Encoding = System.Text.Encoding.UTF8;
                result = web.DownloadString(url);
            }
            catch { return "Noi dung khong hop le"; }
            result = result.Substring(result.IndexOf("<span title=\"") + "<span title=\"".Length);
            result = result.Substring(result.IndexOf(">") + 1);
            result = result.Substring(0, result.IndexOf("</span>"));
            if (result.Length>=6)
            {
                if (result.Substring(0, 6) == "<html>")
                    return "";
            }
               

            return result.Trim();
        }


        //Translate with hook key is Alt+Q
        void TranslateCapturedImage(object sender, KeyPressedArgs e)
        {
            if (((Keyboard.GetKeyStates(Key.LeftAlt) & KeyStates.Down) > 0 || (Keyboard.GetKeyStates(Key.RightAlt) & KeyStates.Down) > 0) && e.KeyPressed == Key.Q)
            {
                if (CheckForInternetConnection() == true)
                {
                    WindowCapture captureWindow = new WindowCapture();
                    captureWindow.Show();
                }
            }
        }


        //remove hook when window closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lowKeyboard.UnHookKeyboard();
        }

        private void SearchWord(string wordInput)
        {
            if (ev.hashDict.ContainsKey(wordInput))
            {
                //link picture
                image_Word.Source = ImageProcessing.GetBitmapImageFromFile(wordInput);

                //Reload data search result
                Content.Document.Blocks.Clear();
                ev.WordOutput(ref Content, wordInput);
                flyoutOption.Width = gridflyout.ActualWidth;
                flyoutOption.IsOpen = true;
                lastSearchedWord = wordInput;
                menuitem_fav.MouseLeave += menuitem_fav_MouseLeave;
                FindRelatedWord();


            }
            else
            {
                //image_Word.Source = null;
                Content.Document.Blocks.Clear();
                Content.Selection.Text = "[Does not exist]";
                Content.VerticalAlignment = VerticalAlignment.Center;
                Content.HorizontalAlignment = HorizontalAlignment.Center;
                Content.FontSize = 50;
                Content.FontStyle = FontStyles.Italic;
                Content.FontFamily = new FontFamily("Calibri");
                Content.Foreground = new SolidColorBrush(Color.FromRgb(44, 96, 73));

            }
        }
        private void SearchWord()
        {
            if (ev.hashDict.ContainsKey(txtbx_input.Text))
            {
                ////link picture
                image_Word.Source = ImageProcessing.GetBitmapImageFromFile(txtbx_input.Text);

                //Reload data search result
                rchtxtbx_output.Document.Blocks.Clear();
                ev.WordOutput(ref Content, txtbx_input.Text);
                WriteHistory();
                using (var n = new StreamReader(historyPath))
                {
                    string history = n.ReadToEnd();
                    list_history.ItemsSource = history.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Reverse();
                }
                lastSearchedWord = txtbx_input.Text;
                var template = menuitem_fav.Template;
                var myImage = (Image)template.FindName("img_iconfav", menuitem_fav);
                var bitmap = new BitmapImage(new Uri(@".\Resources\essentials-10fade.png", UriKind.Relative));
                myImage.Source = bitmap;
                menuitem_fav.MouseLeave += menuitem_fav_MouseLeave;
                menuitem_tag.MouseLeave += menuitem_fav_MouseLeave;
                flyoutOption.Width = gridflyout.ActualWidth;
                flyoutOption.IsOpen = true;

                FindRelatedWord();
            }
            else
            {
                //image_Word.Source = null;
                Content.Document.Blocks.Clear();
                Content.Selection.Text = "[Does not exist]";
                Content.VerticalAlignment = VerticalAlignment.Center;
                Content.HorizontalAlignment = HorizontalAlignment.Center;
                Content.FontSize = 50;
                Content.FontStyle = FontStyles.Italic;
                Content.FontFamily = new FontFamily("Calibri");
                Content.Foreground = new SolidColorBrush(Color.FromRgb(44, 96, 73));

            }

        }





        private void WriteHistory()
        {
            string read;
            using (StreamReader readHistory = new StreamReader(historyPath, true))
            {
                read = readHistory.ReadToEnd();
            };

            using (StreamWriter writeHistory = new StreamWriter(historyPath, true))
            {
                writeHistory.WriteLine(txtbx_input.Text);
            };

        }
        void FindRelatedWord()
        {
            //Show related word at stackpanel
            stackRelated.Children.Clear();

            int numberOfRelated = 7;


            //in each topic
            foreach (TopicType tp in topics)
            {
                //if topic contains searchword
                if (tp.items.Contains(lastSearchedWord))
                {
                    //create random list
                    Random random = new Random();
                    List<int> indexList = new List<int>();
                    int number = 0;
                    while (indexList.Count <= numberOfRelated)
                    {
                        number = random.Next(0, tp.items.Count - 1);
                        if (!indexList.Contains(number))
                            indexList.Add(number);
                    }

                    //in that random list
                    foreach (int i in indexList)
                    {
                        StackPanel stackContains = new StackPanel();
                        stackContains.Width = stackRelated.ActualHeight - 20;


                        Label lb = new Label()
                        {
                            Content = tp.items[i],
                            FontSize = 13,
                            Foreground = new SolidColorBrush(Color.FromRgb(44, 96, 73))
                        };
                        Image img = new Image()
                        {
                            Source = ImageProcessing.GetBitmapImageFromFile(tp.items[i]),
                            Width = stackContains.Width - 2,
                            Height = stackContains.Width - 2
                        };

                        stackContains.Children.Add(img);
                        stackContains.Children.Add(lb);

                        stackContains.MouseDown += (sender, e)
                            =>
                        {
                            txtbx_input.Text = lb.Content.ToString();
                            SearchWord();
                        };

                        stackRelated.Children.Add(stackContains);
                    }
                }
            }

        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {

            SearchWord();
        }
        private void menuitem_fav_MouseLeave(object sender, MouseEventArgs e)
        {
            var template = menuitem_fav.Template;
            var myImage = (Image)template.FindName("img_iconfav", menuitem_fav);
            var bitmap = new BitmapImage(new Uri(@".\Resources\essentials-10fade.png", UriKind.Relative));
            myImage.Source = bitmap;

        }
        private void menuitem_fav_MouseEnter(object sender, MouseEventArgs e)
        {
            var template = menuitem_fav.Template;
            var myImage = (Image)template.FindName("img_iconfav", menuitem_fav);
            var bitmap = new BitmapImage(new Uri(@".\Resources\essentials-10.png", UriKind.Relative));
            myImage.Source = bitmap;
        }


        private void menuitem_fav_Click(object sender, RoutedEventArgs e)
        {
            var template = menuitem_fav.Template;
            var myImage = (Image)template.FindName("img_iconfav", menuitem_fav);
            var bitmap = new BitmapImage(new Uri(@".\Resources\Checked.png", UriKind.Relative));
            myImage.Source = bitmap;
            menuitem_fav.MouseLeave -= menuitem_fav_MouseLeave;


            string read;
            string favPath = @".\Resources\favorites.txt";
            using (StreamReader readFavorite = new StreamReader(favPath, true))
            {
                read = readFavorite.ReadToEnd();
            };
            if (read.Contains(lastSearchedWord) == false)
                using (StreamWriter writeFavorite = new StreamWriter(favPath, true))
                {
                    writeFavorite.WriteLine(lastSearchedWord);
                };



        }




        #endregion

        private void MetroTabItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            miniMode.Show();

        }

        private void list_history_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list_history.SelectedItem != null)
                SearchWord(list_history.SelectedItem.ToString());
        }
        #region Tab:Setting
        private void cbbx_ChooseDictionary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Selected value is m
            int m = (sender as ComboBox).SelectedIndex;

            //Selected case
            if (m == 0)
            {
                ev = new DictionaryType(solutionPath + @".\Resources\ResourceDictionary\EngtoVie.txt");
                txtbx_input.ItemsSource = ev.suggestKey;
                historyPath = @".\Resources\History\en-viHistory.txt";
                languagePair = "en|vi";
                list_history.ItemsSource = null;

            }

            if (m == 1)
            {
                ev = new DictionaryType(solutionPath + @".\Resources\ResourceDictionary\EngtoEng.txt");
                txtbx_input.ItemsSource = ev.suggestKey;
                languagePair = "en|en";
                historyPath = @".\Resources\History\en-enHistory.txt";
                list_history.ItemsSource = null;
            }
            if (m == 2)
            {
                ev = new DictionaryType(solutionPath + @".\Resources\ResourceDictionary\VietoEng.txt");
                txtbx_input.ItemsSource = ev.suggestKey;
                languagePair = "vi|en";
                historyPath = @".\Resources\History\vi-enHistory.txt";
                list_history.ItemsSource = null;
            }

            using (var n = new StreamReader(historyPath))
            {

                string history = n.ReadToEnd();
                list_history.ItemsSource = history.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Reverse();


            }
        }

        #endregion

        #region Tab: Advance
        private void BtnRead_Click(object sender, RoutedEventArgs e)
        {
            using (System.Speech.Synthesis.SpeechSynthesizer synthesizer = new System.Speech.Synthesis.SpeechSynthesizer())
            {
                synthesizer.SelectVoiceByHints(System.Speech.Synthesis.VoiceGender.Female, System.Speech.Synthesis.VoiceAge.Senior);

                // select audio device
                synthesizer.SetOutputToDefaultAudioDevice();

                // build and speak a prompt
                System.Speech.Synthesis.PromptBuilder builder = new System.Speech.Synthesis.PromptBuilder();
                builder.AppendText(rchtxtbx_output.Selection.Text);
                synthesizer.Speak(builder);
            }
        }
        //Provides the means to access and manage an In-Process Speech recog. engine
        public SpeechRecognitionEngine spEngine = new SpeechRecognitionEngine();


        private enum ButtonSpeechState
        {
            NORMAL,
            RECOGNIZING
        };

        ButtonSpeechState btnSpState = ButtonSpeechState.NORMAL;

        private void BtnSpeech_Click(object sender, RoutedEventArgs e)
        {
            if (btnSpState == ButtonSpeechState.RECOGNIZING)
            {
                var template = btn_voiceregco.Template;
                var myImage = (Image)template.FindName("iconmic", btn_voiceregco);
                var bitmap = new BitmapImage(new Uri(@".\Resources\21fade.png", UriKind.Relative));
                myImage.Source = bitmap;
                btnSpState = ButtonSpeechState.NORMAL;
                spEngine.RecognizeAsyncStop();
            }
            else
            {
                btnSpState = ButtonSpeechState.RECOGNIZING;
                var template = btn_voiceregco.Template;
                var myImage = (Image)template.FindName("iconmic", btn_voiceregco);
                var bitmap = new BitmapImage(new Uri(@".\Resources\21.png", UriKind.Relative));
                myImage.Source = bitmap;
                try
                {

                    spEngine.RequestRecognizerUpdate();
                    Grammar gr = new DictationGrammar();
                    spEngine.LoadGrammar(gr);
                    spEngine.SpeechRecognized += SpEngine_SpeechRecognized;
                    spEngine.SetInputToDefaultAudioDevice();
                    spEngine.RecognizeAsync(RecognizeMode.Multiple);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void SpEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {

            rchtxtbx_input.Selection.Text += e.Result.Text.ToString();
        }
        private void rchtxtbx_input_TextChanged(object sender, TextChangedEventArgs e)
        {
            rchtxtbx_output.Selection.Text = TranslateGoogle(rchtxtbx_input.Selection.Text, "en|vi");
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);


            // Đăng ký stratup cùng Windows
            registryKey.SetValue("UserInterface", Directory.GetCurrentDirectory() + "\\UserInterface.exe");

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            registryKey.DeleteValue("UserInterface");
        }


        #endregion

        //game ôn tập --------------------------------------------------------------------
        #region Tab: Game
        #region Declarations
        //default words list from history or topics
        List<string> listDefaultWords = new List<string>();

        //words list remix from default words list
        List<string> listRanDomWords = new List<string>();

        //list forgottenWord
        List<string> listForgottenWord = new List<string>();

        //char list remix from choosen word 
        List<char> listMixChar = new List<char>();

        //Timer and Count down integer for each round
        DispatcherTimer timerCount = new DispatcherTimer();
        int intCount = 15;

        //Timer and Count down integer for each Endding round effect
        DispatcherTimer timeEffect = new DispatcherTimer();
        int intEffect = 0;

        //choosen word index from listRandomWords
        int indexChoosenWord = 0;


        #endregion

        //refresh button state and all stack, textbox, score
        private void RefreshAllGame()
        {
            //reset foreground color for each button
            foreach (Button btn in stackbtnTopicGame.Children)
            {
                btn.Background = new SolidColorBrush(Color.FromRgb(44, 62, 80));
            }

            gridGameMain.Visibility = Visibility.Visible;
            stackAnswer.Children.Clear();
            listDefaultWords.Clear();
            listRanDomWords.Clear();
            listForgottenWord.Clear();
        }


        //chứa topic chủ đề
        #region Tab: Topic

        TopicType[] topics = new TopicType[9];

        private void LoadTabTopic()
        {
            //read from topic txt
            topics[0] = new TopicType(@".\Resources\Tags\Content\topicBodyAndAppearance.txt");
            topics[1] = new TopicType(@".\Resources\Tags\Content\topicBusinessAndWork.txt");
            topics[2] = new TopicType(@".\Resources\Tags\Content\topicCultureAndSociety.txt");
            topics[3] = new TopicType(@".\Resources\Tags\Content\topicEducation.txt");
            topics[4] = new TopicType(@".\Resources\Tags\Content\topicEntertainment.txt");
            topics[5] = new TopicType(@".\Resources\Tags\Content\topicFoodAndDrink.txt");
            topics[6] = new TopicType(@".\Resources\Tags\Content\topicRelationship.txt");
            topics[7] = new TopicType(@".\Resources\Tags\Content\topicScienceAndTechnology.txt");
            topics[8] = new TopicType(@".\Resources\Tags\Content\topicSport.txt");

            //Add name to list of topic
            //foreach (TopicType tp in topics)
            //{
            //    string name = tp.name;
            //    lbxTopicNames.Items.Add(name);
            //}

            //lbxTopicNames.SelectedIndex = 0;

        }





        //private void LbxTopicNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    lbxTopicItems.Items.Clear();
        //    foreach (string word in topics[lbxTopicNames.SelectedIndex].items)
        //    {
        //        lbxTopicItems.Items.Add(word);
        //    }
        //}
        //private void LbxTopicItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    TabAll.SelectedIndex = 0;

        //    if (lbxTopicItems.SelectedIndex < 0) return;

        //    autotxtInput.Text = lbxTopicItems.SelectedValue.ToString();
        //    SearchWord();
        //}



        #endregion
        private void btnGameTopic_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            RefreshAllGame();
            //selected button difference foreground from the others
            btn.Background = new SolidColorBrush(Color.FromRgb(230, 126, 34));

            //get index of topic from the last characters of button topic 's name
            int index = Int32.Parse((btn.Name[btn.Name.Length - 1]).ToString());

            //create the default list words with corresponding index 
            listDefaultWords = topics[index].items;
        }



        private void btnGameStart_Click(object sender, RoutedEventArgs e)
        {
            //Check if user had choosed topic
            int check = 0;

            foreach (Button btn in stackbtnTopicGame.Children)
            {
                if (btn.Background.ToString() == "#FFE67E22")
                {
                    check = 1;
                    break;
                }
            }
            if (check == 0)
            {
                System.Windows.MessageBox.Show("You must book a topic first");
                return;
            }
            gridGameMain.Visibility = Visibility.Hidden;
            SetupGamePlay();
        }


        private void btnHighScore_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("High score : " + lbHighScore.Content.ToString());
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            ReFreshAllChar(listRanDomWords[indexChoosenWord]);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            EndRound();
            StartEffectAndEndRound();

        }


        private void btnEndGame_Click(object sender, RoutedEventArgs e)
        {
            EndGame();
        }

        private void btnBackGame_Click(object sender, RoutedEventArgs e)
        {
            gridGameOver.Visibility = Visibility.Hidden;
            RefreshAllGame();
            gridGameMain.Visibility = Visibility.Visible;
        }

        private void lboxForgottenWords_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TabAll.SelectedIndex = 0;

            if (lboxForgottenWords.SelectedIndex < 0) return;

            txtbx_input.Text = lboxForgottenWords.SelectedValue.ToString();
            SearchWord();
        }


        private void TimeCount_Tick(object sender, EventArgs e)
        {
            lbTimer.Content = intCount.ToString();

            intCount -= 1;

            if (intCount == 0)
            {
                timerCount.Stop();
                EndRound();
                StartEffectAndEndRound();
            }
        }

        //Set up for a whole new game with choosen topic
        private void SetupGamePlay()
        {
            //setup timer
            timerCount.Stop();
            timerCount.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            timerCount.Tick -= TimeCount_Tick;
            timerCount.Tick += TimeCount_Tick;

            //Setup Point
            lbScore.Content = "0";
            //Setup highscore
            GetHighScore();

            if (listDefaultWords.Count == 0) return;

            // mix list items string for game 
            Random random = new Random();

            int listCount = listDefaultWords.Count;

            while (listRanDomWords.Count < listCount)
            {
                int indexNext = random.Next(0, listDefaultWords.Count - 1);
                string wordNext = listDefaultWords[indexNext];
                listDefaultWords.Remove(wordNext);
                listRanDomWords.Add(wordNext);
            }

            NextRound();

        }

        private void NextRound()
        {
            //increase index
            if (indexChoosenWord == listRanDomWords.Count - 1)
            {
                EndGame();
            }

            if (indexChoosenWord < listRanDomWords.Count - 1)
                indexChoosenWord++;


            //reset timer
            intCount = 15;

            timerCount.Stop();
            timerCount.Start();

            //Start new game round
            GameRound(listRanDomWords[indexChoosenWord]);
        }

        //Start new game round
        private void GameRound(string choosenWord)
        {

            //Set questions : 
            txtQuestion.Text = "Question : Find the correct word with definition : " + Environment.NewLine;
            txtQuestion.Text += ev.strFirstDefinition(choosenWord).Replace("$", "");

            Random random = new Random();


            listMixChar.Clear();
            List<char> listAnswer = new List<char>();

            //Add char to list answer
            foreach (char charItem in choosenWord)
            {
                listAnswer.Add(charItem);
            }

            //Add random char to list mixchar
            while (listMixChar.Count < choosenWord.Length)
            {
                int number = random.Next(0, listAnswer.Count - 1);
                char choosenChar = listAnswer[number];
                listAnswer.Remove(choosenChar);
                listMixChar.Add(choosenChar);
            }

            ReFreshAllChar(choosenWord);
        }


        //Refresh All char for a Round
        private void ReFreshAllChar(string choosenWord)
        {
            //Clear all stack 's buttons
            stackAnswer.Children.Clear();
            stackMix.Children.Clear();

            //Get the edge of a square-buton
            int edge = Convert.ToInt32(stackAnswer.ActualWidth / choosenWord.Length);
            if (edge > Convert.ToInt32(stackAnswer.ActualHeight))
                edge = Convert.ToInt32(stackAnswer.ActualHeight);

            //add items to stackMix from listMixChar
            foreach (char charItem in listMixChar)
            {
                //create button with its content is Character from list mix char
                Button btnMix = new Button()
                {
                    Content = charItem.ToString(),
                    Width = edge,
                    Height = edge,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    Background = new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontWeight = FontWeights.ExtraBold,
                    FontSize = 25,
                    FontStretch = FontStretches.ExtraCondensed
                };

                //Add button mix to stack answer when click (as known as : delete from stack mix char and add a new button to stack answer)
                btnMix.Click += (sender, e) =>
                {

                    Button btnAnswer = new Button
                    {
                        Content = charItem.ToString(),
                        Width = edge,
                        Height = edge,
                        BorderBrush = new SolidColorBrush(Colors.Black),
                        Background = new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                        Foreground = new SolidColorBrush(Colors.Black),
                        FontWeight = FontWeights.ExtraBold,
                        FontSize = 25,
                        FontStretch = FontStretches.ExtraCondensed
                    };


                    stackAnswer.Children.Add(btnAnswer);

                    stackMix.Children.Remove(btnMix);
                };

                //add button to stack mix
                stackMix.Children.Add(btnMix);
            }
        }


        private void EndRound()
        {
            int checkRight = 1;

            //Create list to check character from player answer
            List<char> charCheck = new List<char>();

            foreach (Button btn in stackAnswer.Children)
            {
                charCheck.Add(Convert.ToChar(btn.Content.ToString()));
            }

            if (indexChoosenWord >= listRanDomWords.Count - 1) return;
            //if is missing chacter
            if (charCheck.Count != listRanDomWords[indexChoosenWord].Length)
            {
                checkRight = 0;
            }
            else
            {
                //if the answer is not correct
                for (int i = 0; i < listRanDomWords[indexChoosenWord].Length; i++)
                {
                    if (charCheck[i] != listRanDomWords[indexChoosenWord][i])
                    {
                        checkRight = 0;
                        break;
                    }
                }
            }

            if (checkRight == 1)
            {
                //Effect on the correct answer -> make a green background  for each square
                foreach (Button btn in stackAnswer.Children)
                {
                    btn.Foreground = new SolidColorBrush(Color.FromRgb(46, 204, 113));
                }

                //Increase Score
                lbScore.Content = Int32.Parse(lbScore.Content.ToString()) + 1;

                //reset highscore
                if (Int32.Parse(lbHighScore.Content.ToString()) < Int32.Parse(lbScore.Content.ToString()))
                {
                    lbHighScore.Content = lbScore.Content;
                }
                //delay a while to show 

                foreach (Button btn in stackAnswer.Children)
                {
                    btn.Background = new SolidColorBrush(Colors.Green);
                }

            }
            else
            {
                listForgottenWord.Add(listRanDomWords[indexChoosenWord]);

                //Effect on the correct answer -> make a red background  for each square
                //show correct answer
                int edge = Convert.ToInt32(stackAnswer.ActualWidth / listRanDomWords[indexChoosenWord].Length);
                if (edge > Convert.ToInt32(stackAnswer.ActualHeight))
                    edge = Convert.ToInt32(stackAnswer.ActualHeight);

                stackAnswer.Children.Clear();
                stackMix.Children.Clear();

                foreach (char charItem in listRanDomWords[indexChoosenWord])
                {
                    Button btnAns = new Button()
                    {
                        Content = charItem.ToString(),
                        Width = edge,
                        Height = edge,
                        BorderBrush = new SolidColorBrush(Colors.Black),
                        Background = new SolidColorBrush(Color.FromRgb(192, 57, 43)),
                        Foreground = new SolidColorBrush(Colors.White),
                        FontWeight = FontWeights.ExtraBold
                    };
                    stackAnswer.Children.Add(btnAns);
                }

            }
        }


        private void StartEffectAndEndRound()
        {
            intEffect = 0;
            timeEffect.Stop();
            timeEffect.Start();
            timeEffect.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timeEffect.Tick -= TimeEffect_Tick;
            timeEffect.Tick += TimeEffect_Tick;
        }

        private void TimeEffect_Tick(object sender, EventArgs e)
        {
            intEffect++;
            if (intEffect % 3 == 0)
            {
                timeEffect.Stop();

                NextRound();
            }
        }


        private void EndGame()
        {

            gridGameOver.Visibility = Visibility.Visible;
            lboxForgottenWords.Items.Clear();

            txtGameOverInfo.Text = "";
            txtGameOverInfo.Text += "High score : " + lbHighScore.Content.ToString() + Environment.NewLine;
            txtGameOverInfo.Text += "Score : " + lbScore.Content.ToString();
            //Write highScore
            System.IO.File.WriteAllText(@".\Resources\HighScore.txt", lbHighScore.Content.ToString());
            foreach (string m in listForgottenWord)
            {
                lboxForgottenWords.Items.Add(m);
            }

        }




        private void GetHighScore()
        {
            string data = "";
            try
            {
                using (StreamReader read = new StreamReader(@".\Resources\HighScore.txt"))
                {
                    data = read.ReadToEnd();
                }
            }
            catch (Exception exceptionWhenRead)
            {
                System.Windows.MessageBox.Show(exceptionWhenRead.Message);

            }


            lbHighScore.Content = data;
        }





        #endregion
        #region Tab: Grammar


        /// <summary>
        /// Tab Idioms in grammars
        /// </summary>

        private IdiomType listIdioms = new IdiomType(solutionPath + @".\Resources\Grammar\Idioms.txt");


        private void LoadTabGrammar()
        {
            //load Tenses
            XpsDocument xpsTenses = new XpsDocument(solutionPath + @".\Resources\Grammar\Tenses.xps", FileAccess.Read);
            docTense.Document = xpsTenses.GetFixedDocumentSequence();
            docTense.FitToWidth();


            //load Words
            XpsDocument xpsWords = new XpsDocument(solutionPath + @".\Resources\Grammar\Words.xps", FileAccess.Read);
            docWords.Document = xpsWords.GetFixedDocumentSequence();
            docWords.FitToWidth();


            //load Phrases
            XpsDocument xpsPhrases = new XpsDocument(solutionPath + @".\Resources\Grammar\Phrases.xps", FileAccess.Read);
            docPhrases.Document = xpsPhrases.GetFixedDocumentSequence();
            docPhrases.FitToWidth();


            //load Clauses
            XpsDocument xpsClauses = new XpsDocument(solutionPath + @".\Resources\Grammar\Clauses.xps", FileAccess.Read);
            docClauses.Document = xpsClauses.GetFixedDocumentSequence();
            docClauses.FitToWidth();


            //load Sentences
            XpsDocument xpsSentences = new XpsDocument(solutionPath + @".\Resources\Grammar\Sentences.xps", FileAccess.Read);
            docSentences.Document = xpsSentences.GetFixedDocumentSequence();
            docSentences.FitToWidth();


            //load IrregularVerbs
            XpsDocument xpsIrregularVerbs = new XpsDocument(solutionPath + @".\Resources\Grammar\IrregularVerbs.xps", FileAccess.Read);
            docIrregularVerbs.Document = xpsIrregularVerbs.GetFixedDocumentSequence();
            docIrregularVerbs.FitToWidth();

            //load Topics
            XpsDocument xpsTopics = new XpsDocument(solutionPath + @".\Resources\Grammar\Topics.xps", FileAccess.Read);
            docTopics.Document = xpsTopics.GetFixedDocumentSequence();
            docTopics.FitToWidth();


            //load Popular
            XpsDocument xpsPopular = new XpsDocument(solutionPath + @".\Resources\Grammar\Popular.xps", FileAccess.Read);
            docPopular.Document = xpsPopular.GetFixedDocumentSequence();
            docPopular.FitToWidth();


            //Load Idioms
            foreach (string m in listIdioms.items)
            {
                lboxIdioms.Items.Add(m);
            }
        }

        private void lboxIdioms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string selected = lboxIdioms.SelectedValue.ToString();
            selected = selected.Substring(selected.IndexOf(" ") + 1, selected.IndexOf(":") - selected.IndexOf(" ") - 1);

            string[] words = selected.Split(' ');

            WindowSelectedIdioms winIdiom = new WindowSelectedIdioms();
            winIdiom.SetUpItems(words);

            string winIdiomselected = "";


            if (winIdiom.ShowDialog().Value)
            {
                winIdiomselected = winIdiom.selectedItem;
            }

            TabAll.SelectedIndex = 0;
            txtbx_input.Text = winIdiomselected.ToLower();
            SearchWord();

        }
        #endregion
        //title bar
        public static ViewModel viewModel = new ViewModel();

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            badge_noti.Badge = null;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
                this.WindowState = System.Windows.WindowState.Maximized;
            else this.WindowState = System.Windows.WindowState.Normal;
            flyoutOption.Width = gridflyout.ActualWidth;
        }



        #region Notification Icon


        //add this line to activated 
        private void Notification()
        {
            notiIcon.Icon = new System.Drawing.Icon(@".\Resources\icono.ico");
            notiIcon.Visible = true;

            notiIcon.ShowBalloonTip(1500, "Compass Language", "Welcome! Thanks for subscribing", System.Windows.Forms.ToolTipIcon.Info);

            System.Windows.Forms.ContextMenu notifyContextMenu = new System.Windows.Forms.ContextMenu();
            notifyContextMenu.MenuItems.Add("Show", new EventHandler(ShowNotify));
            notifyContextMenu.MenuItems.Add("Exit", new EventHandler(ExitNotify));

            notiIcon.ContextMenu = notifyContextMenu;
        }
        private void ShowNotify(object sender, EventArgs e)
        {
            this.Show();
        }

        private void ExitNotify(object sender, EventArgs e)
        {
            notiIcon.ShowBalloonTip(1500, "Compass Language", "Good bye my friend", System.Windows.Forms.ToolTipIcon.Info);

            this.Close();
        }



        #endregion

        //Setting


        private void btn_resethighscore_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter resetHighscore = new StreamWriter(@".\Resources\HighScore.txt", false))
            {
                resetHighscore.WriteLine(0);
            };

        }

        private void cbbi_DispEng_Selected(object sender, RoutedEventArgs e)
        {
            head_main.Header = "Main";
            head_advance.Header = "Advance";
            head_grammar.Header = "Grammar";
            head_fav.Header = "Favorite";
            head_game.Header = "Game";
            head_mini.Header = "Mini";
            head_setting.Header = "Setting";
            main_history.Content = "History:";
            advance_from.Content = "         From:";
            advance_to.Content = "To:        ";
            gra_clause.Header = "Clause";
            gra_idiom.Header = "Idiom:";
            gra_irr.Header = "Irregular Verb";
            gra_phrase.Header = "Phrase";
            gra_popular.Header = "Popular";
            gra_sentence.Header = "Sentence";
            gra_topic.Header = "Topic";
            gra_word.Header = "Word";
            btnEndGame.Content = "End Game";
            btnGameStart.Content = "Start";
            btnHighScore.Content = "Highscore";
            btnNext.Content = "Next";
            btnRefresh.Content = "Refresh";
            btn_resethighscore.Content = "Reset Highscore";
            setting_disp.Content = "Display Language:";
            setting_startup.Content = "Start with Window";
            setting_navigate.Content = "Dictionary:";
            about_des.Text = "Designer:";
            about_pro.Text = "Programmer:";
            var template = menuitem_fav.Template;
            var label = (Label)template.FindName("main_asfav", menuitem_fav);
            label.Content = "Mark as favorite";
            var template2 = menuitem_tag.Template;
            var label2 = (Label)template2.FindName("main_adtag", menuitem_tag);
            label2.Content = "Add Tag";
            viewModel.BadgeValue = "Displayed in English";
            mini_inro.Content = "Double Click on the tab to open Mini mode, experience fast lookup on every other program's text and image";
            mini_inro2.Content = "Press Alt + Q to translate from extracted image  \n Press Alt + C t translate text copied to clipboard";



        }


        //Check connection
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                notiIcon.Icon = new System.Drawing.Icon(@".\Resources\icono.ico");
                notiIcon.Visible = true;
                viewModel.BadgeValue = "Internet connection lost";
                notiIcon.ShowBalloonTip(1500, "Compass Language", "Internet connection lost", System.Windows.Forms.ToolTipIcon.Warning);
                return false;
            }
        }
        private void cbbi_DispVie_Selected(object sender, RoutedEventArgs e)
        {
            head_main.Header = "Màn hình chính";
            head_advance.Header = "Nâng cao";
            head_grammar.Header = "Ngữ pháp";
            head_fav.Header = "Yêu thích";
            head_game.Header = "Trò chơi";
            head_mini.Header = "Dạng thu nhỏ";
            head_setting.Header = "Cài đặt";
            main_history.Content = "Lịch sử:";
            advance_from.Content = "         Từ:";
            advance_to.Content = "Đến:        ";
            gra_clause.Header = "Mệnh đề";
            gra_idiom.Header = "Thành ngữ:";
            gra_irr.Header = "Bất qui tắc";
            gra_phrase.Header = "Cụm từ";
            gra_popular.Header = "Thông dụng";
            gra_sentence.Header = "Câu";
            gra_topic.Header = "Chủ đề";
            gra_word.Header = "Từ";
            btnEndGame.Content = "Kết thúc";
            btnGameStart.Content = "Bắt đầu";
            btnHighScore.Content = "Điểm cao";
            btnNext.Content = "Tiếp theo";
            btnRefresh.Content = "Làm lại";
            btn_resethighscore.Content = "Xóa bảng thành tích";
            setting_disp.Content = "Ngôn ngữ hiển thị:";
            setting_startup.Content = "Khởi động cùng máy";
            setting_navigate.Content = "Điều hướng:";
            about_des.Text = "Người thiết kế:";
            about_pro.Text = "Lập trình viên:";
            tb_forgottenword.Text = "Những từ sai:";
            mini_inro.Content = "Bạn nhấn đôi vào tiêu mục tab để mở chế độ mini, phải có kết nối internet để sử dụng";
            var template = menuitem_fav.Template;
            var label = (Label)template.FindName("main_asfav", menuitem_fav);
            label.Content = "Đánh dấu thích";
            var template2 = menuitem_tag.Template;
            var label2 = (Label)template2.FindName("main_adtag", menuitem_tag);
            label2.Content = "Gắn thẻ";
            mini_inro.Content = "Nhấp đôi chuột để mở chế độ thu nhỏ, trải nghiệm những tính năng tiện lợi và hiệu quả";
            mini_inro2.Content = "Nhấn Alt+Q để dịch từ phần màn hình đã chụp \n Nhấn Alt+C để dịch từ phần văn bản đã chép vào bộ đệm";
            viewModel.BadgeValue = "Hiển thị bằng Tiếng Việt";

        }


        private void rchtxtbx_input_KeyDown(object sender, TextChangedEventArgs e)
        {
            rchtxtbx_output.Selection.Text = TranslateGoogle(rchtxtbx_input.Selection.Text, "en|vi");
        }
    }
    public class ViewModel : INotifyPropertyChanged
    {
        private string _badgeValue;
        public string BadgeValue
        {
            get { return _badgeValue; }
            set { _badgeValue = value; NotifyPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
