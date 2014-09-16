using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
using TaskReminder.ViewModels;

namespace TaskReminder
{
    
    public partial class NotificationView
    {
        public NotificationView(NotificationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            var mng = NotificationManager.GetInstance();
            mng.AcceptNotification();
        }
    }
}
