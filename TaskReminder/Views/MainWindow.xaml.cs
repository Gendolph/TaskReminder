using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using TaskReminder.ViewModels;


namespace TaskReminder
{
    /// <summary>
    /// Interaction logic for DXWindow1.xaml
    /// </summary>
    public partial class MainWindow : DXWindow
    {

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            // Укажем контекст для биндингов
            this.DataContext = viewModel;
            this.ShowInTaskbar = false;
        }





        private void btn_TestPopup_Click(object sender, RoutedEventArgs e)
        {
            var TrayIcon = ((Core)Application.Current).TrayIcon;
            var viewModel = new NotificationViewModel(((MainWindowViewModel)DataContext).TaskList[0]);
            var baloon = new NotificationView(viewModel);
            //TrayIcon.ShowBalloonTip("MyBaloon", "Yam", Hardcodet.Wpf.TaskbarNotification.BalloonIcon);
            TrayIcon.ShowCustomBalloon(baloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, null);
        }
    }
}
