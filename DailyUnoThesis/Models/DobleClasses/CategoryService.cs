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



            var dataProj = await APIHost.GetInstance().GetProject();
            Projects.AddRange(dataProj);


        }

        public async Task RefreshFromProjectsDatabaseAsync()
        {
            Projects.Clear();
            ArchivalProjects.Clear();
              var dataProj = await APIHost.GetInstance().GetProject();
            AssignmentOfProjects.AddRange(dataProj);
            Projects.AddRange(dataProj.Where(s => (s.IdUpCategory == null || s.IdUpCategory == 0) && s.ProgressstatesId == (int)ProgressStateEnum.InProgress));
            ArchivalProjects.AddRange(dataProj.Where(s => (s.IdUpCategory == null || s.IdUpCategory == 0) && s.ProgressstatesId != (int)ProgressStateEnum.InProgress));

        }

        private async Task<List<Category>> GetCategoriesFromApi()
        {
            // Здесь ваш код вызова API
            return new List<Category>();
        }
    }
}
