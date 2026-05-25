using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

//using AndroidX.Collection;
using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using Windows.UI.Core;

namespace DailyUnoThesis.Presentation.ViewModel.PagesControls
{
    public partial class TaskViewModel : ObservableObject
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

        [ObservableProperty]
        private string taskDateSettings;

        [ObservableProperty]
        private bool isProject;




        // Свойство: Выбрано ли именно "Сегодня"?
        [ObservableProperty]
        // Говорим: "Когда меняется дата, уведомь интерфейс, что эти свойства тоже изменились"
        [NotifyPropertyChangedFor(nameof(IsTodaySelected))]
        [NotifyPropertyChangedFor(nameof(IsTomorrowSelected))]
        [NotifyPropertyChangedFor(nameof(IsCustomDateSelected))]
        // Основная дата задачи
        private DateTimeOffset? selectedDate;

        //[ObservableProperty]
        //private string textDate;

        public bool IsTodaySelected => SelectedDate?.Date == DateTime.Today;

        // Свойство: Выбрано ли именно "Завтра"?
        public bool IsTomorrowSelected => SelectedDate?.Date == DateTime.Today.AddDays(1).Date;

        // Свойство: Выбрана ли какая-то другая дата в календаре?
        public bool IsCustomDateSelected => SelectedDate != null && !IsTodaySelected && !IsTomorrowSelected;

        // Метод для установки даты из кнопок
        //[RelayCommand]
        //public void SetPresetDate(string type, CalendarView calendar)
        //{
        //    if (calendar != null)
        //    {
        //        calendar.SelectedDates.Clear(); // Сброс календаря
        //    }
        //    if (type == "Today") SelectedDate = DateTimeOffset.Now;
        //    else if (type == "Tomorrow") SelectedDate = DateTimeOffset.Now.AddDays(1);
        //    // Уведомляем интерфейс, что наши "флаги" изменились
        //    //RefreshSelection();
        //}
        private RelayCommand<object> setPresetDateToday;
        public RelayCommand<object> SetPresetDateToday
        {
            get
            {
                return setPresetDateToday ?? new RelayCommand<object>(async (parameter) =>
                {
                    
                        if (parameter != null)
                        {
                            if (parameter is CalendarView calendar)
                            {
                                calendar.SelectedDates.Clear();
                            }
                        }
                        if (IsTodaySelected)
                        {
                            SelectedDate = DateTimeOffset.MinValue;
                            return;
                        }
                        SelectedDate = DateTimeOffset.Now;
                    ConversionToText();



                    // Уведомляем интерфейс, что наши "флаги" изменились
                    //RefreshSelection();
                }
                );
            }
        }

        private RelayCommand<object> setPresetDateTomorrow;
        public RelayCommand<object> SetPresetDateTomorrow
        {
            get
            {
                return setPresetDateTomorrow ?? new RelayCommand<object>(async (parameter) =>
                {
                    if (parameter != null)
                    {
                        if (parameter is CalendarView calendar)
                        {
                            calendar.SelectedDates.Clear();
                        }
                    }
                    if (IsTomorrowSelected)
                    {
                        SelectedDate = DateTimeOffset.MinValue;
                        return;
                    }
                    SelectedDate = DateTimeOffset.Now.AddDays(1);
                    ConversionToText();


                    // Уведомляем интерфейс, что наши "флаги" изменились
                    //RefreshSelection();
                }
                );
            }
        }


        public void OnCalendarDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {

            // Обновляем дату во ViewModel при клике на календарь
            SelectedDate = args.AddedDates.FirstOrDefault();
            ConversionToText();

        }


        [ObservableProperty]
        private TimeSpan selectedTime = DateTime.Now.TimeOfDay; // По умолчанию текущее время
        [ObservableProperty]
        private bool useTime = false;

        partial void OnSelectedTimeChanged(TimeSpan value)
        {
            if ((SelectedDate == DateTimeOffset.MinValue || SelectedDate == null) && UseTime)
            {
               SelectedDate = DateTimeOffset.Now.Date;
            }
            ConversionToText();
        }


        private void ConversionToText()
        {
            if (SelectedDate != null)
            {
                if (Task.IdUpMissionNavigation != null)
                {
                    if (Task.IdUpMissionNavigation.EndDate != null && Task.IdUpMissionNavigation.EndDate != DateTime.MinValue)
                    {
                        if (SelectedDate.Value.Date > Task.IdUpMissionNavigation.EndDate)
                        {
                            SelectedDate = Task.IdUpMissionNavigation.EndDate;
                        }
                    }
                    else if (Task.IdUpMissionNavigation.IdUpMissionNavigation != null)
                    {
                        if (Task.IdUpMissionNavigation.IdUpMissionNavigation.EndDate != null && Task.IdUpMissionNavigation.IdUpMissionNavigation.EndDate != DateTime.MinValue)
                        {
                            if (SelectedDate.Value.Date > Task.IdUpMissionNavigation.IdUpMissionNavigation.EndDate)
                            {
                                SelectedDate = Task.IdUpMissionNavigation.IdUpMissionNavigation.EndDate;
                            }
                        }
                    }
                }
                if (SelectedDate == DateTimeOffset.MinValue || SelectedDate == null)
                    return;
                if (SelectedDate.Value.Date == DateTimeOffset.Now.Date)
                {
                    TaskDateSettings = "Сегодня";
                }
                else if (SelectedDate.Value.Date == DateTimeOffset.Now.AddDays(1).Date)
                {
                    TaskDateSettings = "Завтра";
                }
                else
                {
                    if (SelectedDate.Value.Year == DateTimeOffset.Now.Year)
                        TaskDateSettings = SelectedDate.Value.ToString("d MMM");
                    else
                        TaskDateSettings = SelectedDate.Value.ToString("d MMM yyyy");
                }
                if (UseTime)
                {
                    TaskDateSettings += ", " + SelectedTime.ToString("hh\\:mm");
                }
            }
        

           
        }


        // Когда меняется дата (в том числе через календарь)

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



        private RelayCommand<object> deleteDateSettings;
        public RelayCommand<object> DeleteDateSettings
        {
            get
            {
                return deleteDateSettings ?? new RelayCommand<object>(async (parameter) =>
                {
                    if (parameter is CalendarView calendar)
                    {
                        calendar.SelectedDates.Clear();
                    }
                    //Task.EndDate = DateTime.MinValue;
                    SelectedDate = DateTimeOffset.MinValue;
                    SelectedTime = TimeSpan.Zero;
                    UseTime = false;
                    TaskDateSettings = null;
                }
                );
            }
        }

        private RelayCommand applyDateSettings;
        public RelayCommand ApplyDateSettings
        {
            get
            {
                return applyDateSettings ?? new RelayCommand(async () =>
                {
                    await ViewModelStore.GetInstance().FillDataViewModels();
                }
                );
            }
        }


      

        private bool ExceedsTheDeadline()
        {

            return false;
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

        public void CreatNewTaskoutside()
        {
            
            NewTask.Execute(null);
        }
        public void GetBoolProject(bool boolProject)
        {
            IsProject = boolProject;
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
            if(Task.Id == mission.Id)
                Task = new() { LevelUp = 1 };
            Task = mission;
            DeleteDateSettings.Execute(1);
            if (Task.EndDate != null && Task.EndDate != DateTime.MinValue)
            {
                SelectedDate = Task.EndDate;
                if (Task.EndDate.Value.TimeOfDay != TimeSpan.Zero)
                {
                    UseTime = true;
                    SelectedTime = Task.EndDate.Value.TimeOfDay;
                }
            }
            else
            {
                SelectedDate = null;
            }

            ConversionToText();
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
                //Task.EndDat
                //foreach (var mis in Subtasks)
                //{
                //    mis.UserId = 1;
                //    mis.CategoryId = Task.CategoryId;
                //}
                if (SelectedDate != DateTimeOffset.MinValue && SelectedDate != null)
                {
                    if (!UseTime)
                        Task.EndDate = SelectedDate.Value.Date;
                    else
                        Task.EndDate = new DateTime(SelectedDate.Value.Year, SelectedDate.Value.Month, SelectedDate.Value.Day,
                              SelectedTime.Hours, SelectedTime.Minutes, 0);

                }
                else
                {
                    Task.EndDate = DateTime.MinValue;
                }
                
                if (Subtasks != null)
                {
                    Task.InverseIdUpMissionNavigation = Subtasks;
                    await EditSubtasksCategory(Task);
                }
                Task.IsProject = IsProject;

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
                    Task = await APIHost.GetInstance().GetLastMission(Task);
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
