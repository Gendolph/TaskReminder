using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Data.Utils.ServiceModel.Native;
using TaskReminder.Annotations;

namespace TaskReminder
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // Управляющий напоминаниями
        private NotificationManager _notificationManager;

        /// <summary>
        /// Список задач для напоминания
        /// </summary>
        private ObservableCollection<Task> _taskList;
        public ObservableCollection<Task> TaskList
        {
            get { return _taskList; }
            set
            {
                _taskList = value;
                OnPropertyChanged("TaskList");
            }
        }


        /// <summary>
        /// Конструктор
        /// </summary>
        public MainWindowViewModel()
        {
            _notificationManager = NotificationManager.GetInstance();
            _notificationManager.PropertyChanged += NotificationManagerOnPropertyChanged;
            TaskList = new ObservableCollection<Task>(_notificationManager.Tasks);
        }


        /// <summary>
        /// Обработчик событий менеджера напоминаний
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="propertyChangedEventArgs"></param>
        private void NotificationManagerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "TaskList")
            {
                // В данном случае копируем ссылку на список задач себе для отображения
                TaskList = new ObservableCollection<Task>(_notificationManager.Tasks);
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
