using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Zt;

namespace ZtPatcher
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public ZtFile PatchFile { get; private set; }

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

        private void OpenPatchButton_Click(object sender, RoutedEventArgs e)
        {
            string filename = this.GetOpenFileName();

            if (string.IsNullOrEmpty(filename))
            {
                return;
            }

            this.RunBusyAction(disp =>
            {
                try
                {
                    this.PatchFile = ZtFile.FromFile(filename);

                    disp(() => this.UpdateDataContext());
                }
                catch (Exception ex)
                {
                    disp(() => Xceed.Wpf.Toolkit.MessageBox.Show(this, ex.ToString(), this.Title, MessageBoxButton.OK, MessageBoxImage.Error));
                }
            });
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.PatchFile == null)
            {
                return;
            }

            string targetFile = this.GetOpenFileName();

            if (string.IsNullOrEmpty(targetFile))
            {
                return;
            }

            this.RunBusyAction(disp =>
            {
                try
                {
                    this.PatchFile.Apply(targetFile);

                    disp(() => Xceed.Wpf.Toolkit.MessageBox.Show(this, "Patch applied.", this.Title, MessageBoxButton.OK));
                }
                catch (Exception ex)
                {
                    disp(() => Xceed.Wpf.Toolkit.MessageBox.Show(this, ex.ToString(), this.Title, MessageBoxButton.OK, MessageBoxImage.Error));
                }
            });
        }
    }
}
