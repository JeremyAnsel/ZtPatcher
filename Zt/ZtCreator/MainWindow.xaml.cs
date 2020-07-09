using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using Zt;

namespace ZtCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public string UnmodifiedFile { get; private set; }

        public string ModifiedFile { get; private set; }

        public string Comment { get; set; }

        private void UpdateDataContext()
        {
            this.DataContext = null;
            this.DataContext = this;
        }

        private void RunBusyAction(Action action)
        {
            this.RunBusyAction(dispatcher => action());
        }

        private void RunBusyAction(Action<Action<Action>> action)
        {
            this.BusyIndicator.IsBusy = true;

            Action<Action> dispatcherAction = a =>
            {
                this.Dispatcher.Invoke(a);
            };

            Task.Factory.StartNew(state =>
            {
                var disp = (Action<Action>)state;
                disp(() => { this.BusyIndicator.IsBusy = true; });

                try
                {
                    action(disp);
                }
                catch (Exception ex)
                {
                    disp(() => Xceed.Wpf.Toolkit.MessageBox.Show(this, ex.ToString(), this.Title, MessageBoxButton.OK, MessageBoxImage.Error));
                }

                disp(() => { this.BusyIndicator.IsBusy = false; });
            }, dispatcherAction);
        }

        private string GetOpenFileName()
        {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;

            if (dialog.ShowDialog(this) == true)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        private string GetSaveFileName()
        {
            var dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.DefaultExt = "202";

            if (dialog.ShowDialog(this) == true)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        private void UnmodifiedButton_Click(object sender, RoutedEventArgs e)
        {
            this.UnmodifiedFile = this.GetOpenFileName();
            this.UpdateDataContext();
        }

        private void ModifiedButton_Click(object sender, RoutedEventArgs e)
        {
            this.ModifiedFile = this.GetOpenFileName();
            this.UpdateDataContext();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UnmodifiedFile))
            {
                return;
            }

            if (string.IsNullOrEmpty(this.ModifiedFile))
            {
                return;
            }

            string fileName = this.GetSaveFileName();

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            this.RunBusyAction(disp =>
            {
                try
                {
                    var zt = ZtFile.Create(this.UnmodifiedFile, this.ModifiedFile);

                    zt.Comment = this.Comment;

                    zt.Save(fileName);

                    disp(() => Xceed.Wpf.Toolkit.MessageBox.Show(this, string.Concat(fileName, " created."), this.Title, MessageBoxButton.OK));
                }
                catch (Exception ex)
                {
                    disp(() => Xceed.Wpf.Toolkit.MessageBox.Show(this, ex.ToString(), this.Title, MessageBoxButton.OK, MessageBoxImage.Error));
                }
            });
        }
    }
}
