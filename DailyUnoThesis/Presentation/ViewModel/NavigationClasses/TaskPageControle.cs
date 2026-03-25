
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging.Internals;
using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using DailyUnoThesis.Presentation.ViewModel.HelperClasses;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;
using Microsoft.UI.Dispatching;
using Newtonsoft.Json.Linq;
using Uno.Extensions.Navigation;
using Uno.Extensions.Navigation;
using Uno.Toolkit.UI;
using Windows.UI.Core;


namespace DailyUnoThesis.Presentation.ViewModel.NavigationClasses;

   public partial class TaskPageControle: ObservableObject
   {
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        public readonly INavigator _navigator;
        private CoreDispatcher dispatcher;
        //public PageNavigation Navigation;
        private TaskPages TaskPages;
        private TaskTodayListPage TaskListPageToday;
        private TaskCompleteListPage TaskListPageComplete;
        private TaskOverdueListPage TaskOverdueListPage;
        private TaskListPageCategory TaskListPageCategory;
        private TaskCategotyControle CategotyControle;

        //[ObservableProperty]
        //private ObservableCollection<Category> categories;
    public ObservableCollection<Category> Categories => CategoryService.Instance.NavCategories;
    //public List<Category> Categories
    //{
    //    get => categories;
    //    set
    //    {
    //        categories = value;
    //        Signal();
    //    }
    //}
    [ObservableProperty]
        private Category selectedCategory;
    [ObservableProperty]
    private Category category;
    //public Category SelectedCategory
    //{
    //    get => selectedCategory;
    //    set
    //    {
    //        selectedCategory = value;
    //        Signal();
    //    }
    //}


    [ObservableProperty]
        private string selectedBaseCategory;
        //public string SelectedBaseCategory
        //{
        //    get => selectedBaseCategory;
        //    set
        //    {
        //        selectedBaseCategory = value;
        //        Signal();
        //    }
        //}


        [ObservableProperty]
        private string categoryTitle;
        //public string CategoryTitle
        //{
        //    get => categoryTitle;
        //    set
        //    {
        //        categoryTitle = value;
        //        Signal();
        //    }
        //}


        private static TaskPageControle instance;
        //public static TaskPageControle GetInstance()
        //{
        //    if (instance == null)
        //        instance = new TaskPageControle();
        //    return instance;
        //}

        [ObservableProperty]
        private Page curPageCategory;
        //public Page CurPageCategory
        //{
        //    get => curPageCategory;
        //    set
        //    {
        //        curPageCategory = value;
        //        Signal();
        //    }
        //}

        [ObservableProperty]
        private List<string> listNavigations;







        [ObservableProperty]
        private SplitViewDisplayMode splitViewDisplayMode;
        [ObservableProperty]
        private double splitViewOpenPaneLength;
        [ObservableProperty]
        private double splitViewCompactPaneLength;
        [ObservableProperty]
        private bool isSplitViewPaneOpen;
        [ObservableProperty]
        private bool isVisibleCat;

    //public SplitViewDisplayMode SplitViewDisplayMode
    //{
    //    get => _splitViewDisplayMode;
    //    set => SetProperty(ref _splitViewDisplayMode, value);
    //}

    //public double SplitViewOpenPaneLength
    //{
    //    get => _splitViewOpenPaneLength;
    //    set => SetProperty(ref _splitViewOpenPaneLength, value);
    //}

    //public double SplitViewCompactPaneLength
    //{
    //    get => _splitViewCompactPaneLength;
    //    set => SetProperty(ref _splitViewCompactPaneLength, value);
    //}

    //public bool IsSplitViewPaneOpen
    //{
    //    get => _isSplitViewPaneOpen;
    //    set => SetProperty(ref _isSplitViewPaneOpen, value);
    //}

    // Это свойство будет связано с ColumnDefinition.Width




    [ObservableProperty]
        private GridLength dynamicColumnWidth;
        //public IState<GridLength> DynamicColumnWidth { get; private set; }










        private RelayCommand openTodayList;
        public RelayCommand OpenTodayList
        {
            get
            {
                return openTodayList ?? new RelayCommand(async () =>
                {
                    await GetListTodayPage();

                }

                );

            }

        }

        private RelayCommand openAllList;
        public RelayCommand OpenAllList
        {
            get
            {
                return openAllList ?? new RelayCommand(async () =>
                {
                    await GetListPage();

                }

                );

            }

        }


        private RelayCommand openCompleteList;  
        public RelayCommand OpenCompleteList
        {
            get
            {
                return openCompleteList ?? new RelayCommand(async () =>
                {
                    await GetListCompletePage();

                }

                );

            }

        }
        private RelayCommand openOverdueList;
        public RelayCommand OpenOverdueList
        {
            get
            {
                return openOverdueList ?? new RelayCommand(async () =>
                {
                    await GetListOverduePage();

                }

                );

            }

        }

        //private RelayCommand openCategoryList;
        //public RelayCommand OpenCategoryList
        //{
        //    get
        //    {
        //        return openCategoryList ?? new RelayCommand(async () =>
        //        {
        //            await GetListCategotyPage();

        //        }

        //        );

        //    }

        //}

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


    private RelayCommand closeAndOpenAddCategoryPanel;
    public RelayCommand CloseAndOpenAddCategoryPanel
    {
        get
        {
            return closeAndOpenAddCategoryPanel ?? new RelayCommand(async () =>
            {
                Category = new();
                IsVisibleCat = IsVisibleCat ? false : true;
            }

            );

        }

    }

    public void CloseCategoryPanel()
    {
        IsVisibleCat = false; 
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
                  await  DeleteCategory(category, CategoryDeleteMode.Cascade);
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
                  await  DeleteCategory(category, CategoryDeleteMode.OrphanTasks);
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
        await CategoryService.Instance.RefreshFromDatabaseAsync();
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
                    Category = new() {IdUpCategory = category.Id };
                    //Category.IdUpCategory = category.Id;
                    IsVisibleCat = IsVisibleCat ? false : true;
                }

            }

            );

        }

    }

    [ObservableProperty]
        private ObservableCollection<NavMenuItem> menuItemsNav;
        //public ObservableCollection<NavMenuItem> MenuItemsNav
        //{ get => menuItemsNav;
        //    set
        //    {
        //        menuItemsNav = value;
        //        Signal();
        //    }
        //}


        public TaskPageControle(/*INavigator navigator*/)
        {
            //_navigator = navigator;
            SelectedCategory = new();
            MenuItemsNav = new();

            ListNavigations = new()
            {
              "Все задания",
              "На сегодня",
              "Выполненные",
              "Просроченные",
            };

            //SelectedBaseCategory = ListNavigations[0];
            //GetListPage();

            //BuildMenu();
            SplitViewDisplayMode = SplitViewDisplayMode.Inline;
            SplitViewOpenPaneLength = 0;
            IsSplitViewPaneOpen = false;
            DynamicColumnWidth = 0;
            IsVisibleCat = false;





        //PropertyChanged += ChangedDynamicColumnWidth;
          //ViewModelStore.GetInstance().PanelTask = this;
        }

        private async void ChangedDynamicColumnWidth(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsSplitViewPaneOpen) || e.PropertyName == nameof(SplitViewOpenPaneLength))
            {
                
                //await Task.Delay(150);
                var newWidth = IsSplitViewPaneOpen ? new GridLength(SplitViewOpenPaneLength) : new GridLength(0);
                DynamicColumnWidth = newWidth;
                
            }
        }

    public void OnWindowSizeChanged(double newWindowWidth)
    {
        // 1. Рассчитываем пропорциональную ширину (60% от окна)
        double calculatedWidth = newWindowWidth * 0.4;

        // 2. Ограничиваем её лимитами [350, 550]
        // Math.Clamp(значение, минимум, максимум)
        SplitViewOpenPaneLength = Math.Clamp(calculatedWidth, 350, 550);

        //if (IsSplitViewPaneOpen == true)
        //{
            // Дополнительно: если всё окно меньше 350, 
            // ширина панели не должна превышать ширину окна
            if (SplitViewOpenPaneLength > newWindowWidth)
            {
                SplitViewOpenPaneLength = newWindowWidth;
            }
        //}
    }


    public ICommand SelectCategoryCommand => new AsyncRelayCommand<Category>(SelectCategory);

        private async Task SelectCategory(Category category)
        {
            if (category == null) return;
            if (ViewModelStore.GetInstance().PanelTask == null)
            {
                //TaskPages.framePage.Navigate(typeof(TaskListPageCategory));
            }
            if (ViewModelStore.GetInstance().Category != null)
            {

                var vm = ViewModelStore.GetInstance().Category;
                CategotyControle = vm;
                await CategotyControle.GetIdCategory(category.Id);
            }
            TaskPages.GridStatic.Visibility = Visibility.Collapsed;
            TaskPages.CollapsedStaticTabBar();
            TaskPages.framePage.Visibility = Visibility.Visible;


           
            //await _navigator.NavigateRouteAsync(this, "Category", data: category);
            //await _navigator.NavigateRouteAsync(this, "TaskContentRegion/Category", data: category);

        }


    //partial void OnSelectedBaseCategoryChanged(string value)
    //{
    //    //var selected = (sender as ListView)?.SelectedItem as string;
    //    if (value != null)
    //    {
    //        //await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
    //        //{
    //        switch (value)
    //        {
    //            case "Все задания":
    //                TaskPages.framePage.Navigate(typeof(TaskListPage));
    //                break;
    //            case "На сегодня":
    //                TaskPages.framePage.Navigate(typeof(TaskTodayListPage));
    //                break;
    //            case "Выполненные":
    //                TaskPages.framePage.Navigate(typeof(TaskCompleteListPage));
    //                break;
    //            case "Просроченные":
    //                TaskPages.framePage.Navigate(typeof(TaskOverdueListPage));
    //                break;
    //        }
    //        ////CurPageCategory = PageNavigation.GetInstance().CurPageCategory;
    //        ////await PageNavigation.GetInstance().TaskListPage.GetTaskCatPage();
    //        //CategoriesListView.SelectedItem = null; // Снимаем выделение с категорий
    //        //ContentFrame.Navigate(typeof(HomePage));
    //        //});
    //        // Ваша логика открытия
    //        //SelectedCategory
    //    }

    //}


    partial void OnSelectedCategoryChanged(Category value)
        {
            if (value != null && value.Id != 0)
            {
                //if (PageNavigation.GetInstance().PageCategory == null)
                //{
                //    //TaskPages.framePage.Navigate(typeof(TaskListPageCategory));

                //}
                if (ViewModelStore.GetInstance().Category != null)
                {
                    GoToSelectCategory(value);
                    TaskPages.GridStatic.Visibility = Visibility.Collapsed;
                    TaskPages.CollapsedStaticTabBar();
                    TaskPages.framePage.Visibility = Visibility.Visible;

                }
               

                //TaskPages.framePage.Navigate(typeof(TaskCategotyControle));
            }
        }

        private async void GoToSelectCategory(Category value)
        {
           await CategotyControle.GetIdCategory(value.Id);
        }


    public async Task BuildMenu()
    {
        //await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        //{
        // 1. Статические пункты
        MenuItemsNav.Add(new NavMenuItem("Все задания", "Accept", "TaskList"));
        MenuItemsNav.Add(new NavMenuItem("На сегодня", "Accept", "Today"));
        MenuItemsNav.Add(new NavMenuItem("Выполненные", "Accept", "Complete"));
        MenuItemsNav.Add(new NavMenuItem("Просроченные", "Accept", "Overdue"));

        // 2. Разделитель и заголовок (визуальный отступ за счет свойств шаблона)
        MenuItemsNav.Add(new NavMenuItem("", IsSeparator: true));
        MenuItemsNav.Add(new NavMenuItem("Категории", IsHeader: true));
        MenuItemsNav.Add(new NavMenuItem("", IsSeparator: true));
        //});

        // 3. Динамические категории из API
        //var categories = await api.GetCategories();
        //Categories = await APIHost.GetInstance().GetCategories();
        //await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        //{
        //    //_dispatcherQueue.TryEnqueue(() =>
        //    //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        //    //{
        //    Categories = new List<Category>(Categories);
        //    List<Category> categories = new();
        //    categories = Categories; //надо будет потом напрямик изменить
        //                             //categories.Add(new Category() { Title = "первый каталог", Id = 1, IdBigBoss = 1 });
        //                             //categories.Add(new Category() { Title = "второй каталог", Id = 2, IdBigBoss = 1 });
        //                             //categories.Add(new Category() { Title = "третий каталог", Id = 3, IdBigBoss = 1 });
        //    foreach (var cat in categories)
        //    {
        //        // Передаем весь объект Category в свойство Data
        //        MenuItemsNav.Add(new NavMenuItem(cat.Title, "Tag", $"Category", Data: cat));
        //    }
        //    //MenuItemsNav = new(MenuItemsNav);
        //});

    }



    public async void SelectorBar_SelectionChanged()
        { 
        
        
        }

        //public async void GetLists()
        //{
        //    TaskListPageToday = new();
        //    TaskListPageComplete = new();
        //    SelectedCategory = new();
        //    TaskOverdueListPage = new();
        //    TaskListPageCategory = new();
        //    await GetCaterogy();
        //}
      

        public async Task GetListPage()
        {
           
           await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                TaskPages.framePage.Navigate(typeof(TaskListPage));
                //await _navigator.NavigateViewModelAsync<TaskListControle>(this);
                //CurPageCategory = PageNavigation.GetInstance().CurPageCategory;
                //await PageNavigation.GetInstance().TaskListPage.GetTaskCatPage();

            });
            
        }


        public async Task GetListTodayPage()
        {
            //this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            // {
            //     CurPageCategory = TaskListPageToday;
            //     //await TaskListPageToday.GetTodayPage();
            // });

            await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                TaskPages.framePage.Navigate(typeof(TaskTodayListPage));
                //await _navigator.NavigateViewModelAsync<TaskListControle>(this);
                //CurPageCategory = PageNavigation.GetInstance().CurPageCategory;
                //await PageNavigation.GetInstance().TaskListPage.GetTaskCatPage();

            });

        }


        public async Task GetListCompletePage()
        {
            await  this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                CurPageCategory = TaskListPageComplete;
                //await TaskListPageComplete.GetCompletePage();
            });

        }

        public async Task GetListOverduePage()
        {
            await  this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                CurPageCategory = TaskOverdueListPage;
                //await TaskListPageComplete.GetCompletePage();
            });

        }


        ////public async Task GetListCategotyPage()
        ////{

        //      this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        //    {
        //        CurPageCategory = TaskListPageCategory;
        //        TaskListPageCategory.GetIdCategory(SelectedCategory.Id);
        //        //await TaskListPageComplete.GetCompletePage();
        //    });

        ////}

        public async Task CreateCategory()
        {
            //Category category = new Category() { Title = CategoryTitle };
            await APIHost.GetInstance().CreateCategory(Category);
        //await GetCaterogy();
        await CategoryService.Instance.RefreshFromDatabaseAsync();
        IsVisibleCat = false;
        Category = new();
            //Categories = new(Categories);
        //await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        //    {
        //        TaskPages.CloseCatBannerClass();
        //    });

        }

        public async Task GetCaterogy()
        {
        await CategoryService.Instance.RefreshFromDatabaseAsync();
        //Categories = await APIHost.GetInstance().GetCategories();
        //Categories = new ObservableCollection<Category>(Categories);


        //await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        //    {
        //        var en = TaskListPageCategory.DataContext as TaskListControle;
        //        en?.GetCategories();
        //    });


        //TaskOverdueListPage = new();
        //TaskListPageCategory = new();

    }


    public async void SetControl(TaskPages pass)
        {
            //this.Navigation = PageNavigation.GetInstance().;
            if(TaskPages == null)   
            TaskPages = pass;
            SelectedBaseCategory = ListNavigations[0];
        TaskPages.framePage.Navigate(typeof(TaskListPageCategory));
        var vm = ViewModelStore.GetInstance().Category;
            TaskPages.framePageTask.Navigate(typeof(SelectedAndNewTask));
            CategotyControle = vm;
        
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

    

