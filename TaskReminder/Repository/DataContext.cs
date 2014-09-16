using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TaskReminder
{
    public class DataContext : DbContext
    {
        public IDbSet<Task> MyTable { get; set; }
    }


    /// <summary>
    /// DataBase initializer (checks for database existing and compliant)
    /// </summary>
    public class DbInitializer : IDatabaseInitializer<DataContext>
    {
        public void InitializeDatabase(DataContext context)
        {
            if (!context.Database.Exists())
            {
                context.Database.Create();
            }
            else if (context.Database.CompatibleWithModel(true))
            {
                if (MessageBox.Show("TaskReminder: Ошибка инициализации БД",
                                    "БД имеет неверную модель.\nНажмите 'Да', чтобы удалить текущую БД и создать заново.\nНажмите 'Нет', чтобы выйти из программы",
                                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    // Yes button clicked
                    context.Database.Delete();
                    context.Database.Create();
                }
                else
                {
                    // No button clicked
                    Environment.Exit(1);
                }
            }
        }
    }
}
