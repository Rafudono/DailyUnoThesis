using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Pages;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;
using DailyUnoThesis.Presentation.ViewModel.ProjectControl;
using DailyUnoThesis.Presentation.ViewModel.TimerPagesControle;
using DailyUnoThesis.Presentation.ViewModel.CalendarControls;
using DailyUnoThesis.Presentation.ViewModel.DashboardControl;

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
        public static readonly DependencyProperty MainPanel =
            DependencyProperty.Register("Main", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));
        public PageNavigation Main
        {
            get => (PageNavigation)GetValue(MainPanel);
            set => SetValue(MainPanel, value);
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

        // Меню подробностей проектов
        public static readonly DependencyProperty DetailedProjectProperty =
          DependencyProperty.Register("DetailedProject", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));
        public TaskViewModel DetailedProject
        {
            get => (TaskViewModel)GetValue(DetailedProjectProperty);
            set => SetValue(DetailedProjectProperty, value);
        }

        // CategoryViewModel 
        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register("Category", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));

        public TaskCategotyControle Category
        {
            get => (TaskCategotyControle)GetValue(CategoryProperty);
            set => SetValue(CategoryProperty, value);
        }







        // Панель проектов
        public static readonly DependencyProperty PanelProjectProperty =
            DependencyProperty.Register("PanelProject", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));

        public PanelProjectViewModel PanelProject
        {
            get => (PanelProjectViewModel)GetValue(PanelProjectProperty);
            set => SetValue(PanelProjectProperty, value);
        }

        
        //список задач в проекте 
        public static readonly DependencyProperty ProjectFolderProperty =
           DependencyProperty.Register("ProjectFolder", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));

        public ProjectFolderViewModel ProjectFolder
        {
            get => (ProjectFolderViewModel)GetValue(ProjectFolderProperty);
            set => SetValue(ProjectFolderProperty, value);
        }



        // pomodoro 
        public static readonly DependencyProperty PomodoroProperty =
            DependencyProperty.Register("PomodoroTime", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));

        public PomodoroTimerControle PomodoroTime
        {
            get => (PomodoroTimerControle)GetValue(PomodoroProperty);
            set => SetValue(PomodoroProperty, value);
        }


        // Обычный таймер  
        public static readonly DependencyProperty RegularTimerProperty =
            DependencyProperty.Register("RegularTimer", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));

        public RegularTimerControle RegularTimer
        {
            get => (RegularTimerControle)GetValue(RegularTimerProperty);
            set => SetValue(RegularTimerProperty, value);
        }

        // Календарь (TableCalendar)
        public static readonly DependencyProperty CalendarViewModelProperty =
            DependencyProperty.Register("CalendarViewModel", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));
        public TableCalendarViewModel CalendarViewModel
        {
            get => (TableCalendarViewModel)GetValue(CalendarViewModelProperty);
            set => SetValue(CalendarViewModelProperty, value);
        }


        // Панель главная
        public static readonly DependencyProperty PanelDashboardProperty =
            DependencyProperty.Register("PanelDashboard", typeof(object), typeof(ViewModelStore), new PropertyMetadata(null));

        public DashboardViewModel PanelDashboard
        {
            get => (DashboardViewModel)GetValue(PanelProjectProperty);
            set => SetValue(PanelProjectProperty, value);
        }
        #endregion


        #region Методы для навигации 
        public async Task GetPageCategory(TaskListPageCategory pageCategory)
        {

            //PageCategory = pageCategory;
            Category = pageCategory.DataContext as TaskCategotyControle;
            if(Category != null)
            await Category.GetIdCategory(CategoryService.Instance.FilterCategories[1]);
        }


        public void GetPageTask()
        {
            //SelectedAndNewTask = page;
            DetailedTask = new TaskViewModel();
        }
        public void GetPageProject()
        {
            //SelectedAndNewTask = page;
            DetailedProject = new TaskViewModel();
        }

        public void ChangeSelected(Mission mission)
        {
            if (DetailedTask != null && mission.Id != 0)
            {
                DetailedTask.GetTask(mission);

            }
        }

        public void ChangeSelectedProject(Mission mission)
        {
            if (DetailedProject != null && mission.Id != 0)
            {
                DetailedProject.GetTask(mission);

            }
        }

        public async Task FillDataViewModels()
        {
            await AllTasks.FillingDuringUpdate();
            await Category.FillingDuringUpdate();
            if(TodayTasks != null)
                await TodayTasks.FillingDuringUpdate();
            if (CompleteTasks != null)
                    await CompleteTasks.FillingDuringUpdate();
            if (OverdueTasks != null)
                        await OverdueTasks.FillingDuringUpdate();
            if (ProjectFolder != null)
                await ProjectFolder.FillingDuringUpdate();

        }
        #endregion

    }
}
