using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskReminder
{
    public class Repository : IRepository
    {
        public Repository()
        {
            //// Убьем инициалайзер, т.к. с ним почему-то не работает, если есть БД
            //Database.SetInitializer<DataContext>(null);//new DbInitializer());

            //// Прочитаем все записи из БД
            //using (var dc = new DataContext())
            //{
            //    if (!dc.Database.Exists())
            //        dc.Database.Create();

            //    var data = from d in dc.MyTable select d;
            //    var res = data.ToList();
            //}
        }

        public IEnumerable<Task> GetAllTasks()
        {
            var TaskList = new List<Task>();

            var t = new Task();
            t.Name = "Вот такая небольшая задача";
            t.Comment = "Comment1";
            t.NextRemindAt = DateTime.Now + new TimeSpan(10000);
            TaskList.Add(t);

            t = new Task();
            t.Name = "Task2";
            t.Comment = "Comment2";
            t.NextRemindAt = DateTime.Now;
            t.Repeat = Repeats.EachHour;
            TaskList.Add(t);

            return TaskList;
        }

        public void AddTask(Task task)
        {

        }

        public void RemoveTask(Task task)
        {

        }

        public void UpdateTask(Task task)
        {

        }
    }
}
