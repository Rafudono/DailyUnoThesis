
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;
using Windows.UI.Core;

namespace DailyUnoThesis.Presentation.ViewModel.NavigationClasses
{
   public class TaskPageControle: Base
    {
        private CoreDispatcher dispatcher;
        //public PageNavigation Navigation;
        private TaskPages TaskPages;
        private TaskTodayListPage TaskListPageToday;
        private TaskCompleteListPage TaskListPageComplete;
        private TaskOverdueListPage TaskOverdueListPage;
        private TaskListPageCategory TaskListPageCategory;

        private List<Category> categories;
        public List<Category> Categories
        {
            get => categories;
            set
            {
                categories = value;
                Signal();
            }
        }

        private Category selectedCategory;
        public Category SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
                Signal();
            }
        }

        private string categoryTitle;
        public string CategoryTitle
        {
            get => categoryTitle;
            set
            {
                categoryTitle = value;
                Signal();
            }
        }


        private static TaskPageControle instance;
        public static TaskPageControle GetInstance()
        {
            if (instance == null)
                instance = new TaskPageControle();
            return instance;
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

        private RelayCommand openCategoryList;
        public RelayCommand OpenCategoryList
        {
            get
            {
                return openCategoryList ?? new RelayCommand(async () =>
                {
                    await GetListCategotyPage();

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



        public TaskPageControle()
        {
            SelectedCategory = new();
        }


        public void GetLists()
        {
            TaskListPageToday = new();
            TaskListPageComplete = new();
            SelectedCategory = new();
            TaskOverdueListPage = new();
            TaskListPageCategory = new();
            GetCaterogy();
        }
      

        public async Task GetListPage()
        {
           
           await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                CurPageCategory = PageNavigation.GetInstance().CurPageCategory;
                await PageNavigation.GetInstance().TaskListPage.GetTaskCatPage();
            });
            
        }


        public async Task GetListTodayPage()
        {
           this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                CurPageCategory = TaskListPageToday;
                //await TaskListPageToday.GetTodayPage();
            });

        }


        public async Task GetListCompletePage()
        {
              this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                CurPageCategory = TaskListPageComplete;
                //await TaskListPageComplete.GetCompletePage();
            });

        }

        public async Task GetListOverduePage()
        {
              this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                CurPageCategory = TaskOverdueListPage;
                //await TaskListPageComplete.GetCompletePage();
            });

        }


        public async Task GetListCategotyPage()
        {

              this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                CurPageCategory = TaskListPageCategory;
                TaskListPageCategory.GetIdCategory(SelectedCategory.Id);
                //await TaskListPageComplete.GetCompletePage();
            });

        }

        public async Task CreateCategory()
        {
            Category category = new Category() { Title = CategoryTitle };
            await APIHost.GetInstance().CreateCategory(category);
            await GetCaterogy();
              this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                TaskPages.CloseCatBannerClass();
            });

        }

        public async Task GetCaterogy()
        {
            Categories = await APIHost.GetInstance().GetCategories();
            Categories = new List<Category>(Categories);
            this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var en = TaskListPageCategory.DataContext as TaskListControle;
                en?.GetCategories();
            });


            //TaskOverdueListPage = new();
            //TaskListPageCategory = new();

        }


        internal void SetControl(TaskPages pass)
        {
            //this.Navigation = PageNavigation.GetInstance().;
            if(TaskPages == null)   
            TaskPages = pass;
           
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
