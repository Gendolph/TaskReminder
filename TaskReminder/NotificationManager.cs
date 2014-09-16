using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using DevExpress.Xpf.LayoutControl;
using Hardcodet.Wpf.TaskbarNotification;
using Samples;
using TaskReminder.Annotations;
using System.Timers;
using TaskReminder.ViewModels;
using Timer = System.Timers.Timer;


namespace TaskReminder
{


    /// <summary>
    /// Класс управляет всеми оповещениями (Singleton)
    /// </summary>
    class NotificationManager : INotifyPropertyChanged
    {
        private static NotificationManager _me;
        private TaskbarIcon _taskbarIcon;
        private readonly IRepository _repo;
        private readonly Timer _timer = new Timer();
        private readonly ConcurrentQueue<Task> _notificationQueue;
        private readonly AutoResetEvent _newTaskEnqueuedLocker;
        private Thread _worker;
        private AutoResetEvent _notificationCanBeShowedLocker;

        private NotificationViewModel _currentNotificationViewModel;
        private NotificationView _currentNotificationView;

        private readonly List<Task> _taskList;
        private readonly object _allWorkLocker;
        private bool _isNotificationActive;


        /// <summary>
        /// Список задач для напоминания
        /// </summary>
        private List<Task> _tasks;
        public List<Task> Tasks
        {
            get { return _tasks; }
            set
            {
                _tasks = value;
                OnPropertyChanged("Tasks");
            }
        }





        /// <summary>
        /// Constructor
        /// </summary>
        private NotificationManager()
        {
            _taskbarIcon = ((Core)Application.Current).TrayIcon;
            _repo = new Repository();
            _allWorkLocker = new object();
            _isNotificationActive = false;

            // Инициализируем список задач
            var t = System.Threading.Tasks.Task.Run(() =>
                                                    {
                                                        Tasks = new List<Task>(_repo.GetAllTasks());
                                                        Tasks.Sort();
                                                    });

            // Запустим механизм очередности оповещений
            _notificationQueue = new ConcurrentQueue<Task>();

            t.Wait();
            _taskList = new List<Task>(Tasks);

            // Первый запуска таймера
            _timer.Elapsed += EnqueueNotification;
            lock (_allWorkLocker)
                StartTimer();
        }



        /// <summary>
        /// Перерасчитывает и запускает таймер
        /// </summary>
        private void StartTimer()
        {
            // Остановим таймер (если он был запущен)
            _timer.Stop();

            // Расчитаем время до следущего напоминания
            if (_taskList.Count != 0)
            {
                DateTime expectedTime;
                lock (_allWorkLocker)
                    expectedTime = _taskList[0].NextRemindAt;

                var dif = expectedTime - DateTime.Now;
                _timer.Interval = dif < TimeSpan.Zero ? 5000 : dif.TotalMilliseconds;

                // Запустим таймер на расчитанный интервал
                _timer.Start();
            }
        }


        /// <summary>
        /// Обработчик сигнализации таймера (когда надо оповестить пользователя)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="elapsedEventArgs"></param>
        private void EnqueueNotification(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            lock (_allWorkLocker)
            {
                // Для начала проверим (для синхронизации), действительно ли первый элемент
                // в списке задач имеет текущее время напоминания (вдруг, между окончанием таймера
                // и вызовом данного метода произошли изменения в списке, например, изменено
                // время первой задачи в списке и она отправилась в середину списка, а первой стала
                // задача с временем напоминания гораздо позже)
                if (_taskList.Count > 0 && _taskList[0].NextRemindAt < DateTime.Now)
                {
                    // Только здесь вытаскиваем первый элемент из списка оповещени
                    var task = _taskList[0];
                    _taskList.RemoveAt(0);

                    // Если в данный момент (под lock(_allWorkLocker)) оповещение уже отображено,
                    // то помещаем новое оповещение в очередь
                    if (_isNotificationActive)
                    {
                        _notificationQueue.Enqueue(task);
                    }
                    else // Иначе отображаем оповещение
                    {
                        Notify(task);

                        // И помечаем, что оповещение уже отображается
                        _isNotificationActive = true;
                    }
                }

                // Запускаем таймер заного для следующей задачи
                StartTimer();
            }
        }


        /// <summary>
        /// Непосредственно отображает оповещение
        /// </summary>
        /// <param name="task"></param>
        private void Notify(Task task)
        {
            // Отобжараем оповещение
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Подготовим оповещение
                _currentNotificationViewModel = new NotificationViewModel(task);
                _currentNotificationView = new NotificationView(_currentNotificationViewModel);

                // Отобразим оповещение
                //_taskbarIcon.ShowCustomBalloon(
                //                _currentNotificationView,
                //                System.Windows.Controls.Primitives.PopupAnimation.Slide,
                //                5000);

                var bal = new FancyBalloon();
                _taskbarIcon.ShowCustomBalloon(bal, PopupAnimation.Slide, null);
            });
        }




        /// <summary>
        /// Возвращает ссылку на себя (Одиночка)
        /// </summary>
        /// <returns></returns>
        public static NotificationManager GetInstance()
        {
            if (_me == null)
            {
                _me = new NotificationManager();
            }

            return _me;
        }


        /// <summary>
        /// Обрабатывает принятие оповещения
        /// </summary>
        public void AcceptNotification()
        {
            // Для начала закрываем окно оповещения.
            // Затем проверим наличие нового оповещения в очереди и, если оно там есть, то
            // отобразим его
            lock (_allWorkLocker)
            {
                _taskbarIcon.CloseBalloon();

                Task task;
                if (_notificationQueue.TryDequeue(out task))
                {
                    Notify(task);
                }
                else
                {
                    // Удалим объекты вью и вью-модели (для освобождения памяти)
                    _currentNotificationViewModel = null;
                    _currentNotificationView = null;

                    _isNotificationActive = false;
                }
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
