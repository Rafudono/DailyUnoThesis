using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Dashboard;
using DailyUnoThesis.Presentation.View.Pages;
using DailyUnoThesis.Presentation.View.Pages.Projects;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using Windows.UI.Core;

namespace DailyUnoThesis.Presentation.ViewModel.DashboardControl;

public partial class DashboardViewModel: ObservableObject
{
    private CoreDispatcher dispatcher;
    private DashboardPage TaskPages;

    [ObservableProperty]
    private User _user;

    // Свойства статистики
    [ObservableProperty]
    private int _tasksTodayCount;

    [ObservableProperty]
    private int _activeTasksCount;

    [ObservableProperty]
    private int _completedTasksCount;

    [ObservableProperty]
    private DashboardProjectDto selectedProjest = new();

    [ObservableProperty]
    private string _focusTimeText = "0ч 0м";

    // Текст заметки
    [ObservableProperty]
    private string _dailyNote = "Если можешь не спать — не спи. Сон для слабых.\n— Изран.";


    [ObservableProperty]
    private bool isVisiblePersonalAccount = false;
// Списки для списков задач и проектов
[ObservableProperty]
    public ObservableCollection<DashboardMissionDto> activeMissions = new();

    [ObservableProperty]
    public ObservableCollection<DashboardProjectDto> activeProjects = new();


    public DashboardViewModel()
    {
        User = AuthorizedUser.GetInstance().AuthUser;
        LoadDashboardDataAsync();
    }

    public async Task LoadDashboardDataAsync()
    {
        try
        {
            var data = await APIHost.GetInstance().GetDashboardData();

            if (data != null)
            {
                // Данные пользователя НЕ перезаписываем из API, так как они уже установлены на клиенте.
                // Обновляем только динамическую статистику:
                TasksTodayCount = data.TasksTodayCount;
                ActiveTasksCount = data.ActiveTasksCount;
                CompletedTasksCount = data.CompletedTasksCount;
                FocusTimeText = data.FocusTimeText;

                // Заполняем список задач на сегодня
                ActiveMissions.Clear();
                foreach (var mission in data.ActiveMissions)
                {
                    ActiveMissions.Add(mission);
                }

                // Заполняем список приоритетных проектов (отсортированных по LastActivityDate)
                ActiveProjects.Clear();
                foreach (var project in data.ActiveProjects)
                {
                    ActiveProjects.Add(project);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки данных рабочего стола: {ex.Message}");
        }
    }

    public void ClosePersonalAccountPanel()
    {
        IsVisiblePersonalAccount = false;
    }

    public void CloseMenu()
    {
        IsVisiblePersonalAccount = false;
    }

    private RelayCommand closeAndOpenlePersonalAccount;
    public RelayCommand CloseAndOpenPersonalAccount
    {
        get
        {
            return closeAndOpenlePersonalAccount ?? new RelayCommand(async () =>
            {
                IsVisiblePersonalAccount = IsVisiblePersonalAccount ? false : true;

            }

            );

        }

    }

    private RelayCommand editUser;
    public RelayCommand EditUser
    {
        get
        {
            return editUser ?? new RelayCommand(async () =>
            {

                IsVisiblePersonalAccount =false;
            }
            );
        }
    }


    private  RelayCommand openTodayList;
    public  RelayCommand OpenTodayList
    {
        get
        {
            return openTodayList ?? new RelayCommand(async () =>
            {

                ViewModelStore.GetInstance().Main.NamderMainPanel = 1;
                if (ViewModelStore.GetInstance().PanelTask == null)
                    await Task.Delay(1000);
                ViewModelStore.GetInstance().PanelTask.NamderMainPanel = 1;

            }
            );
        }
    }

    partial void OnSelectedProjestChanged(DashboardProjectDto value)
    {
        if (value != null && value.Id != 0)
        {
            OpenProjectList.Execute(null);
        }
    }


    private RelayCommand openProjectList;
    public RelayCommand OpenProjectList
    {
        get
        {
            return openProjectList ?? new RelayCommand(async () =>
            {

                ViewModelStore.GetInstance().Main.NamderMainPanel = 2;
                if (ViewModelStore.GetInstance().PanelProject == null)
                    await Task.Delay(1000);

                
                var project = CategoryService.Instance.AssignmentOfProjects.FirstOrDefault(s=>s.Id == SelectedProjest.Id);
                if(project != null)
                ViewModelStore.GetInstance().PanelProject.GoToProjectFolder(project);


               
            }
            );
        }
    }

   



    public async void SetControl(DashboardPage pass)
    {
        if (TaskPages == null)
            TaskPages = pass;
        //SelectedBaseCategory = ListNavigations[0];
        

    }


    public void SetDispatcher(CoreDispatcher dispatcher)
    {
        if (this.dispatcher == null)
        {
            this.dispatcher = dispatcher;
            //GetListPage();
        }

    }
}
