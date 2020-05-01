using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WPFFinalTest3.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _url;
        public string URL { get { return _url; } set { _url = value; StartCommand.RaiseCanExecuteChanged(); } }
        private int _size = -1;
        public int Size { get { return _size; } set { _size = value; OnPropertyChanged("Size"); } }
        private bool _running;
        private bool Running { get { return _running; } set { _running = value; StartCommand.RaiseCanExecuteChanged(); } }
        public DelegateCommand StartCommand { get; private set; }
        private int _counter;
        public int Counter { get { return _counter; } set { _counter = value; OnPropertyChanged("Counter"); } }

        public MainViewModel ()
        {
            DispatcherTimer Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(1);
            Timer.Tick += new EventHandler((o, e) => { Counter++; });

            StartCommand = new DelegateCommand(async() =>
            {
                    Counter = 0;
                    Timer.Start();
                    Size = 0;
                    Running = true;


                    WebRequest webRequest = WebRequest.Create(URL);
                    WebResponse response = await webRequest.GetResponseAsync();
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string text = await reader.ReadToEndAsync();
                        Size = text.Length;
                        Timer.Stop();
                    }

                Running = false;

            }, () =>
            {
                if (Running || URL is null || URL.Length < 4)
                    return false;

                if (URL.Substring(0, 4).ToUpper() != "HTTP")
                    return false;

                return true;
            }) ;
        }

        private void OnPropertyChanged (string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
