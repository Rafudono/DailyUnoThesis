using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;

namespace DailyUnoThesis.Presentation.ViewModel.HelperClasses
{
    public partial class  ViewModelStore : DependencyObject
    {

        public static ViewModelStore Instance { get; private set; }
        public static ViewModelStore GetInstance()
        {
            if (Instance == null)
            {
                Instance = new ViewModelStore();
            }
            return Instance;
        }

        public ViewModelStore()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                GetInstance();
            }
            
            
        }

        #region Свойства классов 
        // Главная панель навигации
        public static readonly DependencyProperty MainProperty =
            DependencyProperty.Register("Main", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));
        public PageNavigation Main
        {
            get => (PageNavigation)GetValue(MainProperty);
            set => SetValue(MainProperty, value);
        }

        // Раздел задач
        public static readonly DependencyProperty PanelTaskProperty =
          DependencyProperty.Register("PanelTask", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));
        public TaskPageControle PanelTask
        {
            get => (TaskPageControle)GetValue(PanelTaskProperty);
            set => SetValue(PanelTaskProperty, value);
        }

        // Все задачи
        public static readonly DependencyProperty AllTasksProperty =
          DependencyProperty.Register("AllTasks", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));
        public TaskListControle AllTasks
        {
            get => (TaskListControle)GetValue(AllTasksProperty);
            set => SetValue(AllTasksProperty, value);
        }

        // сегодня
        public static readonly DependencyProperty TodayTasksProperty =
          DependencyProperty.Register("TodayTasks", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));
        public TaskTodayControle TodayTasks
        {
            get => (TaskTodayControle)GetValue(TodayTasksProperty);
            set => SetValue(TodayTasksProperty, value);
        }


        // выполненые
        public static readonly DependencyProperty CompleteTasksProperty =
          DependencyProperty.Register("CompleteTasks", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));
        public TaskCompleteControle CompleteTasks
        {
            get => (TaskCompleteControle)GetValue(CompleteTasksProperty);
            set => SetValue(CompleteTasksProperty, value);
        }


        // просроченные
        public static readonly DependencyProperty OverdueTasksProperty =
          DependencyProperty.Register("OverdueTasks", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));
        public OverdueTaskControle OverdueTasks
        {
            get => (OverdueTaskControle)GetValue(OverdueTasksProperty);
            set => SetValue(OverdueTasksProperty, value);
        }


        

        // Меню подробностей задач
        public static readonly DependencyProperty DetailedTaskProperty =
          DependencyProperty.Register("DetailedTask", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));
        public TaskViewModel DetailedTask
        {
            get => (TaskViewModel)GetValue(DetailedTaskProperty);
            set => SetValue(DetailedTaskProperty, value);
        }

        // CategoryViewModel 
        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register("Category", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));

        public TaskCategotyControle Category
        {
            get => (TaskCategotyControle)GetValue(CategoryProperty);
            set => SetValue(CategoryProperty, value);
        }
        #endregion


        #region Методы для навигации 
        public async Task GetPageCategory(TaskListPageCategory pageCategory)
        {

            //PageCategory = pageCategory;
            Category = pageCategory.DataContext as TaskCategotyControle;
            if(Category != null)
            await Category.GetIdCategory(1);
        }


        public void GetPageTask(SelectedAndNewTask page)
        {
            //SelectedAndNewTask = page;
            DetailedTask = page.DataContext as TaskViewModel;
        }

        public void ChangeSelected(Mission mission)
        {
            if (DetailedTask != null && mission.Id != 0)
            {
                DetailedTask.GetTask(mission);

            }
        }

        public async Task FillDataViewModels()
        {
            await AllTasks.FillData();
            await Category.FillData();
        }
        #endregion

    }
}
