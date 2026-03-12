//using DailyThesis.Model;
//using DailyThesis.Model.MainClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using Uno.Extensions;
using Windows.UI.Core;

//using System.Windows.Controls;
//using System.Windows.Threading;

namespace DailyUnoThesis.Presentation.ViewModel.PagesControls
{
    public partial class TaskListControle: ObservableObject
    {
        public int[] Items { get; } = new[] { 1, 2, 3 };
        public string SomeText { get; } = "Lorem Ipsum";
        private CoreDispatcher dispatcher { get; set; }
        //public PageNavigation Navigation;
        private TaskListPage TaskPages;
        //[ObservableProperty]
        //private ObservableCollection<Category> categories;
        public ObservableCollection<Category> Categories => CategoryService.Instance.Categories;
        [ObservableProperty]
        private Category selectedCategory;
        [ObservableProperty]
        private Category selectedFilterCategory;

        //public List<Category> Categories
        //{ get => categories;
        //    set
        //    {
        //        categories = value;
        //        Signal();
        //    }
        //}

        //public Category SelectedCategory
        //{ get => selectedCategory;
        //    set
        //    {
        //        selectedCategory = value;
        //        Signal();
        //    }
        //}

        //public Category SelectedFilterCategory
        //{
        //    get => selectedFilterCategory;
        //    set
        //    {
        //        selectedFilterCategory = value;
        //        Signal();
        //    }
        //}


        [ObservableProperty]
        private string taskTitle;
        //public string TaskTitle
        //{
        //    get => taskTitle; set
        //    {
        //        taskTitle = value;
        //        Signal();
        //    }
        //}
        //[ObservableProperty]
        [ObservableProperty]
        private ObservableCollection<Mission> missions;

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

                    SelectedCategory = Categories.FirstOrDefault(s => s.Id == Task.Category.Id);
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
                PageNavigation.GetInstance().ChangeSelected(Task);
                if (Task.Id != 0)
                    PageNavigation.GetInstance().TaskPageControle.IsSplitViewPaneOpen = true;
            }
        }








        //public List<Mission> Missions
        //{
        //    get => missions; set
        //    {
        //        missions = value;
        //        Signal();
        //    }
        //}
        [ObservableProperty]
        private List<Mission> subtasks;
        //public List<Mission> Subtasks
        //{
        //    get => subtasks; set
        //    {
        //        subtasks = value;
        //        Signal();
        //    }
        //}

       
        [ObservableProperty]
        public User authPerson;



        //-
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



        //-
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


        private RelayCommand saveSubtasks;
        public RelayCommand SaveSubtasks
        {
            get
            {
                return saveSubtasks ?? new RelayCommand(async () =>
                {
                    ContentDialog contentDialog = new ContentDialog()
                    {
                        Content = "Сохранение подзадачи"
                    };
                    if (Task.Id != 0)
                    {

                        Subtasks.RemoveAll(s => s.Title == null || s.Title == "");


                    }
                }

                );

            }

        }

        [ObservableProperty]
        private double delButtonOpacity = 0;

        public void ShowButton()
        {
            DelButtonOpacity = 1;
        }
        public void HideButton() { DelButtonOpacity = 0; }

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
                                Missions.Remove(Mission);
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


        public TaskListControle()
        {
            AuthPerson = AuthorizedUser.GetInstance().AuthUser;
            GetCategories();
            SelectedCategory = new();
            FillData();
            Task = new() { LevelUp = 1 };
            PageNavigation.GetInstance().TaskListControle = this;
        }

        private async void CreateAndEditNewTask()
        {
            if (Task != null)
            {

                if(Subtasks != null)
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
                await FillData();

            }
        }


      

        public async Task FillData()
        {
            Mission mission = Task;
            Task = new() { LevelUp = 1};
            List<Mission> missions = new List<Mission>();
            missions = await APIHost.GetInstance().GetMissions();
            //GetCategories();
            await UpdateLists(mission, missions);
      
               
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

    


        private async Task UpdateLists(Mission mission, List<Mission> missions)
        {
            
            Missions = new();
            foreach (Mission mis in missions)
            {

                if (mis.IdUpMission == null)
                {
                    mis.LevelUp = 1;
                    
                    Missions.Add(mis);
                    foreach (Mission downMis in mis.InverseIdUpMissionNavigation/*.Where(s => s.IdUpMission == mis.Id)*/)
                    {
                        downMis.LevelUp = 2;
                        
                        //Missions.Add(downMis);
                        //Missions.AddRange(missions.Where(s => s.IdUpMission == downMis.Id));
                    }
                }
                //else
                //{

                //    if (missions.FirstOrDefault(s => s.Id == mis.IdUpMission) == null)
                //    {
                //        mis.LevelUp = 1;
                //        Missions.Add(mis);
                //    }
                
                //}
            }
            //missions = missions.OrderBy(s=>s.IdUpMission).ToList();
            Missions = new(Missions);

            
        }

        public async void DeleteTasks(Mission mission)
        {
            if(mission == null)
                { return; }

            if (mission.InverseIdUpMissionNavigation.Count > 0)
            {
                DeleteSubtasks(mission);        
            }
            mission.IsDelete = true;
            //await System.Threading.Tasks.Task.Delay(400);
            //Missions.Remove(mission);
            ////await System.Threading.Tasks.Task.Delay(300);
            //await System.Threading.Tasks.Task.Delay(500);
            //Missions = new(Missions);
            await System.Threading.Tasks.Task.Delay(400);
            await APIHost.GetInstance().DeleteMission(mission);
            await System.Threading.Tasks.Task.Delay(300);
            Missions.Remove(mission);
            //await FillData();
        }

        private void DeleteSubtasks(Mission mission)
        {
            foreach (Mission submission in mission.InverseIdUpMissionNavigation)
            {
                submission.IsDelete = true;
            }
        
        }

        internal void SetControl(TaskListPage pass)
        {
            //this.Navigation = PageNavigation.GetInstance().;
            TaskPages = pass;
        }


        internal void SetDispatcher(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }
    }
}
