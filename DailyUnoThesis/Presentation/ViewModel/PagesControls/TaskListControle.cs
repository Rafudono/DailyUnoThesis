//using DailyThesis.Model;
//using DailyThesis.Model.MainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using Windows.UI.Core;

//using System.Windows.Controls;
//using System.Windows.Threading;

namespace DailyUnoThesis.Presentation.ViewModel.PagesControls
{
    public class TaskListControle: Base
    {
        private CoreDispatcher dispatcher { get; set; }
        //public PageNavigation Navigation;
        private TaskListPage TaskPages;

        private List<Category> categories;
        private Category selectedCategory;
        private Category selectedFilterCategory;

        public List<Category> Categories
        { get => categories;
            set
            {
                categories = value;
                Signal();
            }
        }

        public Category SelectedCategory
        { get => selectedCategory;
            set
            {
                selectedCategory = value;
                Signal();
            }
        }

        public Category SelectedFilterCategory
        {
            get => selectedFilterCategory;
            set
            {
                selectedFilterCategory = value;
                Signal();
            }
        }



        private string taskTitle { get; set; }
        public string TaskTitle
        {
            get => taskTitle; set
            {
                taskTitle = value;
                Signal();
            }
        }

        private List<Mission> missions { get; set; }

        private Mission task { get; set; }

        public Mission Task
        {
            get => task; set
            {
                task = value;
                FindId();  
                Signal();
                ChangeCategory();
            }
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

        public List<Mission> Missions
        {
            get => missions; set
            {
                missions = value;
                Signal();
            }
        }

        private List<Mission> subtasks { get; set; }
        public List<Mission> Subtasks
        {
            get => subtasks; set
            {
                subtasks = value;
                Signal();
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
        public User AuthPerson { get; set; }




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

        private RelayCommand newTask;
        public RelayCommand NewTask
        {
            get
            {
                return newTask ?? new RelayCommand(async () =>
                {
                    Task = new() { LevelUp = 1 };
                    SelectedCategory = Categories[0];

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

       

        public TaskListControle()
        {
            AuthPerson = AuthorizedUser.GetInstance().AuthUser;
            Task = new() { LevelUp = 1 };
            GetCategories();

            SelectedCategory = new();
            //FillData();
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
            Categories = new();
            Categories = await APIHost.GetInstance().GetCategories();
            Categories = new List<Category>(Categories);
            Categories.Insert(0, new Category { Id = 0, Title = "Без категории" });
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
                    foreach (Mission downMis in missions.Where(s => s.IdUpMission == mis.Id))
                    {
                        downMis.LevelUp = 2;
                        Missions.Add(downMis);
                        Missions.AddRange(missions.Where(s => s.IdUpMission == downMis.Id));
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

            if (mission == null)
                Task = new();
            else
            {
                if (mission.Id == 0)
                    Task = Missions.LastOrDefault(s => s.Title == mission.Title);
                else
                    Task = Missions.FirstOrDefault(s => s.Id == mission.Id);
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
