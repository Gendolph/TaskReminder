using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace TaskReminder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class Core : Application
    {
        public TaskbarIcon TrayIcon;

        private NotificationManager _notificationManager;

        private Repository _repo;
        private MainWindowViewModel _mainViewModel;
        private MainWindow _mainWindow;


        /// <summary>
        /// Обработчик события запуска приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_Startup(object sender, StartupEventArgs e)
        {
            // Инициализируем иконку в трее
            TrayIcon = (TaskbarIcon)FindResource("TrayIcon");

            _mainViewModel = new MainWindowViewModel();
            _mainWindow = new MainWindow(_mainViewModel);

            this.MainWindow = _mainWindow;
            TrayIcon.TrayMouseDoubleClick += TrayMouseDoubleClickEventHandler;
            //_mainWindow.Show();

            NotificationManager.GetInstance();
        }


        private void TrayMouseDoubleClickEventHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_mainWindow.Visibility == Visibility.Collapsed)
            {
                _mainWindow.Topmost = true;
                _mainWindow.Show();
                _mainWindow.Topmost = false;
            }
            else
                _mainWindow.Visibility = Visibility.Collapsed;
        }
    }
}
