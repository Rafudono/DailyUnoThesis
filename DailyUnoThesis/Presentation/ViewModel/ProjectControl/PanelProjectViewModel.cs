using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.Messaging.Internals;
using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using DailyUnoThesis.Presentation.View.Pages.Projects;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;
using Microsoft.UI.Dispatching;
using Newtonsoft.Json.Linq;
using Uno.Extensions.Navigation;
using Uno.Extensions.Navigation;
using Uno.Toolkit.UI;
using Windows.UI.Core;


namespace DailyUnoThesis.Presentation.ViewModel.ProjectControl;
public partial class PanelProjectViewModel : ObservableObject
{
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public readonly INavigator _navigator;
    private CoreDispatcher dispatcher;
    //public PageNavigation Navigation;
    private PanelProjects TaskPages;
    private TaskTodayListPage TaskListPageToday;
    private TaskCompleteListPage TaskListPageComplete;
    private TaskOverdueListPage TaskOverdueListPage;
    private TaskListPageCategory TaskListPageCategory;
    private ProjectFolderViewModel ProjectControle;

    public ObservableCollection<Category> Categories => CategoryService.Instance.Projects;
    public ObservableCollection<Category> ArchivalProjects => CategoryService.Instance.ArchivalProjects;

    

    [ObservableProperty]
    private Category selectedCategory;

    [ObservableProperty]
    private Progressstate selectedProgressStates;

    [ObservableProperty]
    private Progressstate selectedArchivalProgressStates;
    

    [ObservableProperty]
    private ObservableCollection<Progressstate> progressStates = new();
    [ObservableProperty]
    private ObservableCollection<Progressstate> fullProgressStates = new();
    [ObservableProperty]
    private ObservableCollection<Progressstate> archivalProgressStates = new();

    [ObservableProperty]
    private Category category;

    [ObservableProperty]
    private string selectedBaseCategory;

    [ObservableProperty]
    private string categoryTitle;

    private static TaskPageControle instance;
 
    [ObservableProperty]
    private Page curPageCategory;

    [ObservableProperty]
    private List<string> listNavigations;


    [ObservableProperty]
    private double splitViewCompactPaneLength;
    [ObservableProperty]
    private bool isSplitViewPaneOpen = false;
    [ObservableProperty]
    private bool isVisibleCat;

    [ObservableProperty]
    private bool isMenuOpen = true;

    [ObservableProperty]
    private bool isVisibilityTabBar = true;
    [ObservableProperty]
    private bool isVisibilityFrame = false;
    [ObservableProperty]
    private bool visibilityArchivalBar = false;

    [ObservableProperty]
    private bool isVisibleNewProjectMember = false;

    [ObservableProperty]
    private string searchText;

    [ObservableProperty]
    private UserSuggestionDto userSuggestion = new();

    [ObservableProperty]
    private ObservableCollection<UserSuggestionDto> suggestions = new();
    [ObservableProperty]
    private ObservableCollection<UserDto> projectParticipants = new ();
    [ObservableProperty]
    private UserDto selectedProjectParticipants = new();

    // Поле для отмены старых запросов
    private CancellationTokenSource? _searchCts;
    private bool SelectSearchUser = false;

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

                UserSuggestionDto userSuggestionDto = new UserSuggestionDto() { Text = query};
                var results = await APIHost.GetInstance().GetSuggestionsUser(userSuggestionDto);

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
        var selected = args.SelectedItem as UserSuggestionDto;
        if (selected != null)
        {
            SelectSearchUser = true;
            UserSuggestion.IdUser = selected.IdUser;
            UserSuggestion.NickName = selected.NickName;
            UserSuggestion.Email = selected.Email;
            UserSuggestion.Text = selected.NickName;
            SearchText = selected.NickName;
            //IsSearchFilter = true;
            await SubmitFilters();
        }
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

        if (SelectSearchUser)
        {
            SelectSearchUser = false;
            return;
        }
        string finalQuery = args.QueryText;
        UserSuggestion.IdUser = 0;
        UserSuggestion.Text = finalQuery;
        //IsSearchFilter = true;
        await SubmitFilters();
        //Missions =  await APIHost.GetInstance().GetSearchMission(finalQuery,0);
    }

    private async Task SubmitFilters()
    {
        //IsFilter = true;
        
        var users = await APIHost.GetInstance().GetProjectParticipants(UserSuggestion);
        if (users.Count > 1)
        {
            ProjectParticipants.Clear();
            ProjectParticipants.AddRange(users);
        }
        else if (users.Count == 1)
        {
            SelectedProjectParticipants = users[0];    
        }
        //ProjectsWithMissions = new(ProjectsWithMissions);
        //MissionSuggestion.UserId = 0;
    }

    public void CloseNewProjectMember()
    {
        IsVisibleNewProjectMember = false;
    }


    private RelayCommand addNewProjectMember;
    public RelayCommand AddNewProjectMember
    {
        get
        {
            return addNewProjectMember ?? new RelayCommand(async () =>
            {
                IsVisibleNewProjectMember = IsVisibleNewProjectMember ? false : true;

            }

            );

        }

    }

    private RelayCommand addProjectParticipants;
    public RelayCommand AddProjectParticipants
    {
        get
        {
            return addProjectParticipants ?? new RelayCommand(async () =>
            {

            }

            );

        }

    }

    


    public async void SelectedAndVisible(TabBar sender, TabBarSelectionChangedEventArgs args)
    {
        if (IsVisibilityTabBar == true)
        {
            return;
        }
        await Task.Delay(100);
        SelectedCategory = null;
        IsVisibilityFrame = false;
        IsVisibilityTabBar = true;
    }

    public void CollapsedStaticTabBar()
    {
        if (TaskPages?.TabBar?.Items != null)
        {
            foreach (var item in TaskPages.TabBar.Items.OfType<TabBarItem>().Where(s => s.IsSelected == true))
            {
                item.IsSelected = false;
            }
            TaskPages.TabBar.SelectedIndex = -1;
            TaskPages.TabBar.SelectedItem = null;
        }
    }


    [ObservableProperty]
    private string currentViewState;

    partial void OnCurrentViewStateChanged(string value)
    {
        if (CurrentViewState == "NarrowState")
        {
            IsMenuOpen = false;
            return;
        }
        IsMenuOpen = true;
    }

    private RelayCommand closeMenuCommand;
    public RelayCommand CloseMenuCommand
    {
        get
        {
            return closeMenuCommand ?? new RelayCommand(async () =>
            {

                IsMenuOpen = IsMenuOpen ? false : true;
            }

            );

        }

    }


    private RelayCommand createCat;

    public RelayCommand CreateCat
    {
        get
        {
            return createCat ?? new RelayCommand(async () =>
            {
                await CreateCategory();

            }

            );

        }

    }


    private RelayCommand closeAndOpenDetailedSplitView;
    public RelayCommand CloseAndOpenDetailedSplitView
    {
        get
        {
            return closeAndOpenDetailedSplitView ?? new RelayCommand(async () =>
            {
                IsSplitViewPaneOpen = IsSplitViewPaneOpen ? false : true;

            }

            );

        }

    }

    private RelayCommand creatNewTask;
    public RelayCommand CreatNewTask
    {
        get
        {
            return creatNewTask ?? new RelayCommand(async () =>
            {
                ViewModelStore.GetInstance().DetailedProject.CreatNewTaskoutside();
                IsSplitViewPaneOpen = IsSplitViewPaneOpen ? false : true;

            }

            );

        }

    }



    private RelayCommand<Category> openPanelEditCategory;
    public RelayCommand<Category> OpenPanelEditCategory
    {
        get
        {
            return openPanelEditCategory ?? new RelayCommand<Category>(async (category) =>
            {
                if (category != null)
                {
                    Category = category;

                    ProgressStates.Clear();
                    if (category.Progress < 100)
                    {
                        ProgressStates.AddRange(FullProgressStates.Where(s => s.Id != (int)ProgressStateEnum.Completed));
                    }
                    else
                        ProgressStates.AddRange(FullProgressStates);
                    if (category.Progressstates != null)
                        SelectedProgressStates = ProgressStates.FirstOrDefault(s => s.Id == category.Progressstates.Id);

                    //Category.IdUpCategory = category.Id;
                    IsVisibleCat = true;
                }

            }

            );

        }

    }


    private RelayCommand closeAndOpenAddCategoryPanel;
    public RelayCommand CloseAndOpenAddCategoryPanel
    {
        get
        {
            return closeAndOpenAddCategoryPanel ?? new RelayCommand(async () =>
            {
                Category = new() { IsProgect = true };
                ProgressStates.Clear() ;
                ProgressStates.AddRange(FullProgressStates.Where(s => s.Id != (int)ProgressStateEnum.Completed));

                SelectedProgressStates = ProgressStates.FirstOrDefault(s=>s.Id == (int)ProgressStateEnum.InProgress);
                IsVisibleCat = IsVisibleCat ? false : true;
            }

            );

        }

    }

    public void CloseCategoryPanel()
    {
        IsVisibleCat = false;
    }

    public void CloseMenu()
    {
        IsMenuOpen = false;
    }


    private RelayCommand<Category> deleteCategoryMoveToParent;
    public RelayCommand<Category> DeleteCategoryMoveToParent
    {
        get
        {
            return deleteCategoryMoveToParent ?? new RelayCommand<Category>(async (category) =>
            {
                if (category != null)
                {

                    await DeleteCategory(category, CategoryDeleteMode.MoveToParent);
                }
            });
        }
    }

    private RelayCommand<Category> deleteCategoryCascade;
    public RelayCommand<Category> DeleteCategoryCascade
    {
        get
        {
            return deleteCategoryCascade ?? new RelayCommand<Category>(async (category) =>
            {
                if (category != null)
                {
                    await DeleteCategory(category, CategoryDeleteMode.Cascade);
                }
            });
        }
    }

    private RelayCommand<Category> deleteCategoryOrphanTasks;
    public RelayCommand<Category> DeleteCategoryOrphanTasks
    {
        get
        {
            return deleteCategoryOrphanTasks ?? new RelayCommand<Category>(async (category) =>
            {
                if (category != null)
                {
                    await DeleteCategory(category, CategoryDeleteMode.OrphanTasks);
                }
            });
        }
    }

    private RelayCommand<Category> deleteCategoryEmptyOnly;
    public RelayCommand<Category> DeleteCategoryEmptyOnly
    {
        get
        {
            return deleteCategoryEmptyOnly ?? new RelayCommand<Category>(async (category) =>
            {
                if (category != null)
                {
                    await DeleteCategory(category, CategoryDeleteMode.EmptyOnlyDelete);
                }
            });
        }
    }

    private RelayCommand<Category> deleteCategoryWithMissions;
    public RelayCommand<Category> DeleteCategoryWithMissions
    {
        get
        {
            return deleteCategoryWithMissions ?? new RelayCommand<Category>(async (category) =>
            {
                if (category != null)
                {
                    await DeleteCategory(category, CategoryDeleteMode.DeleteWithMissions);
                }
            });
        }
    }

    private RelayCommand<Category> clearСategory;
    public RelayCommand<Category> СlearСategory
    {
        get
        {
            return clearСategory ?? new RelayCommand<Category>(async (category) =>
            {
                if (category != null)
                {
                    await APIHost.GetInstance().СlearСategory(category);
                }
            });
        }
    }

    private async Task DeleteCategory(Category category, CategoryDeleteMode deleteMode)
    {
        await APIHost.GetInstance().DeleteCategories(category.Id, deleteMode);
        await CategoryService.Instance.RefreshFromProjectsDatabaseAsync();
        await ViewModelStore.GetInstance().FillDataViewModels();
    }


    private RelayCommand<Category> createSubcategory;
    public RelayCommand<Category> CreateSubcategory
    {
        get
        {
            return createSubcategory ?? new RelayCommand<Category>(async (category) =>
            {
                if (category != null)
                {
                    Category = new() { IdUpCategory = category.Id, IsProgect = false };
                    //Category.IdUpCategory = category.Id;
                    IsVisibleCat = IsVisibleCat ? false : true;
                }

            }

            );

        }

    }


    private RelayCommand<Category> goToKanbanBoard;
    public RelayCommand<Category> GoToKanbanBoard
    {
        get
        {
            return goToKanbanBoard ?? new RelayCommand<Category>(async (category) =>
            {

                GoToKanban();
            }

            );

        }

    }


    private RelayCommand<Category> openingArchivalBar;
    public RelayCommand<Category> OpeningArchivalBar
    {
        get
        {
            return openingArchivalBar ?? new RelayCommand<Category>(async (category) =>
            {
                VisibilityArchivalBar = VisibilityArchivalBar ? false : true;
            }

            );

        }

    }

    

    private async Task GoToKanban()
    {
        await _navigator.NavigateViewModelAsync<KanbanBoardViewModel>(this);
    }


    public PanelProjectViewModel(INavigator navigator)
    {
        _navigator = navigator;
        SelectedCategory = new();

        IsSplitViewPaneOpen = false;
        IsVisibleCat = false;
    }

  

    partial void OnSelectedCategoryChanged(Category value)
    {
        if (value != null && value.Id != 0)
        {
            GoToProjectFolder(value);
        }
    }

    public void GoToProjectFolder(Category value)
    {
        if (ViewModelStore.GetInstance().ProjectFolder != null)
        {
            GoToSelectCategory(value);
            IsVisibilityTabBar = false;
            CollapsedStaticTabBar();
            IsVisibilityFrame = true;
            if (CurrentViewState == "NarrowState")
            {
                IsMenuOpen = false;
            }
        }
    }

    private async void GoToSelectCategory(Category value)
    {
        await ProjectControle.GetIdCategory(value);
        await ViewModelStore.GetInstance().DetailedProject.GetMainProject(value);
    }

    public async Task CreateCategory()
    {
        Category.IsProgect = true;
        if (Category.Id != 0)
        {
            Category.ProgressstatesId = SelectedProgressStates.Id;
            await APIHost.GetInstance().EditCategory(Category);

        }
        else
        {
            Category.ProgressstatesId = SelectedProgressStates.Id;
            //Category.ProgressstatesId = (int?)ProgressStateEnum.InProgress;
            await APIHost.GetInstance().CreateCategory(Category);

        }
        await CategoryService.Instance.RefreshFromProjectsDatabaseAsync();
        IsVisibleCat = false;
        Category = new();
    }

    public async Task GetCaterogy()
    {
        await CategoryService.Instance.RefreshFromProjectsDatabaseAsync();
        FullProgressStates = await APIHost.GetInstance().GetStatusCategories();
        ProgressStates.AddRange(FullProgressStates);
        ArchivalProgressStates.AddRange(FullProgressStates.Where(s=>s.Id != (int)ProgressStateEnum.InProgress));
        ArchivalProgressStates.Insert(0, new Progressstate { Id = 0, Title = "ВСЕ" });
        SelectedArchivalProgressStates = ArchivalProgressStates[0];

    }

    public async void SelectArchivalProgressStates(object sender, SelectionChangedEventArgs e)
    {

      await CategoryService.Instance.FilterArchivalProgressStates(SelectedArchivalProgressStates.Id);

    }



    public async void SetControl(PanelProjects pass)
    {
        if (TaskPages == null)
            TaskPages = pass;
        //SelectedBaseCategory = ListNavigations[0];
        TaskPages.framePage.Navigate(typeof(ProjectFolder));
        var vm = ViewModelStore.GetInstance().ProjectFolder;
        var del = TaskPages.framePageTask.Navigate(typeof(SelectedAndNewTask), false);
        
        ViewModelStore.GetInstance().DetailedProject.GetBoolProject(true);
        ProjectControle = vm;

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
