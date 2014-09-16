using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaskReminder.Annotations;

namespace TaskReminder
{
    /// <summary>
    /// Периоды повторов напоминаний
    /// </summary>
    public enum Repeats
    {
        None,
        EachHour,
        EachDay,
        EachWeek,
        EachMonth,
        UserSpecified
    }

    /// <summary>
    /// Определяет задачу для напоминания
    /// </summary>
    public class Task : INotifyPropertyChanged, IComparable<Task>
    {
        private Int32 _id;
        /// <summary>
        /// ID задачи
        /// </summary>
        [Key]
        public Int32 Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        private string _name;
        /// <summary>
        /// Название (заголовок) задачи
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _comment;
        /// <summary>
        /// Комментарий к задаче (доп. данные)
        /// </summary>
        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                OnPropertyChanged("Comment");
            }
        }

        private DateTime _nextRemindAt = new DateTime(3000, 12, 31);
        /// <summary>
        /// Время следующего срабатывания (напоминания)
        /// </summary>
        public DateTime NextRemindAt
        {
            get { return _nextRemindAt; }
            set
            {
                _nextRemindAt = value;
                OnPropertyChanged("NextRemindAt");
            }
        }

        private DateTime _reminderDateTime = new DateTime(3000, 12, 31);
        /// <summary>
        /// Базовое время напоминания (для вычисления повторов)
        /// </summary>
        public DateTime ReminderDateTime
        {
            get { return _reminderDateTime; }
            set
            {
                _reminderDateTime = value;
                OnPropertyChanged("ReminderDateTime");
            }
        }

        private Repeats _repeat = Repeats.None;
        /// <summary>
        /// Условие повтора (период повторения)
        /// </summary>
        public Repeats Repeat
        {
            get { return _repeat; }
            set
            {
                _repeat = value;
                OnPropertyChanged("Repeat");
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



        public int CompareTo(Task other)
        {
            return other == null ? 1 : NextRemindAt.CompareTo(other.NextRemindAt);
        }
    }


    /// <summary>
    /// Интерфейс доступа к репозиторию задач
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Определяет полный список задача для напоминания
        /// </summary>
        /// <returns></returns>
        IEnumerable<Task> GetAllTasks();

        void AddTask(Task task);

        void RemoveTask(Task task);

        void UpdateTask(Task task);
    }
}
