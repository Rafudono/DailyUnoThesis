using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using DailyUnoThesis.Presentation.View.Timer;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyUnoThesis.Presentation.ViewModel.NavigationClasses;

public partial class PageNavigation : Base
{
private INavigator _navigator;

    private string? name;

    private static PageNavigation instance;
    public static PageNavigation GetInstance()
    {
        if (instance == null)
        {
            instance = new PageNavigation();              
        }
        return instance;
    }
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

    public void GetPageCategory(TaskListPageCategory pageCategory)
    {

        PageCategory = pageCategory;
        CategotyControle = pageCategory.DataContext as TaskCategotyControle;
        CategotyControle.GetIdCategory(1);
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

    public Pomodoro PomodoroPage { get; private set; }

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

    public PageNavigation()
    {
        instance = this;
    }
    public string? Title { get; }

    public ICommand GoToSecond { get; }

    //private async Task GoToSecondView()
    //{
    //    await _navigator.NavigateViewModelAsync<SecondViewModel>(this, data: new Entity(Name!));
    //}
}
