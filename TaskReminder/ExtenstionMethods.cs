using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskReminder
{
    /// <summary>
    /// В данном классе собраны методы расширения класса List_T_
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Вставляет элемент в отсортированный список
        /// </summary>
        /// <typeparam name="T">Тип элементов списка</typeparam>
        /// <param name="list">Отсортированный список по компаратору по умолчанию</param>
        /// <param name="item">Элемент для вставки</param>
        public static void AddToSorted<T>(this List<T> list, T item) where T : IComparable<T>
        {
            if(item == null)
                throw new ArgumentNullException("item");
            
            var retval = list.BinarySearch(item);
            if (retval < 0)
                retval = ~retval;

            list.Insert(retval, item);
        }
    }
}
