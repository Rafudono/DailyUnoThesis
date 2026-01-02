using DailyUnoThesis.Models;
using DailyUnoThesis.Models.MainClasses;
//using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DailyUnoThesis.Presentation.View.Pages;
//using System.Windows.Threading;

namespace DailyUnoThesis.Presentation.ViewModel.PagesControls
{
    public class TaskCompleteControle : Base
    {
        private Dispatcher dispatcher;
        //public PageNavigation Navigation;
        private TaskCompleteListPage TaskPages;
        private string TypePage;



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
                    await GetComplete();
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







        public TaskCompleteControle()
        {
            AuthPerson = AuthorizedUser.GetInstance().AuthUser;
            Task = new();
            //FillData();
            GetComplete();
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
                await GetComplete();

            }
        }




     

       

        public async Task GetComplete()
        {
            Mission mission = Task;
            Task = new();

            List<Mission> missions = new List<Mission>();
            missions = await APIHost.GetInstance().GetMCompleteList();
            await UpdateLists(mission, missions);
           
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

        internal void SetControl(TaskCompleteListPage pass)
        {
            //this.Navigation = PageNavigation.GetInstance().;
            TaskPages = pass;
        }


        internal void SetDispatcher(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }
    }
}

