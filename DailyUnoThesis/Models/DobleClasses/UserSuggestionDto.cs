using System;
using System.Collections.Generic;
using System.Text;

namespace DailyUnoThesis.Models.DobleClasses
{
    public class UserSuggestionDto
    {
        public int IdUser { get; set; }
        public string? Text { get; set; }
        public string? NickName { get; set; }
        public string? Email { get; set; }
        public int ItsMeId { get; set; }





        public bool IsEmpty()
        {
            // Перебираем все свойства объекта
            return this.GetType().GetProperties().All(p =>
            {
                var value = p.GetValue(this);

                // 1. Если null — значит фильтр не задан
                if (value == null) return true;

                // 2. Если это строка — проверяем на пустоту
                if (value is string s) return string.IsNullOrWhiteSpace(s);

                // 3. Если это коллекция (List, Array и т.д.) — проверяем, есть ли в ней элементы
                if (value is System.Collections.IEnumerable enumerable)
                {
                    // Пытаемся получить первый элемент. Если его нет — коллекция пуста.
                    var enumerator = enumerable.GetEnumerator();
                    return !enumerator.MoveNext();
                }

                // 4. Если это число (int, long) — проверяем, не 0 ли это (если 0 значит не выбрано)
                if (value is int i) return i == 0;

                // 5. Для всех остальных типов (например, DateTimeOffset) 
                // проверяем, равен ли он значению по умолчанию
                var defaultValue = p.PropertyType.IsValueType ? Activator.CreateInstance(p.PropertyType) : null;
                return value.Equals(defaultValue);
            });
        }
    }
}
