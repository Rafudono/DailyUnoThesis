
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using Windows.UI.Core;
namespace DailyUnoThesis.Presentation.ViewModel.PagesControls
{
    public class OverdueTaskControle : Base
    {
        private CoreDispatcher dispatcher;
        //public PageNavigation Navigation;
        private TaskOverdueListPage TaskPages;
        private string TypePage;

        private List<Category> categories;
        private Category selectedCategory;
        public List<Category> Categories
        {
            get => categories;
            set
            {
                categories = value;
                Signal();
            }
        }

        public Category SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
                Signal();
            }
        }



        private string taskTitle;
        public string TaskTitle
        {
            get => taskTitle; set
            {
                taskTitle = value;
                Signal();
            }
        }

        private List<Mission> missions;

        private Mission task;

        public Mission Task
        {
            get => task; set
            {
                task = value;
                FindId();
                Signal();
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
            if (task != null)
            {
                if (task.InverseIdUpMissionNavigation != null)
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
                    //await GetComplete();
                    Missions = new(Missions);
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
                    //MessageBox.Show("yep");
                    if (Task.Id != 0)
                    {

                        Subtasks.RemoveAll(s => s.Title == null || s.Title == "");


                    }
                }

                );

            }

        }







        public OverdueTaskControle()
        {
            AuthPerson = AuthorizedUser.GetInstance().AuthUser;
            Task = new();
            //FillData();
            
        }

        private async void CreateAndEditNewTask()
        {
            if (Task != null)
            {

                if (Subtasks != null)
                    Subtasks.RemoveAll(s => s.Title == null || s.Title == "");
                foreach (var mis in Subtasks)
                {
                    mis.UserId = 1;
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
                //await GetComplete();

            }
        }








        public async Task GetOverdue()
        {
            Mission mission = Task;
            Task = new();

            List<Mission> missions = new List<Mission>();
            missions = await APIHost.GetInstance().GetOverdue();
            await UpdateLists(mission, missions);

        }


        public async Task GetCategories()
        {
            Categories = await APIHost.GetInstance().GetCategories();
            Categories = new List<Category>(Categories);
            Categories.Insert(0, new Category { Id = 0, Title = "Без категории" });
            SelectedCategory = Categories[0];


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
                else
                {

                    if (missions.FirstOrDefault(s => s.Id == mis.IdUpMission) == null)
                    {
                        mis.LevelUp = 1;
                        Missions.Add(mis);
                    }

                }
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

        internal void SetControl(TaskOverdueListPage pass)
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

