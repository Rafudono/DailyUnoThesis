using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using DailyUnoThesis.Models.MainClasses;

namespace DailyUnoThesis.Models.DobleClasses
{
    public class CategoryService
    {
        private static CategoryService _instance;
        public static CategoryService Instance => _instance ??= new CategoryService();

        // Эта коллекция создается ОДИН раз за всё время жизни приложения
        public ObservableCollection<Category> Categories { get; } = new();
        public ObservableCollection<Category> NavCategories { get; } = new();

        private CategoryService() { }

        // Метод для обновления данных из БД
        public async Task RefreshFromDatabaseAsync()
        {
            // 1. Получаем данные из вашего API или БД
            // Например: var data = await _api.GetCategories();
            Categories.Clear();
            NavCategories.Clear();
            var data = await APIHost.GetInstance().GetCategories();
            Categories.AddRange(data);
            Categories.Insert(0, new Category { Id = 0, Title = "Без категории" });
            //data.AddRange(s => s.IdUpCategory != null);
            //NavCategories.AddRange(data);
            //NavCategories.RemoveAll(s => s.IdUpCategory != null);

            // 2. Очищаем текущую коллекцию
            //Categories.Clear();

            // 3. Заполняем её новыми данными
            foreach (var item in data.Where(s=>s.IdUpCategory == null || s.IdUpCategory == 0))
            {
                
                NavCategories.Add(item);
            }
        }

        private async Task<List<Category>> GetCategoriesFromApi()
        {
            // Здесь ваш код вызова API
            return new List<Category>();
        }
    }
}
