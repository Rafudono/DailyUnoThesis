using System;
using System.Collections.Generic;
using System.Text;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using Windows.UI.Core;

namespace DailyUnoThesis.Presentation.ViewModel.PagesControls
{
    public partial class TaskViewModel: ObservableObject
    {

        private CoreDispatcher dispatcher { get; set; }
        private SelectedAndNewTask Page;

        [ObservableProperty]
        private List<Category> categories;

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
            if (SelectedCategory != null && Task != null)
            {
                if (Task.Category != null)
                {

                    int index = Categories.FindIndex(s => s.Id == Task.Category.Id);
                    SelectedCategory = Categories[index];
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
                    Subtasks = (List<Mission>)Task.InverseIdUpMissionNavigation;
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


        public TaskViewModel()
        {
            Task = new Mission();
        }

        public void GetTask(Mission mission)
        {
            
            Task = mission;
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
                //await FillData();

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
