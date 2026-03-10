using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using Windows.UI.Core;

namespace DailyUnoThesis.Presentation.ViewModel.PagesControls
{
    public partial class TaskViewModel: ObservableObject
    {

        private CoreDispatcher dispatcher { get; set; }
        private SelectedAndNewTask Page;

        //[ObservableProperty]
        //private ObservableCollection<Category> categories;
        public ObservableCollection<Category> Categories => CategoryService.Instance.Categories;

        [ObservableProperty]
        private Category selectedCategory;

        [ObservableProperty]
        private Category selectedFilterCategory;

        [ObservableProperty]
        private string taskTitle;

        [ObservableProperty]
        private List<Mission> missions;

        [ObservableProperty]
        private List<Mission> subtasks;

        [ObservableProperty]
        private Mission task;

        //public Mission Task
        //{
        //    get => task; set
        //    {
        //        task = value;
        //        FindId();
        //        Signal();
        //        ChangeCategory();
        //    }
        //}
        partial void OnTaskChanged(Mission value)
        {
            FindId();
            ChangeCategory();
        }
        private async Task ChangeCategory()
        {
            if (SelectedCategory == null)
            {
                SelectedCategory = new Category();  
            }
            if (SelectedCategory != null && Task != null)
            {
                if (Task.Category != null)
                {
                    //int index = Categories.FindIndex(s => s.Id == Task.Category.Id);
                    SelectedCategory = Categories.FirstOrDefault(s => s.Id == Task.Category.Id); /*Categories[index];*/
                }
                else
                {
                    SelectedCategory = Categories[0];
                }
            }

        }
        private void FindId()
        {
            if (Task != null)
            {
                if (Task.InverseIdUpMissionNavigation != null)
                {
                    List<Mission> missions = new();
                    missions.AddRange(Task.InverseIdUpMissionNavigation);
                    Subtasks = missions;
                }
            }
        }

        private RelayCommand createSubtask;
        public RelayCommand CreateSubtask
        {
            get
            {
                return createSubtask ?? new RelayCommand(async () =>
                {
                    if (Subtasks == null)
                    {
                        Subtasks = new();
                    }
                    Mission mission = new();
                    if (Task.Id != null)
                        mission.IdUpMission = Task.Id;
                    Subtasks.Add(mission);
                    Subtasks = new(Subtasks);
                }

                );

            }

        }



        private RelayCommand newTask;
        public RelayCommand NewTask
        {
            get
            {
                return newTask ?? new RelayCommand(async () =>
                {
                    Task = new() { LevelUp = 1 };
                    //SelectedCategory = Categories[0];
                }
                );
            }
        }

        private RelayCommand createAndEditTask;
        public RelayCommand CreateAndEditTask
        {
            get
            {
                return createAndEditTask ?? new RelayCommand(async () =>
                {
                    CreateAndEditNewTask();
                }

                );

            }

        }




        public TaskViewModel()
        {
            Task = new Mission();
            GetCategories();
            SelectedCategory = new();
            Task = new() { LevelUp = 1 };
        }

        public void GetTask(Mission mission)
        {
            Task = mission;
        }

        public async Task GetCategories()
        {
            //Categories = new();
            //Categories = await APIHost.GetInstance().GetCategories();
            //Categories = new ObservableCollection<Category>(Categories);
            //Categories.Insert(0, new Category { Id = 0, Title = "Без категории" });
            SelectedCategory = Categories[0];
            SelectedFilterCategory = Categories[0];


        }

        private async void CreateAndEditNewTask()
        {
            if (Task != null)
            {

                if (Subtasks != null)
                    Subtasks.RemoveAll(s => s.Title == null || s.Title == "");
                if (SelectedCategory.Id != 0)
                {
                    Task.CategoryId = SelectedCategory.Id;
                }
                foreach (var mis in Subtasks)
                {
                    mis.UserId = 1;
                    mis.CategoryId = Task.CategoryId;
                }
                Task.InverseIdUpMissionNavigation = Subtasks;

                if (Task.Id == 0)
                {
                    await APIHost.GetInstance().CreateMission(Task);
                }
                else
                {
                    await APIHost.GetInstance().EditMission(Task);

                }
                
                if (Task == null)
                    Task = new();
                else
                {
                    Task = await APIHost.GetInstance().GetLastMission(Task.Id, Task.Title);
                }
                await PageNavigation.GetInstance().FillDataViewModels();
                //await PageNavigation.GetInstance().CurPage.FillData();

            }
        }

        internal void SetControl(SelectedAndNewTask pass)
        {
            //this.Navigation = PageNavigation.GetInstance().;
            if (Page == null)
                Page = pass;
            
        }


        internal void SetDispatcher(CoreDispatcher dispatcher)
        {
            if (this.dispatcher == null)
            {
                this.dispatcher = dispatcher;
                //GetListPage();
            }

        }
    }
}
