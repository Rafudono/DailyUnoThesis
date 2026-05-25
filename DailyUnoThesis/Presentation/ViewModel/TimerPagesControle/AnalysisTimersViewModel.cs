using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Models.MainClasses;
using DailyUnoThesis.Presentation.View.Timer;

namespace DailyUnoThesis.Presentation.ViewModel.TimerPagesControle
{
    public partial class AnalysisTimersViewModel : ObservableObject
    {
        [ObservableProperty]
        private AnalysisTimers page;
        [ObservableProperty]
        private TimeAnalyticsSummaryDto timeAnalyticsSummary = new();

        [ObservableProperty]
        private ObservableCollection<DailyWorkHoursDto> dailyWorkHoursData = new();
        [ObservableProperty]
        private ObservableCollection<TaskDistributionDto> taskDistributionData = new();
        [ObservableProperty]
        private TaskDeepAnalysisDto selectedTaskAnalysis = new();


        [ObservableProperty]
        public ObservableCollection<Missionstimer> missionstimers;
        [ObservableProperty]
        private Missionstimer selectedMissionstimer = new();

        [ObservableProperty]
        private ObservableCollection<DayTimelineDto> weeklyTimeline = new();


        [ObservableProperty]
        private bool isPaneOpen = false;


        public AnalysisTimersViewModel()
        {
            GetAnalysis();

        }

        private async Task GetAnalysis()
        {
            //List<Mission> missions = new List<Mission>();
            //missions = await APIHost.GetInstance().GetMissions();
            TimeAnalyticsSummary = await APIHost.GetInstance().GetSummaryTimers();
            Missionstimers = await APIHost.GetInstance().GetTitlesMissionsTimer();
           
            // 1. Вычисляем даты
            DateTime now = DateTime.Now;
            // Находим разницу между текущим днем и понедельником
            int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = now.AddDays(-1 * diff).Date; // Это будет Понедельник 00:00:00
            DateTime endOfWeek = startOfWeek.AddDays(6); // Это Воскресенье 23:59:59
            _currentWeekStart = startOfWeek;
            var dataFromApi = await APIHost.GetInstance().GetDailyWorkHours(startOfWeek, endOfWeek);
            var data = await APIHost.GetInstance().GetTaskDistribution(startOfWeek.AddDays(-7), endOfWeek);

            if (Missionstimers.FirstOrDefault(s => s.StartDate.Date == startOfWeek.AddDays(-7).Date) != null)
            {
                SelectedMissionstimer = Missionstimers.FirstOrDefault(s => s.StartDate.Date == startOfWeek.AddDays(-7).Date);
                FilterStartDate = startOfWeek.AddDays(-7);
                FilterEndDate = endOfWeek;

                await GetDeepAnalysis();
            }
            else
            {
                SelectedTaskAnalysis = ReturnNullDeepAnalysis();
            }

            await DisplayHorizontalChart(dataFromApi);
            await LoadTaskDistribution(data);
            await LoadTimelineData(startOfWeek.AddDays(-7));
            //await UpdateLists(missions);
            IsPaneOpen = false;
        }


        private TaskDeepAnalysisDto ReturnNullDeepAnalysis()
        {
            var result = new TaskDeepAnalysisDto
            {
                TaskName = "",
                TotalTimeFormatted = "00:00:00",
                GlobalTotalTimeFormatted = "00:00:00",
                GlobalPercentage = 0,
                LongestSession = "00:00:00",
                ShortestSession = "00:00:00",
                AverageSession = "00:00:00",
                ProductivityRatio = 0, 
                PeakTimeOfDay = "0",
                TotalSessions = 0,

                GlobalPercentageFormatted = $"0%",
                ProductivityRatioFormatted = $"0%",
            };

            return result;
        }



        public async Task GetDeepAnalysis()
        {
            if (FilterStartDate != null)
            {
                DateTime startOfWeek = FilterStartDate.Value.Date;
                DateTime endOfWeek;
                if (FilterEndDate != null)
                {
                    endOfWeek = FilterEndDate.Value.Date;
                }
                else
                    endOfWeek = FilterStartDate.Value.Date;

                if (SelectedMissionstimer == null || SelectedMissionstimer.TitleMission == null || SelectedMissionstimer.TitleMission == "")
                {
                    var contentDialog = new ContentDialog
                    {
                        Title = "Выберите задачу",
                        //Content = "This is a very important message.",
                        PrimaryButtonText = "OK",
                        XamlRoot = Page.XamlRoot
                    };
                    await contentDialog.ShowAsync();
                    return;
                }

                var selectedTaskAnalysis = await APIHost.GetInstance().GetTaskDeepAnalysis(SelectedMissionstimer, startOfWeek, endOfWeek);
                SelectedTaskAnalysis = selectedTaskAnalysis;
                OpenPanel();
            }
            else
            {
                var contentDialog = new ContentDialog
                {
                    Title = "Выберите диапазон дат",
                    //Content = "This is a very important message.",
                    PrimaryButtonText = "OK",
                    XamlRoot = Page.XamlRoot
                };
                await contentDialog.ShowAsync();
                return;
            }
        }

        public async Task LoadTaskDistribution(List<TaskDistributionDto> apiData)
        {

            TaskDistributionData.Clear();
            foreach (var item in apiData)
            {
                TaskDistributionData.Add(item);
            }
        }


        public async Task DisplayHorizontalChart(List<DailyWorkHoursDto> apiData)
        {
            if (apiData == null || !apiData.Any()) return;

            double maxSeconds = apiData.Max(d => d.RawWorkTime.TotalSeconds);
            if (maxSeconds == 0) maxSeconds = 1;
            if (maxSeconds < 28800) maxSeconds = 28800;

            // Указываем максимальную ширину, которую может занять столбик (например, 300 пикселей)
            double maxBarWidth = 500;


            //double maxSecondsHeight = apiData.Max(d => d.RawWorkTime.TotalSeconds);

            // Если за неделю 0 часов работы, ставим заглушку в 1 секунду, чтобы не делить на 0
            //if (maxSecondsHeight == 0) maxSecondsHeight = 1;

            // 2. Указываем желаемую высоту области графика (в пикселях)
            double chartAreaHeight = 180;

            DailyWorkHoursData.Clear();
            //List<DailyWorkHoursDto> list = new List<DailyWorkHoursDto>();   
            foreach (var item in apiData)
            {
                // Теперь это NormalizedWidth
                item.NormalizedWidth = (item.RawWorkTime.TotalSeconds / maxSeconds) * maxBarWidth;
                item.NormalizedHeight = (item.RawWorkTime.TotalSeconds / maxSeconds) * chartAreaHeight;
                DailyWorkHoursData.Add(item);
            }
            //DailyWorkHoursData.AddRange(list);
        }


        public async Task LoadTimelineData(DateTime from)
        {
            _currentWeekStartTimeline = from;
            OnPropertyChanged(nameof(WeekRangeDisplayTimeline));

            DateTime to = from.AddDays(6);

            var data = await APIHost.GetInstance().GetTimelineData(from, to);

            WeeklyTimeline.Clear();
            foreach (var item in data)
            {
                WeeklyTimeline.Add(item);
            }
            // ... в методе LoadTimelineData, после получения данных ...
           
        }


      

        public double SessionTopMargin { get; set; } // Расстояние от верха до начала сессии
        public double SessionHeight { get; set; }    // Высота блока сессии

       




        private DateTime _currentWeekStart;
        private DateTime _currentWeekStartTimeline;


        // Свойство для отображения диапазона в заголовке (например, "11 МАЯ - 17 МАЯ")
        public string WeekRangeDisplay => GetRangeWeekDates(_currentWeekStart);
        public string WeekRangeDisplayTimeline => GetRangeWeekDates(_currentWeekStartTimeline);


        private string GetRangeWeekDates(DateTime currentWeek)
        {
            if (currentWeek.Year == DateTime.Now.Year || currentWeek.Year == 0001)
               return $"{currentWeek:dd MMM} - {currentWeek.AddDays(6):dd MMM}".ToUpper();
            else
                return $"{currentWeek:dd MMM yyyy} - {currentWeek.AddDays(6):dd MMM yyyy}".ToUpper();

            //TaskDateSettings = SelectedDate.Value.ToString("d MMM yyyy");
        }
        public async Task LoadWeekData(DateTime startOfMonday)
        {
            _currentWeekStart = startOfMonday;
            OnPropertyChanged(nameof(WeekRangeDisplay));

            DateTime endOfSunday = _currentWeekStart.AddDays(6);

            var data = await APIHost.GetInstance().GetDailyWorkHours(_currentWeekStart, endOfSunday);

            // Рассчитываем NormalizedWidth и заполняем коллекцию
            DisplayHorizontalChart(data);
        }

        // Команды для кнопок
        //public async void MoveToPreviousWeek() => await LoadWeekData(_currentWeekStart.AddDays(-7));


        private RelayCommand findDeepAnalysis;
        public RelayCommand FindDeepAnalysis
        {
            get
            {
                return findDeepAnalysis ?? new RelayCommand(async () =>
                {

                    await GetDeepAnalysis();
                }
                );
            }
        }


        private RelayCommand moveToPreviousWeek;
        public RelayCommand MoveToPreviousWeek
        {
            get
            {
                return moveToPreviousWeek ?? new RelayCommand(async () =>
                {

                    await LoadWeekData(_currentWeekStart.AddDays(-7));
                }
                );
            }
        }
        //public async void MoveToNextWeek() => await LoadWeekData(_currentWeekStart.AddDays(7));

        private RelayCommand moveToNextWeek;
        public RelayCommand MoveToNextWeek
        {
            get
            {
                return moveToNextWeek ?? new RelayCommand(async () =>
                {

                    await LoadWeekData(_currentWeekStart.AddDays(7));
                }
                );
            }
        }
        //public async void MoveToCurrentWeek() => await LoadWeekData(GetStartOfCurrentWeek());

        private RelayCommand moveToCurrentWeek;
        public RelayCommand MoveToCurrentWeek
        {
            get
            {
                return moveToCurrentWeek ?? new RelayCommand(async () =>
                {

                    await LoadWeekData(GetStartOfCurrentWeek());
                }
                );
            }
        }



        private RelayCommand moveToPreviousWeekTimeline;
        public RelayCommand MoveToPreviousWeekTimeline
        {
            get
            {
                return moveToPreviousWeekTimeline ?? new RelayCommand(async () =>
                {

                    await LoadTimelineData(_currentWeekStartTimeline.AddDays(-7));
                }
                );
            }
        }

        private RelayCommand moveToNextWeekTimeline;
        public RelayCommand MoveToNextWeekTimeline
        {
            get
            {
                return moveToNextWeekTimeline ?? new RelayCommand(async () =>
                {

                    await LoadTimelineData(_currentWeekStartTimeline.AddDays(7));
                }
                );
            }
        }

        private RelayCommand moveToCurrentWeekTimeline;
        public RelayCommand MoveToCurrentWeekTimeline
        {
            get
            {
                return moveToCurrentWeekTimeline ?? new RelayCommand(async () =>
                {

                    await LoadTimelineData(GetStartOfCurrentWeek());
                }
                );
            }
        }

        private DateTime GetStartOfCurrentWeek()
        {
            DateTime now = DateTime.Now;
            int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
            return now.AddDays(-1 * diff).Date;
        }



        private RelayCommand openPanelSample;
        public RelayCommand OpenPanelSample
        {
            get
            {
                return openPanelSample ?? new RelayCommand(async () =>
                {
                    OpenPanel();                }
                );
            }
        }
        private void OpenPanel()
        {
            
           IsPaneOpen = IsPaneOpen ? false : true;
       
        }

        public void OnItemInvoked(object sender, ItemClickEventArgs e)
        {
            // e.ClickedItem — это ваш объект из коллекции (например, модель шаблона таймера)
            //var clickedItem = e.ClickedItem as Mission;

            if (e.ClickedItem != null)
            {
                //var time = SelectedTimer;
                OpenPanel();
            }
        }



        [ObservableProperty]
        private DateTimeOffset? filterStartDate;

        [ObservableProperty]
        private DateTimeOffset? filterEndDate;
        private bool isProcessing = false;


        public void OnCalendarDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (isProcessing) return;

            var selected = sender.SelectedDates;
            if (args.AddedDates.Count == 0)
            {

                if (FilterEndDate != null)
                    if (args.RemovedDates[0].Date == FilterEndDate.Value.Date)
                    {
                        sender.SelectedDates.Clear();
                        FilterStartDate = null;
                        FilterEndDate = null;
                        return;
                    }
            }

            if (selected.Count == 2)
            {
                var start = selected.Min();
                var end = selected.Max();

                isProcessing = true;

                for (var dt = start.AddDays(1); dt < end; dt = dt.AddDays(1))
                {
                    if (!sender.SelectedDates.Contains(dt))
                    {
                        sender.SelectedDates.Add(dt);
                    }
                }
                FilterStartDate = start.Date;
                FilterEndDate = end.Date;
                //FilterStartDate = start;
                //FilterEndDate = end;
                isProcessing = false;
                return;
            }
            else if (selected.Count == 1)
            {
                FilterEndDate = null;
                FilterStartDate = selected[0].Date;
                //FilterStartDate = null;
                //FilterEndDate = selected[0];
                return;
            }

            if (args.RemovedDates.Count > 0 || (selected.Count > 2 && args.AddedDates.Count > 0))
            {
                isProcessing = true;

                var nextDate = args.AddedDates.Count > 0
                               ? args.AddedDates[0]
                               : args.RemovedDates[0];

                sender.SelectedDates.Clear();

                isProcessing = false;
                if (args.RemovedDates.Count > 0)
                {
                    if (args.RemovedDates[0].Date == FilterStartDate.Value.Date)
                    {
                        sender.SelectedDates.Clear();
                        FilterStartDate = null;
                        FilterEndDate = null;
                    }
                }
                else
                    sender.SelectedDates.Add(nextDate);
                return;
            }

        }
        public void ClosePanel()
        {
            OpenPanel();
        }







        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private bool isSearchFilter = false;

        [ObservableProperty]
        private MissionSuggestionDto missionSuggestion = new();

        partial void OnSearchTextChanged(string value)
        {
            if (value.IsNullOrEmpty() && IsSearchFilter == true)
                SearchReset();
        }

        private async Task SearchReset()
        {
            MissionSuggestion.IdMission = 0;
            MissionSuggestion.Title = null;
            IsSearchFilter = false;
            await SubmitFilters();
        }

        private async Task SubmitFilters()
        {
            //IsFilter = true;
            if (MissionSuggestion.IsEmpty())
                Missionstimers = await APIHost.GetInstance().GetTitlesMissionsTimer();
            Missionstimers = await APIHost.GetInstance().GetSearchMissionTimer(MissionSuggestion);

            MissionSuggestion.UserId = 0;
        }


        [ObservableProperty]
        private ObservableCollection<MissionSuggestionDto> suggestions = new();

        // Поле для отмены старых запросов
        private CancellationTokenSource? _searchCts;
        private bool SelectSearchMission = false;

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
                    MissionSuggestionDto missionSuggestion = new MissionSuggestionDto() { Title = query, PageMode = PageMode.AllTasks };

                    //////////
                    var results = await APIHost.GetInstance().GetSuggestionsTimer(missionSuggestion);

                    Suggestions = results;
                }
                catch (OperationCanceledException)
                {
                    // Это нормально, просто пользователь печатает быстрее, чем работает интернет
                }
            }
        }

        public async Task OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var selected = args.SelectedItem as MissionSuggestionDto;
            SelectSearchMission = true;
            //MissionSuggestion.IdMission = selected.IdMission;
            MissionSuggestion.Title = selected.Title;
            MissionSuggestion.IsChosen = false;
            IsSearchFilter = true;
            await SubmitFilters();
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

            if (SelectSearchMission)
            {
                SelectSearchMission = false;
                return;
            }
            string finalQuery = args.QueryText;
            MissionSuggestion.IdMission = 0;
            MissionSuggestion.Title = finalQuery;
            MissionSuggestion.IsChosen = false;
            IsSearchFilter = true;
            await SubmitFilters();
            //Missions =  await APIHost.GetInstance().GetSearchMission(finalQuery,0);
        }


        internal void SetControl(AnalysisTimers pass)
        {
            //this.Navigation = PageNavigation.GetInstance().;
            Page = pass;
        }
    }
}
