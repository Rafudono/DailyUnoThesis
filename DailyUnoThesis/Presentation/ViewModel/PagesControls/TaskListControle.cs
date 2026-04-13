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
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.VisualBasic;
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
        public ObservableCollection<Category> Categories => CategoryService.Instance.FilterCategories;
        [ObservableProperty]
        private Category selectedCategory;
        [ObservableProperty]
        private Category selectedFilterCategory;
        [ObservableProperty]
        private string filterDate;
        [ObservableProperty]
        private MissionSuggestionDto missionSuggestion;
        [ObservableProperty]
        private bool isFilter = false;
        [ObservableProperty]
        private bool isSearchFilter = false;

        private RelayCommand resetData;
        public RelayCommand ResetData
        {
            get
            {
                return resetData ?? new RelayCommand(async () =>
                {
                    //FilterDate = DateTime.MinValue;
                }

                );

            }

        }


        [ObservableProperty]
        private string searchText;

        partial void OnSearchTextChanged(string value)
        {
            if (value.IsNullOrEmpty() && IsSearchFilter == true)
                SearchReset();
        }

        private async Task SearchReset()
        {
            MissionSuggestion.IdMission = 0;
            MissionSuggestion.Title = null;
            IsSearchFilter =false;
               await SubmitFilters();
        }
        [ObservableProperty]
        private ObservableCollection<MissionSuggestionDto> suggestions = new();

        // Поле для отмены старых запросов
        private CancellationTokenSource? _searchCts;
        private bool SelectSearchMission = false;

        public async void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                string query = sender.Text;

                if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                {
                    Suggestions.Clear();
                    return;
                }

                
                //_searchCts?.Cancel(); 
                _searchCts = new CancellationTokenSource();
                var token = _searchCts.Token;

                try
                {
                    await System.Threading.Tasks.Task.Delay(300, token);
                    var results = await APIHost.GetInstance().GetSuggestions(query);
 
                    //if (!token.IsCancellationRequested)
                    //{
                        //Suggestions.Clear();
                        //foreach (var item in results)
                        //{
                        //    Suggestions.Add(item);
                        //}
                        Suggestions = results;
                    //}
                }
                catch (OperationCanceledException)
                {
                    // Это нормально, просто пользователь печатает быстрее, чем работает интернет
                }
            }
        }

        public async Task OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var selected = args.SelectedItem as MissionSuggestionDto;
            SelectSearchMission = true;
            MissionSuggestion.IdMission = selected.IdMission;
            MissionSuggestion.Title = selected.Title;
            IsSearchFilter = true;
            await SubmitFilters();
            //Missions = await APIHost.GetInstance().GetSearchMission(selected.Title, selected.IdMission);
        }

        public async Task OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var query = sender.Text;
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                Suggestions.Clear();
                return;
            }

            if (SelectSearchMission)
            {
                SelectSearchMission = false;
                return;
            }
            string finalQuery = args.QueryText;
            MissionSuggestion.IdMission = 0;
            MissionSuggestion.Title = finalQuery;
            IsSearchFilter = true;
            await SubmitFilters();
            //Missions =  await APIHost.GetInstance().GetSearchMission(finalQuery,0);
        }




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
                ViewModelStore.GetInstance().ChangeSelected(Task);
                //if (Task.Id != 0)
                //    ViewModelStore.GetInstance().PanelTask.IsSplitViewPaneOpen = true;
            }
        }

        public async void OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            // args.InvokedItem — это объект задачи или категории, на который кликнули
            var clickedItem = args.InvokedItem as Mission;

            // Открываем панель подробностей
            if (Task.Id != 0)
                ViewModelStore.GetInstance().PanelTask.IsSplitViewPaneOpen = true;

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


        private RelayCommand applyDateFilter;
        public RelayCommand ApplyDateFilter
        {
            get
            {
                return applyDateFilter ?? new RelayCommand(async () =>
                {
                    FilterDate = null;
                    IsFilter = true;
                    if (MissionSuggestion.End != null && MissionSuggestion.End != DateTime.MinValue)
                    {
                        if (MissionSuggestion.Start != null)
                        {
                            FilterDate = ConversionToText((DateTime)MissionSuggestion.Start) + "-";
                            FilterDate += ConversionToText((DateTime)MissionSuggestion.End);
                        }
                        else
                            FilterDate = ConversionToText((DateTime)MissionSuggestion.End);
                    }
                    List<int> categories = new List<int>();
                    categories.AddRange(Categories.Where(s => s.IsCheack == true).Select(s => s.Id));
                    MissionSuggestion.CateroriesId = categories;
                    await SubmitFilters();
                }
                );
            }
        }


        private string ConversionToText(DateTime date)
        {
            if (date == null || date == DateTime.MinValue)
                return "";

           
            if (date.Date == DateTime.Now.Date)
            {
                return "Сегодня";
            }
            else if (date.Date == DateTime.Now.AddDays(1).Date)
            {
                return "Завтра";
            }
            else
            {
                if (date.Year == DateTime.Now.Year)
                    return date.ToString("d MMM");
                else
                    return date.ToString("d MMM yyyy");
            }


        }

        private RelayCommand<object> deleteDateFilter;
        public RelayCommand<object> DeleteDateFilter
        {
            get
            {
                return deleteDateFilter ?? new RelayCommand<object>(async (parameter) =>
                {
                    if (parameter is CalendarView calendar)
                    {
                        calendar.SelectedDates.Clear();
                    }
                    FilterDate = null;
                    MissionSuggestion.End = null;
                    MissionSuggestion.Start = null;
                    SubmitFilters();
                    
                }
                );
            }
        }


        private RelayCommand<object> deleteFilter;
        public RelayCommand<object> DeleteFilter
        {
            get
            {
                return deleteFilter ?? new RelayCommand<object>(async (parameter) =>
                {
                    
                    if (parameter is CalendarView calendar)
                    {
                        calendar.SelectedDates.Clear();
                    }
                    FilterDate = null;
                    SearchText = null;
                    foreach (var item in Categories)
                    {
                        item.IsCheack = false;
                    }
                    MissionSuggestion = new();
                   
                    if(IsFilter != false || IsSearchFilter ==true)
                        await FillData();
                    IsFilter = false;
                    IsSearchFilter = false;
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
            MissionSuggestion = new();
            //ViewModelStore.GetInstance().AllTasks = this;
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
            Missions = new();
            Missions.AddRange(missions);
            //GetCategories();
            //await UpdateLists(mission, missions);
      
               
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

                if (mis.IdUpMission == null || mis.IdUpMission == 0)
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
                if (submission.InverseIdUpMissionNavigation.Count != 0)
                {
                    DeleteSubtasks(submission);
                }
            }
        
        }


        [ObservableProperty]
        private DateTimeOffset? filterStartDate;

        [ObservableProperty]
        private DateTimeOffset? filterEndDate;
        private bool isProcessing = false;

        public void OnCalendarDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (isProcessing) return;

            var selected = sender.SelectedDates;
            if (args.AddedDates.Count == 0)
            {

                if (MissionSuggestion.End != null )
                   if( args.RemovedDates[0].Date == MissionSuggestion.End.Value)
                {
                    sender.SelectedDates.Clear();
                    MissionSuggestion.Start = null;
                    MissionSuggestion.End = null;
                    return;
                }
            }

            if (selected.Count == 2)
            {
                var start = selected.Min();
                var end = selected.Max();

                isProcessing = true;

                for (var dt = start.AddDays(1); dt < end; dt = dt.AddDays(1))
                {
                    if (!sender.SelectedDates.Contains(dt))
                    {
                        sender.SelectedDates.Add(dt);
                    }
                }
                MissionSuggestion.Start = start.Date;
                MissionSuggestion.End = end.Date;
                //FilterStartDate = start;
                //FilterEndDate = end;
                isProcessing = false;
                return;
            }
            else if (selected.Count == 1)
            {
                MissionSuggestion.Start = null;
                MissionSuggestion.End = selected[0].Date;
                //FilterStartDate = null;
                //FilterEndDate = selected[0];
                return;
            }

            if (args.RemovedDates.Count > 0 || (selected.Count > 2 && args.AddedDates.Count > 0))
            {
                isProcessing = true;

                var nextDate = args.AddedDates.Count > 0
                               ? args.AddedDates[0]
                               : args.RemovedDates[0];

                sender.SelectedDates.Clear();

                isProcessing = false;
                if (args.RemovedDates.Count > 0)
                {
                    if (args.RemovedDates[0].Date == MissionSuggestion.Start.Value)
                    {
                        sender.SelectedDates.Clear();
                        MissionSuggestion.Start = null;
                        MissionSuggestion.End = null;
                    }
                }
                else
                    sender.SelectedDates.Add(nextDate);
                return;
            }
           
        }

        private async Task SubmitFilters()
        {
            //IsFilter = true;
            if (MissionSuggestion.IsEmpty())
                FillData();
            Missions = await APIHost.GetInstance().GetSearchMission(MissionSuggestion);

            MissionSuggestion.UserId = 0;
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
