using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
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

        private RelayCommand<Mission> completeTaskCommand;
        public RelayCommand<Mission> CompleteTaskCommand
        {
            get
            {
                return completeTaskCommand ?? new RelayCommand<Mission>(async (Mission) =>
                {
                    if (Mission != null && Mission.IsComplete != null)
                    {
                        if ((bool)Mission.IsComplete)
                        {
                            if (Mission.IdUpMission == 0 || Mission.IdUpMission == null)
                            {
                                ChangeOfCompletionStatusAndRemoving(Mission);
                                Mission.IsRemoving = true;
                                await System.Threading.Tasks.Task.Delay(400);
                                //Missions.Remove(Mission);
                            }
                            else
                            {
                                ChangeOfCompletionStatus(Mission);

                            }
                        }

                        await APIHost.GetInstance().EditMission(Mission);
                        //await System.Threading.Tasks.Task.Delay(400);
                        //FillData();

                        //await System.Threading.Tasks.Task.Delay(300);

                        //await System.Threading.Tasks.Task.Delay(500);
                        //Missions = new(Missions);
                    }
                }
                );
            }
        }

        private void ChangeOfCompletionStatusAndRemoving(Mission Mission)
        {
            if (Mission.InverseIdUpMissionNavigation == null) return;
            foreach (var mis in Mission.InverseIdUpMissionNavigation)
            {
                mis.IsComplete = true;
                mis.IsRemoving = true;

                if (mis.InverseIdUpMissionNavigation.Count != 0)
                {
                    ChangeOfCompletionStatusAndRemoving(mis);
                }
            }
        }

        private void ChangeOfCompletionStatus(Mission Mission)
        {
            if (Mission.InverseIdUpMissionNavigation == null) return;
            foreach (var mis in Mission.InverseIdUpMissionNavigation)
            {
                mis.IsComplete = true;

                if (mis.InverseIdUpMissionNavigation.Count != 0)
                {
                    ChangeOfCompletionStatus(mis);
                }
            }
        }


        private RelayCommand<Mission> deleteTask;
        public RelayCommand<Mission> DeleteTask
        {
            get
            {
                return deleteTask ?? new RelayCommand<Mission>(async (Mission) =>
                {
                    if (Mission != null)
                    {
                        DeleteTasks(Mission);
                    }

                }
                );
            }
        }

        public async void DeleteTasks(Mission mission)
        {
            if (mission == null)
            { return; }

            if (mission.InverseIdUpMissionNavigation.Count > 0)
            {
                DeleteSubtasks(mission);
            }
            mission.IsDelete = true;
            await System.Threading.Tasks.Task.Delay(400);
            await APIHost.GetInstance().DeleteMission(mission);
            await System.Threading.Tasks.Task.Delay(300);
            if (mission.Id == Task.Id)
                Task = new();
            else
            {
                Subtasks.Remove(mission);
                Subtasks = new(Subtasks);
            }
            await ViewModelStore.GetInstance().FillDataViewModels();
           

        }

        private void DeleteSubtasks(Mission mission)
        {
            foreach (Mission submission in mission.InverseIdUpMissionNavigation)
            {
                submission.IsDelete = true;

                if (submission.InverseIdUpMissionNavigation.Count != 0)
                {
                    DeleteSubtasks(submission);
                }
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
                //foreach (var mis in Subtasks)
                //{
                //    mis.UserId = 1;
                //    mis.CategoryId = Task.CategoryId;
                //}
                if (Subtasks != null)
                {
                    Task.InverseIdUpMissionNavigation = Subtasks;
                    await EditSubtasksCategory(Task);
                }

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
                    Subtasks = (List<Mission>?)Task.InverseIdUpMissionNavigation;
                    
                }
                await ViewModelStore.GetInstance().FillDataViewModels();
                //await PageNavigation.GetInstance().CurPage.FillData();

            }
        }
        private async Task EditSubtasksCategory(Mission mission)
        {
            if (mission.InverseIdUpMissionNavigation.Count == 0)
                return;
            foreach (var mis in mission.InverseIdUpMissionNavigation)
            {
                mis.UserId = 1;
                mis.CategoryId = Task.CategoryId;
               await EditSubtasksCategory(mis);
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
