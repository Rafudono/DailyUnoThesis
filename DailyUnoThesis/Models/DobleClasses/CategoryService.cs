using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using DailyUnoThesis.Models.MainClasses;

namespace DailyUnoThesis.Models.DobleClasses
{
    public class CategoryService
    {
        private static CategoryService _instance;
        public static CategoryService Instance => _instance ??= new CategoryService();

        public ObservableCollection<IconItem> AvailableIcons = new ObservableCollection<IconItem>
        {
            new IconItem { Glyph = "\uE8B7", Value = "E8B7" }, // Папка
            new IconItem { Glyph = "\uE814", Value = "E814" }, // Работа
            new IconItem { Glyph = "\uE80F", Value = "E80F" }, // Дом
            new IconItem { Glyph = "\uE734", Value = "E734" }, // Звезда
            new IconItem { Glyph = "\uE82D", Value = "E82D" }, // Книга
            new IconItem { Glyph = "\uE787", Value = "E787" }, // Календарь
            new IconItem { Glyph = "\uE70E", Value = "E70E" }, // Чашка
            new IconItem { Glyph = "\uE187", Value = "E187" }, // Деньги
            new IconItem { Glyph = "\uE716", Value = "E716" }  // Люди
        };
        // Эта коллекция создается ОДИН раз за всё время жизни приложения
        public ObservableCollection<Category> Categories { get; } = new();
        public ObservableCollection<Category> NavCategories { get; } = new();
        public ObservableCollection<Category> FilterCategories { get; } = new();
        public ObservableCollection<Category> Projects { get; } = new();
        public ObservableCollection<Category> AssignmentOfProjects { get; } = new();
        public ObservableCollection<Category> ArchivalProjects { get; } = new();


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
            NavCategories.AddRange(data.Where(s => s.IdUpCategory == null || s.IdUpCategory == 0));
            FilterCategories.AddRange(data);



            //var dataProj = await APIHost.GetInstance().GetProject();
            //Projects.AddRange(dataProj);


        }

        public async Task RefreshFromProjectsDatabaseAsync()
        {
            Projects.Clear();
            ArchivalProjects.Clear();
            AssignmentOfProjects.Clear();
              var dataProj = await APIHost.GetInstance().GetProject();
            AssignmentOfProjects.AddRange(dataProj.Where(s=>s.IdBigBoss == AuthorizedUser.GetInstance().AuthUser.Id));
            Projects.AddRange(dataProj.Where(s => (s.IdUpCategory == null || s.IdUpCategory == 0) && s.ProgressstatesId == (int)ProgressStateEnum.InProgress));
            ArchivalProjects.AddRange(dataProj.Where(s => (s.IdUpCategory == null || s.IdUpCategory == 0) && s.ProgressstatesId != (int)ProgressStateEnum.InProgress));

        }

        public async Task FilterArchivalProgressStates(int id)
        {
            ArchivalProjects.Clear();
            if (id <= 0)
            {
                ArchivalProjects.AddRange(AssignmentOfProjects.Where(s => (s.IdUpCategory == null || s.IdUpCategory == 0) && s.ProgressstatesId != (int)ProgressStateEnum.InProgress));
            }
            else
            {
                ArchivalProjects.AddRange(AssignmentOfProjects.Where(s => (s.IdUpCategory == null || s.IdUpCategory == 0) && s.ProgressstatesId == id));
            }
        }

        private async Task<List<Category>> GetCategoriesFromApi()
        {
            // Здесь ваш код вызова API
            return new List<Category>();
        }
    }
}
