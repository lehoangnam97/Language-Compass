using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace UserInterface
{
    public class TranslateMiniMode:INotifyPropertyChanged
    {
        private string keyWord, contentKeyWord;
        public event PropertyChangedEventHandler PropertyChanged;
        public string KeyWord
        {
            get { return keyWord; }
            set
            {
                keyWord = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("KeyWord");
            }
        }
        public string ContentKeyWord
        {
            get { return contentKeyWord; }
            set
            {
                contentKeyWord = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("ContentKeyWord");
            }
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
    
}
