using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Android.Graphics.Drawables;
//using Android.OS;
using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using DailyUnoThesis.Presentation.View.Timer;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;
using DailyThesisAPI.SignalR;
using Uno.Extensions.Navigation;
 
namespace DailyUnoThesis.Presentation.ViewModel.NavigationClasses;

public partial class PageNavigation : Base
{

    private int namderMainPanel = 0;
    public int NamderMainPanel
    {
        get => namderMainPanel;
        set
        {
            namderMainPanel = value;
            Signal();
        }
    }

    private string? name;

    //private static PageNavigation instance;
    //public static PageNavigation GetInstance()
    //{
    //    if (instance == null)
    //    {
    //        instance = new PageNavigation();              
    //    }
    //    return instance;
    //}
    public PageNavigation Navigation;
    private TaskPages TaskPages;
    //public Pomodoro PomodoroPage;
    public PanelTimer TimerPage;

    //Раздел задач
    public TaskPageControle TaskPageControle;

    //Все задачи
    public TaskListPage TaskListPage;
    public TaskListControle TaskListControle;

    //Катагории
    public TaskListPageCategory PageCategory;
    public TaskCategotyControle CategotyControle;

    //Меню подробностей задач
    public SelectedAndNewTask SelectedAndNewTask;
    public TaskViewModel SelectedViewModel;
    private readonly INavigator _navigator;

    public void GetPageCategory(TaskListPageCategory pageCategory)
    {

        PageCategory = pageCategory;
        CategotyControle = pageCategory.DataContext as TaskCategotyControle;
        CategotyControle.GetIdCategory(CategoryService.Instance.FilterCategories[1]);
    }


    public void GetPageTask(SelectedAndNewTask page)
    {
        SelectedAndNewTask = page;
        SelectedViewModel = page.DataContext as TaskViewModel;
    }

    public void ChangeSelected(Mission mission)
    {
        if (SelectedViewModel != null && mission.Id != 0)
        {
            SelectedViewModel.GetTask(mission);
            
        }
    }

    public async Task FillDataViewModels()
    {
       await TaskListControle.FillData();
       await  CategotyControle.FillData();
    }

    //public async void GetClass()
    //{
    //    if (instance != null)
    //    {
    //        Navigation = this;
    //        TaskPages = new TaskPages();
    //        TaskListPage = new TaskListPage();
    //        CurPageCategory = TaskListPage;
    //        PomodoroPage = new Pomodoro();
    //        TimerPage = new PanelTimer();   
    //        CurPage = TaskPages;
    //    }
    //}

    private bool isMenuTimerOpen = true;
    public bool IsMenuTimerOpen
    {
        get => isMenuTimerOpen;
        set
        {
            isMenuTimerOpen = value;
            Signal();
        }
    }

    private bool isMenuTimer = false;
    public bool IsMenuTimer
    {
        get => isMenuTimer;
        set
        {
            isMenuTimer = value;
            Signal();
        }
    }

    private bool isPause = true;
    public bool IsPause
    {
        get => isPause;
        set
        {
            isPause = value;
            Signal();
        }
    }

    private bool isEnd = false;
    public bool IsEnd
    {
        get => isEnd;
        set
        {
            isEnd = value;
            Signal();
            //Test();
            //if (IsEnd && ViewModelStore.GetInstance().PomodoroTime.IsAbsoluteEnd)
            //{
            //    AbsoluteEnd();
            //}
        }
    }

    public void AbsoluteEnd()
    {
        IsPause = false;
        ViewModelStore.GetInstance().RegularTimer.CountupTimer.IsPaused = false;
        //ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.IsPaused = false;
            //ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.Timer.Start();
            //ViewModelStore.GetInstance().RegularTimer.CountupTimer.Timer.Start();
        //ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.PauseTimer();
        ViewModelStore.GetInstance().RegularTimer.CountupTimer.PauseTimer();
    }

    private void Test()
    {
      bool end = ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.IsEnd;

    }


    private bool stopForBreak = false;
    public bool StopForBreak
    {
        get => stopForBreak;
        set
        {
            stopForBreak = value;
            Signal();
            //if (StopForBreak && !ViewModelStore.GetInstance().PomodoroTime.IsStopForBreak)
            //{
            //    StopRegular();
            //}
            //else if (!StopForBreak && !ViewModelStore.GetInstance().PomodoroTime.IsStopForBreak)
            //{
            //    StartRegular();
            //}
        }
    }

    public void StopRegular()
    {
        ViewModelStore.GetInstance().RegularTimer.CountupTimer.IsPaused = false;
        ViewModelStore.GetInstance().RegularTimer.CountupTimer.Timer.Start();
        ViewModelStore.GetInstance().RegularTimer.CountupTimer.PauseTimer();
    }

    public void StartRegular()
    {
        if (ViewModelStore.GetInstance().RegularTimer.CountupTimer.Timer != null)
        {
            ViewModelStore.GetInstance().RegularTimer.CountupTimer.IsPaused = true;
            ViewModelStore.GetInstance().RegularTimer.CountupTimer.Timer.Stop();
            ViewModelStore.GetInstance().RegularTimer.CountupTimer.PauseTimer();
        }
    }

    private bool isIcon = true;
    public bool IsIcon
    {
        get => isIcon;
        set
        {
            isIcon = value;
            Signal();
        }
    }

    private Page curPage { get; set; }
    public Page CurPage
    {
        get => curPage;
        set
        {
            curPage = value;
            Signal();
        }
    }

    private int numCurPage { get; set; }
    public int NumCurPage
    {
        get => numCurPage;
        set
        {
            numCurPage = value;
            Signal();
        }
    }

    private Page curPageCategory { get; set; }
    public Page CurPageCategory
    {
        get => curPageCategory;
        set
        {
            curPageCategory = value;
            Signal();
        }
    }

    private RelayCommand openMenuTimer;
    public RelayCommand OpenMenuTimer
    {
        get
        {
            return openMenuTimer ?? new RelayCommand(async () =>
            {
                IsMenuTimerOpen = IsMenuTimerOpen ? false : true;
            }
            );
        }
    }

    private RelayCommand stopTimers;

    public RelayCommand StopTimers
    {
        get
        {
            return stopTimers ?? new RelayCommand(async () =>
            {
                if (ViewModelStore.GetInstance().RegularTimer.CountupTimer.IsPaused && ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.IsPaused)
                {
                    IsPause = false;
                    ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.PauseTimer();
                    if (!ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.BreakTimer)
                        ViewModelStore.GetInstance().RegularTimer.CountupTimer.PauseTimer();

                }
                else
                {
                    IsPause = IsPause ? false : true;
                    if (!ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.BreakTimer)
                        ViewModelStore.GetInstance().RegularTimer.CountupTimer.IsPaused = !IsPause;
                    ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.IsPaused = !IsPause;

                    if (IsPause)
                    {
                        ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.Timer.Start();
                        if(!ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.BreakTimer)
                        ViewModelStore.GetInstance().RegularTimer.CountupTimer.Timer.Start();
                    }
                    else
                    {
                        ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.Timer.Stop();
                        if (!ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.BreakTimer)
                            ViewModelStore.GetInstance().RegularTimer.CountupTimer.Timer.Stop();
                    }
                    ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.PauseTimer();
                    if (!ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.BreakTimer)
                        ViewModelStore.GetInstance().RegularTimer.CountupTimer.PauseTimer();
                }

                //if (IsPause)
                //{
                //    IsIcon = true;
                //}
                //else
                //{
                //    IsIcon = false;
                //}

                
            }

            );

        }

    }

    private RelayCommand restartTimers;
    public RelayCommand RestartTimers
    {
        get
        {
            return restartTimers ?? new RelayCommand(async () =>
            {
                ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.IsEnd = false;
                IsEnd = false;
                StopForBreak = false;
                IsPause = false;
                ViewModelStore.GetInstance().RegularTimer.CountupTimer.IsPaused = IsPause;
                ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.IsPaused = IsPause;
                ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.Timer.Stop();
                ViewModelStore.GetInstance().RegularTimer.CountupTimer.Timer.Stop();
                ViewModelStore.GetInstance().PomodoroTime.RestartCountdownTimer.Execute(null);
                ViewModelStore.GetInstance().RegularTimer.RestartCountupTimer.Execute(null);
            }
            );
        }
    }

       private RelayCommand closeMenuTimer;
    public RelayCommand CloseMenuTimer
    {
        get
        {
            return closeMenuTimer ?? new RelayCommand(async () =>
            {
                IsMenuTimer = false;
                IsPause = true;
                ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.IsPaused = false;
                ViewModelStore.GetInstance().RegularTimer.CountupTimer.Timer.Start();
                ViewModelStore.GetInstance().RegularTimer.CountupTimer.PauseTimer();

                ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.IsPaused = false;
                ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.Timer.Start();
                ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.PauseTimer();
                ViewModelStore.GetInstance().PomodoroTime.CountdownTimer.WithTheSecondTimer = false;
                ViewModelStore.GetInstance().PomodoroTime.IsDoubleTimer = false; 




            }

            );

        }

    }

    public Pomodoro PomodoroPage { get; private set; }

    private RelayCommand logOutCommand;
    public RelayCommand LogOutCommand
    {
        get
        {
            return logOutCommand ?? new RelayCommand(async () =>
            {
                await APIHost.GetInstance().Logout();
                await ConnectionToHub.Instance.Disconnect();
                var services = App.Services;
                if (services != null)
                {
                    //var navigator = services.GetRequiredService<INavigator>();
                    await _navigator.NavigateBackAsync(this);
                }
            });
        }
    }

    private RelayCommand openTaskListPage;
    public RelayCommand OpenTaskListPage
    {
        get
        {
            return openTaskListPage ?? new RelayCommand(async () =>
            {
                CurPage = TaskPages;
            }

            );

        }

    }



    private RelayCommand openPomodoroPage;
    public RelayCommand OpenPomodoroPage
    {
        get
        {
            return openPomodoroPage ?? new RelayCommand(async () =>
            {
                CurPage = TimerPage;
            }
            );

        }

    }

    public PageNavigation(INavigator navigator)
    {
        _navigator = navigator;
    }
    public string? Title { get; }

    public ICommand GoToSecond { get; }

    //private async Task GoToSecondView()
    //{
    //    await _navigator.NavigateViewModelAsync<SecondViewModel>(this, data: new Entity(Name!));
    //}
}
