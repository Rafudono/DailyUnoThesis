using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using DailyThesisAPI.SignalR;
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
    private string _dailyNote = "Вы близки к истине. Ещё один рывок.";


    [ObservableProperty]
    private bool isVisiblePersonalAccount = false;

    [ObservableProperty]
    private ObservableCollection<Invitation> invitations = new();

    [ObservableProperty]
    private bool _hasInvitations;

    [ObservableProperty]
    private byte[]? userImage;

    [ObservableProperty]
    private bool isChangePasswordVisible = false;

    [ObservableProperty]
    private bool isReadOnlyData = false;

    [ObservableProperty]
    private string oldPassword = string.Empty;

    [ObservableProperty]
    private string newPassword = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private string saveSuccessMessage = string.Empty;

    [ObservableProperty]
    private bool isSaveSuccess;

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

        LoadInvitations();
    }

    private void LoadInvitations()
    {
        Invitations.Clear();
        if (User?.InvitationIdToUserNavigations != null)
        {
            foreach (var inv in User.InvitationIdToUserNavigations)
                Invitations.Add(inv);
        }
        HasInvitations = Invitations.Count > 0;
    }

    public void ClosePersonalAccountPanel()
    {
        IsVisiblePersonalAccount = false;
    }

    public void CloseMenu()
    {
        IsVisiblePersonalAccount = false;
    }

    private RelayCommand<Invitation> acceptInvitationCommand;
    public RelayCommand<Invitation> AcceptInvitationCommand
    {
        get
        {
            return acceptInvitationCommand ?? new RelayCommand<Invitation>(async (invitation) =>
            {
                if (invitation == null) return;
                invitation.IsDelete = true;
                await Task.Delay(400);
                await ConnectionToHub.Instance.AcceptInvitation(invitation.Id);
                await ViewModelStore.GetInstance().FillDataViewModels();
                Invitations.Remove(invitation);
                HasInvitations = Invitations.Count > 0;
            });
        }
    }

    private RelayCommand<Invitation> declineInvitationCommand;
    public RelayCommand<Invitation> DeclineInvitationCommand
    {
        get
        {
            return declineInvitationCommand ?? new RelayCommand<Invitation>(async (invitation) =>
            {
                if (invitation == null) return;
                invitation.IsDelete = true;
                await Task.Delay(400);
                await ConnectionToHub.Instance.DeclineInvitation(invitation.Id);
                Invitations.Remove(invitation);
                HasInvitations = Invitations.Count > 0;
            });
        }
    }

    private RelayCommand closeAndOpenlePersonalAccount;
    public RelayCommand CloseAndOpenPersonalAccount
    {
        get
        {
            return closeAndOpenlePersonalAccount ?? new RelayCommand(async () =>
            {
                IsVisiblePersonalAccount = IsVisiblePersonalAccount ? false : true;

                if (IsVisiblePersonalAccount)
                    LoadInvitations();
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
                SaveSuccessMessage = string.Empty;
                IsSaveSuccess = false;

                if (IsChangePasswordVisible && !string.IsNullOrEmpty(NewPassword))
                {
                    if (string.IsNullOrEmpty(OldPassword))
                    {
                        SaveSuccessMessage = "Введите старый пароль";
                        return;
                    }
                    if (NewPassword != ConfirmPassword)
                    {
                        SaveSuccessMessage = "Новые пароли не совпадают";
                        return;
                    }
                }

                try
                {
                    var updated = await APIHost.GetInstance().EditUser(User, OldPassword,
                        !string.IsNullOrEmpty(NewPassword) ? NewPassword : null,
                        !string.IsNullOrEmpty(NewPassword) ? ConfirmPassword : null);

                    AuthorizedUser.GetInstance().AuthUser = updated;
                    User = updated;

                    SaveSuccessMessage = "Успешно сохранено";
                    IsSaveSuccess = true;
                    IsChangePasswordVisible = false;
                    OldPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;
                }
                catch (UnauthorizedAccessException)
                {
                    SaveSuccessMessage = "Неверный пароль";
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка сохранения профиля: {ex.Message}");
                    SaveSuccessMessage = "Ошибка сохранения";
                }
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

   



    private RelayCommand toggleChangePassword;
    public RelayCommand ToggleChangePassword
    {
        get
        {
            return toggleChangePassword ?? new RelayCommand(() =>
            {
                IsChangePasswordVisible = !IsChangePasswordVisible;
                if (!IsChangePasswordVisible)
                {
                    OldPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;
                }
            });
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
