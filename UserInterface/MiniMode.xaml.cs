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
using System.Windows.Shapes;
using GlobalKeyBoardHook;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for MiniMode.xaml
    /// </summary>
    public partial class MiniMode : Window
    {
        private GlobalKeyBoardHook.LowLevelKeyboardListener lowKeyboard = new GlobalKeyBoardHook.LowLevelKeyboardListener();

        public TranslateMiniMode Translate
        {
            get; set;
        }
        
        public string LanguagePair
        {
            get; set;
        }
       
        public MiniMode()
        {
            InitializeComponent();
            Translate = new TranslateMiniMode();
            lowKeyboard.OnKeyPressed += updateSource;
            Loaded += SetUpValueWhenLoaded;
            LanguagePair = "en|vi";
            this.Topmost = true;
            lowKeyboard.HookKeyboard();
        }

        void updateSource(object sender, KeyPressedArgs e)
        {
            if (((Keyboard.GetKeyStates(Key.LeftAlt) & KeyStates.Down) > 0 || (Keyboard.GetKeyStates(Key.RightAlt) & KeyStates.Down) > 0)
               && e.KeyPressed == Key.C)
            {
                this.rchtb_input.Text = Translate.KeyWord;
                this.rchtb_output.Text = Translate.ContentKeyWord;
            }
        }

        private void SetUpValueWhenLoaded(object sender, RoutedEventArgs e)
        {
            if (Translate.KeyWord != null && Translate.ContentKeyWord != null)
            {
                this.rchtb_input.Text = Translate.KeyWord;
                this.rchtb_output.Text = Translate.ContentKeyWord;
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Visibility =Visibility.Collapsed;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            flyoutOption.IsOpen = true;

        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lowKeyboard.UnHookKeyboard();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (lb_option.SelectedIndex)
            {
                case (0):
                    {
                        LanguagePair = "en|vi";
                        break;
                    }
                case (1):
                    {
                        LanguagePair = "en|en";
                        break;
                    }
                case (2):
                    {
                        LanguagePair = "vi|en";
                        break;
                    }
            }

        }
    }
}
