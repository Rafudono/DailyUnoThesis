using System; // Для IntPtr
using System.Threading.Tasks;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Authentication;
using DailyUnoThesis.Presentation.View.Calendar;
using DailyUnoThesis.Presentation.View.Dashboard;
using DailyUnoThesis.Presentation.View.Pages;
using DailyUnoThesis.Presentation.View.Pages.Other; // Для Task
using DailyUnoThesis.Presentation.View.Pages.Projects;
using DailyUnoThesis.Presentation.View.Timer;
using DailyUnoThesis.Presentation.ViewModel.AuthenticationControle;
using DailyUnoThesis.Presentation.ViewModel.CalendarControls;
using DailyUnoThesis.Presentation.ViewModel.DashboardControl;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;
using DailyUnoThesis.Presentation.ViewModel.PagesControls;
using DailyUnoThesis.Presentation.ViewModel.ProjectControl;
using DailyUnoThesis.Presentation.ViewModel.TimerPagesControle;
using Microsoft.UI.Dispatching; // Для DispatcherQueue
using Microsoft.UI.Xaml;
using Windows.Graphics;
using Windows.UI;
using Windows.UI.Core; // Для CoreWindow, если нужно

namespace DailyUnoThesis;
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    // Configure log levels for different categories of logging
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)

                        // Default filters for core Uno Platform namespaces
                        .CoreLogLevel(LogLevel.Warning);

                    // Uno Platform namespace filter groups
                    // Uncomment individual methods to see more detailed logging
                    //// Generic Xaml events
                    //logBuilder.XamlLogLevel(LogLevel.Debug);
                    //// Layout specific messages
                    //logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
                    //// Storage messages
                    //logBuilder.StorageLogLevel(LogLevel.Debug);
                    //// Binding related messages
                    //logBuilder.XamlBindingLogLevel(LogLevel.Debug);
                    //// Binder memory references tracking
                    //logBuilder.BinderMemoryReferenceLogLevel(LogLevel.Debug);
                    //// DevServer and HotReload related
                    //logBuilder.HotReloadCoreLogLevel(LogLevel.Information);
                    //// Debug JS interop
                    //logBuilder.WebAssemblyLogLevel(LogLevel.Debug);

                }, enableUnoLogging: true)
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        .EmbeddedSource<App>()
                        .Section<AppConfig>()
                )
                // Enable localization (see appsettings.json for supported languages)
                .UseLocalization()
                .UseHttp((context, services) =>
                {
#if DEBUG
                    // DelegatingHandler will be automatically injected
                    services.AddTransient<DelegatingHandler, DebugHttpHandler>();
#endif

                })
                .ConfigureServices((context, services) =>
                {
                    // TODO: Register your services
                    //services.AddSingleton<IMyService, MyService>();
                })
                .UseNavigation(RegisterRoutes)
            );

        var view = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
        view.TryResizeView(new Windows.Foundation.Size(1200, 700));


        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();
        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            //раздер задач
            new ViewMap(ViewModel: typeof(ShellViewModel)),
            new ViewMap<MainPage, PageNavigation>(),
            new ViewMap<TaskPages, TaskPageControle>(),
            new ViewMap<TaskCompleteListPage, TaskCompleteControle>(),
            new ViewMap<TaskListPage, TaskListControle>(),
            new ViewMap<TaskOverdueListPage, OverdueTaskControle>(),
            new ViewMap<TaskTodayListPage, TaskTodayControle>(),
            //new DataViewMap<TaskListPageCategory, TaskCategotyControle, Category>(),
            new ViewMap<TaskListPageCategory, TaskCategotyControle>(),
            new ViewMap<SelectedAndNewTask, TaskViewModel>(),

            //авторизация
            new ViewMap<LoginPage, LoginViewModel>(),
            new ViewMap<RegistrationPage, RegistrationViewModel>(),

            //раздел проектов
            new ViewMap<PanelProjects, PanelProjectViewModel>(),
            new ViewMap<ProjectFolder, ProjectFolderViewModel>(),

            // управляющий раздел
            new ViewMap<DashboardPage, DashboardViewModel>(),

            
            new ViewMap<KanbanBoard, KanbanBoardViewModel>(),

            // раздел таймеров
            new ViewMap<PanelTimer, PanelTimerControle>(),
            new ViewMap<Pomodoro, PomodoroTimerControle>(),
            new ViewMap<RegularTimer, RegularTimerControle>(),
            new ViewMap<AnalysisTimers, AnalysisTimersViewModel>(),


            // new ViewMap<TableCalendar, TableCalendarControle>(),
            new ViewMap<TableCalendar, MonthCalendarViewModel>(),

            new DataViewMap<SecondPage, SecondViewModel, Entity>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellViewModel>(),
                Nested: new RouteMap[]
                {
                   new("Login", View: views.FindByViewModel<LoginViewModel>(), IsDefault:true),
                   new("Registration", View: views.FindByViewModel<RegistrationViewModel>()),
                   new ("Main", View: views.FindByViewModel<PageNavigation>(), IsDefault:false,
                    
                     Nested: new RouteMap[] // Просто используем массив
                     {
                         new("TaskPage", View: views.FindByViewModel<TaskPageControle>()/*, IsDefault:true*/,
                         Nested: new RouteMap[]
                         {
                              new ("Second", View: views.FindByViewModel<SecondViewModel>()),
                              new ("Complete", View: views.FindByViewModel<TaskCompleteControle>()),
                              new ("TaskList", View: views.FindByViewModel<TaskListControle>()),
                              new ("Overdue", View: views.FindByViewModel<OverdueTaskControle>()),
                              new ("Today", View: views.FindByViewModel<TaskTodayControle>()),
                              new ("Category", View: views.FindByViewModel<TaskCategotyControle>()),

                               new ("Task", View: views.FindByViewModel<TaskViewModel>()),
                                new ("Kanban", View: views.FindByViewModel<KanbanBoardViewModel>()),
                         }),
                         new("PanelTimers", View: views.FindByViewModel<PanelTimerControle>(), 
                         Nested: new RouteMap[]
                         {
                              new ("Pomodoro", View: views.FindByViewModel<PomodoroTimerControle>()),
                              new ("Regular", View: views.FindByViewModel<RegularTimerControle>()),
                              new ("AnalysisTime", View: views.FindByViewModel<AnalysisTimersViewModel>()),
                         }),

                          new("PanelProject", View: views.FindByViewModel<PanelProjectViewModel>(),
                         Nested: new RouteMap[]
                         {
                         }),

                            new("Dashboard", View: views.FindByViewModel<DashboardViewModel>(),
                         Nested: new RouteMap[]
                         {
                         }),

                         new("PanelCalendar", View: views.FindByViewModel<MonthCalendarViewModel>()),
                     }),

                }
            )
        );
    }
}
