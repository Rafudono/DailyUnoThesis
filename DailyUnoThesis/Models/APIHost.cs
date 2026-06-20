using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using DailyUnoThesis.Models.AuthModels;
using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Models.MainClasses;
using Microsoft.Extensions.Options;
using Windows.Storage;

namespace DailyUnoThesis.Models;

    public class APIHost
    {
        public APIHost()
        {
            client.BaseAddress = new Uri("http://localhost:5114/api/"); //5114
            options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true };
        }
        private static APIHost instance;
        public static APIHost GetInstance()
        {
            if (instance == null)
             instance = new APIHost();
                return instance;
        }
        HttpClient client = new HttpClient();
        JsonSerializerOptions options = new JsonSerializerOptions();
        public List<Category> Categories { get; set; }

        public List<Mission> Missions { get; set; }

        public List<Notification> Notifications { get; set; }

        public List<Tag> Tags { get; set; }

        public List<User> Users { get; set; }

    public List<TaskCompletionTime> Sessions { get; set; }




    //надоел безрорядок 

    #region User
    public async Task<List<User>> GetUsers()
    {
        var res = await client.GetAsync($"Users");
        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = "невозможно получить данные пользователей"
            };
            return null;
        }
        else
        {
            Users = await res.Content.ReadFromJsonAsync<List<User>>(options);
        }
        return Users;
    }
    public async Task<(bool,string)> AuthUser(string password, string emailOrusername)
    {
        try
        {
            AuthUserData userData = new AuthUserData() { EmailOrLogin = emailOrusername, Password = password };
            var arg = JsonSerializer.Serialize(userData, options);
            var res = await client.PostAsync($"Users/AuthUser", new StringContent(arg, Encoding.UTF8, "application/json"));
            if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return (false,"Неверный логин или пароль");
            else if(res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var authResponse = await res.Content.ReadFromJsonAsync<AuthResponse>(options);
                AuthorizedUser.GetInstance().AuthUser = authResponse.User;
                // Сохрани токен — он понадобится для всех следующих запросов
                AuthorizedUser.GetInstance().AccessToken = authResponse.AccessToken;
                AuthorizedUser.GetInstance().RefreshToken = authResponse.RefreshToken;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthorizedUser.GetInstance().AccessToken);
                return (true, "Успех");
            }
            else if(res.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                return (false, "Внутренняя ошибка сервера, обратитесь к администратору");
            else
                return (false, "Что-то пошло не так");
        }
        catch( Exception ex)
        {
            return (false, "Ошибка при подключении к серверу");
        }
    }
    public async Task<(bool,string)> RegUser(string password, string email, string username)
    {
        AuthUserData userData = new AuthUserData() { Email = email, Password = password, Login = username };
        var arg = JsonSerializer.Serialize(userData, options);
        var res = await client.PostAsync($"Users", new StringContent(arg, Encoding.UTF8, "application/json"));
        if (res.StatusCode == System.Net.HttpStatusCode.OK)
        { 
            var response = await res.Content.ReadFromJsonAsync<AuthResponse>(options);
            AuthorizedUser.GetInstance().AuthUser = response.User;
            AuthorizedUser.GetInstance().AccessToken = response.AccessToken;
            AuthorizedUser.GetInstance().RefreshToken = response.RefreshToken;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthorizedUser.GetInstance().AccessToken);
            return (true,"Успех");
        }
        else if (res.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            return (false, "Внутренняя ошибка сервера, обратитесь к администратору");
        else
            return (false, "Что-то пошло не так");
    }

    public async Task SendApproval(string Email)
    {
        var resp = await client.GetAsync($"Users/Approval?email={Email}");
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog() { Content = $"не удалось получить подтверждение \t  {Error} " };
        }
        else
            ;
    }

    public async Task<bool> UsernameExist(string username)
    {
        var resp = await client.GetAsync($"Users/UsernameExist?username={username}");
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog() { Content = $"не удалось получить подтверждение \t  {Error} " };
            return true;
        }
        else
            return await resp.Content.ReadFromJsonAsync<bool>(options);
    }
    public async Task<bool> RefreshToken()
    {
        var token = AuthorizedUser.GetInstance().RefreshToken;
        var res = await client.PostAsync($"Tokens/Refresh?request={Uri.EscapeDataString(token)}", null);
        if (!res.IsSuccessStatusCode) return false;
        var data = await res.Content.ReadFromJsonAsync<AuthResponse>(options);
        if (data == null) return false;
        AuthorizedUser.GetInstance().AccessToken = data.AccessToken;
        AuthorizedUser.GetInstance().RefreshToken = data.RefreshToken;
        AuthorizedUser.GetInstance().AuthUser = data.User;
        ApplicationData.Current.LocalSettings.Values["RefreshToken"] = data.RefreshToken;

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", data.AccessToken);
        return true;
    }
    #endregion

    #region Missions

    #region Get
    //получения списка всех задач пользователя (не забыть поменять, чтоб разные пользователи получали свои задачи)
    public async Task<List<Mission>> GetMissions()
    {
        var res = await client.GetAsync($"Missions?id={AuthorizedUser.GetInstance().AuthUser.Id}");
        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await res.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = $"не удалось получить миссии \t  {Error} "
            };
            var dialog = new ContentDialog { Title = "Ошибка", Content = Error, CloseButtonText = "Закрыть" };
            await dialog.ShowAsync();
        }
        else
            Missions = await res.Content.ReadFromJsonAsync<List<Mission>>(options);
        return Missions;
    }

    // получение списка заданий на сегодня (тоже не забыть указывать пользователя)
    public async Task<List<Mission>> GetTodayList()
    {
        var res = await client.GetAsync($"Missions/GetToday?id={AuthorizedUser.GetInstance().AuthUser.Id}");
        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await res.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = $"не удалось получить список заданий на сегодня \t  {Error} "
            };
        }
        else
            Missions = await res.Content.ReadFromJsonAsync<List<Mission>>(options);
        return Missions;
    }

    //  получение списка просроченных заданий 
    internal async Task<List<Mission>> GetOverdue()
    {
        int id = AuthorizedUser.GetInstance().AuthUser.Id;
       //int id = 1;
        var resp = await client.GetAsync($"Missions/GetOverdue?id={id}");
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog() { Content = $"не удалось overdue \t  {Error} " };
        }
        else
            Missions = await resp.Content.ReadFromJsonAsync<List<Mission>>(options);
        return Missions;
    }

    // получение списка выполненных заданий
    public async Task<List<Mission>> GetMCompleteList()
    {

        var res = await client.GetAsync($"Missions/GetComplete?id={AuthorizedUser.GetInstance().AuthUser.Id}");
        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await res.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = $"не удалось получить список заданий на сегодня \t  {Error} "
            };
        }
        else
            Missions = await res.Content.ReadFromJsonAsync<List<Mission>>(options);
        return Missions;
    }

    // получение списка задач категории (тоже надо передавать пльзователя)
    public async Task<List<Mission>> GetMissionCategiry(int id)
    {
        //int id = AuthorizedUser.GetInstance().AuthUser.Id;
        var req = JsonSerializer.Serialize(id, options);
        var resp = await client.GetAsync($"Missions/GetMissionCategiry?id={id}");
        //?id={AuthorizedUser.GetInstance().AuthUser.Id}
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = $"не удалось получить список категорий заданий \t  {Error} "
            };
        }
        else
            Missions = await resp.Content.ReadFromJsonAsync<List<Mission>>(options);
        return Missions;
    }

    
    // получение списка задач категории (тоже надо передавать пльзователя)
    public async Task<List<Mission>> GetMissionProject(int id)
    {
        //int id = AuthorizedUser.GetInstance().AuthUser.Id;
        var req = JsonSerializer.Serialize(id, options);
        var resp = await client.GetAsync($"Missions/GetMissionProject?id={id}");
        //?id={AuthorizedUser.GetInstance().AuthUser.Id}
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = $"не удалось получить список категорий заданий \t  {Error} "
            };
        }
        else
            Missions = await resp.Content.ReadFromJsonAsync<List<Mission>>(options);
        return Missions;
    }

    // получение изменённого или созданного задания
    public async Task<Mission> GetLastMission(/*int id, string title*/ Mission getMission)
    {

        var resp = await client.GetAsync($"Missions/GetLastMission?id={getMission.Id}&title={getMission.Title}");
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }
        else
        {
            var mission = await resp.Content.ReadFromJsonAsync<Mission>(options);
            //Mission mis = mission;
            mission.LevelUp = getMission.LevelUp;
            mission.IdUpMissionNavigation = getMission.IdUpMissionNavigation;
           
            return mission;
        }
        return new Mission();

    }

   


    #endregion

    #region Post

    //создание задачи
    public async Task CreateMission(Mission mission)
    {
        //mission.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        mission.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        mission.User = null;
        //mission.User = AuthorizedUser.GetInstance().AuthUser;
        var arg = JsonSerializer.Serialize(mission, options);
        var res = await client.PostAsync($"Missions", new StringContent(arg, Encoding.UTF8, "application/json"));
        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await res.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = $"не удалось создать миссию \t  {Error} "
            };
        }

    }

    // удаление задачи (надо поменять с post на delete)
    public async Task DeleteMission(Mission mission)
    {
        var arg = JsonSerializer.Serialize(mission, options);
        var res = await client.PostAsync($"Missions/DeletedMission", new StringContent(arg, Encoding.UTF8, "application/json"));
        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await res.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog() { Content = $"не удалось удалить task \t  {Error} " };
        }

    }

    //получение подсказки задач в строке поиска
    public async Task<ObservableCollection<MissionSuggestionDto>> GetSuggestions(MissionSuggestionDto missionSuggestion)
    {
        missionSuggestion.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        var arg = JsonSerializer.Serialize(missionSuggestion, options);
        var resp = await client.PostAsync($"Missions/SearchSuggestions", new StringContent(arg, Encoding.UTF8, "application/json"));

        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }
        else
        {
            var result = await resp.Content.ReadFromJsonAsync<ObservableCollection<MissionSuggestionDto>>(options);
            return result ?? new ObservableCollection<MissionSuggestionDto>();
        }

        return new ObservableCollection<MissionSuggestionDto>();
    }

    // получение фильтрованных задач
    public async Task<ObservableCollection<Mission>> GetSearchMission(MissionSuggestionDto missionSuggestion)
    {
        missionSuggestion.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        var arg = JsonSerializer.Serialize(missionSuggestion, options);
        var res = await client.PostAsync($"Missions/GetFilterMission", new StringContent(arg, Encoding.UTF8, "application/json"));

        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await res.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }
        else
        {
            var result = await res.Content.ReadFromJsonAsync<ObservableCollection<Mission>>(options);
            return result ?? new ObservableCollection<Mission>();
        }

        return new ObservableCollection<Mission>();
    }

    // получение фильтрованных задач проектов
    public async Task<ObservableCollection<Category>> GetSearchMissionProjects(MissionSuggestionDto missionSuggestion)
    {
        missionSuggestion.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        var arg = JsonSerializer.Serialize(missionSuggestion, options);
        var res = await client.PostAsync($"Missions/GetFilterProjects", new StringContent(arg, Encoding.UTF8, "application/json"));

        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await res.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }
        else
        {
            var result = await res.Content.ReadFromJsonAsync<ObservableCollection<Category>>(options);
            return result ?? new ObservableCollection<Category>();
        }

        return new ObservableCollection<Category>();
    }
    
    #endregion

    #region Put

    // редактирование задачи
    public async Task EditMission(Mission mission)
    {
        //mission.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        //mission.UserId = 1;
        //mission.User = AuthorizedUser.GetInstance().AuthUser;
        var arg = JsonSerializer.Serialize(mission, options);
        var resp = await client.PutAsync($"Missions", new StringContent(arg, Encoding.UTF8, "application/json"));
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog() { Content = $"не удалось редактировать миссию \t  {Error} " };
        }
    }


    public async Task NewCreateMission(Mission mission)
    {
        //mission.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        mission.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        //mission.User = AuthorizedUser.GetInstance().AuthUser;
        var arg = JsonSerializer.Serialize(mission, options);
        var res = await client.PostAsync($"Missions", new StringContent(arg, Encoding.UTF8, "application/json"));
        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await res.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = $"не удалось создать миссию \t  {Error} "
            };
        }

    }
    public async Task NewEditMission(Mission mission)
    {
        //mission.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        //mission.UserId = 1;
        //mission.User = AuthorizedUser.GetInstance().AuthUser;
        var arg = JsonSerializer.Serialize(mission, options);
        var resp = await client.PutAsync($"Missions", new StringContent(arg, Encoding.UTF8, "application/json"));
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog() { Content = $"не удалось редактировать миссию \t  {Error} " };
        }
    }
    #endregion

    #region Delete
    #endregion
    #endregion

    #region Categories

    #region Get
    // получение списка категорий
    internal async Task<ObservableCollection<Category>> GetCategories()
    {
        //int id = AuthorizedUser.GetInstance().AuthUser.Id;
        int id = AuthorizedUser.GetInstance().AuthUser.Id;
        ObservableCollection<Category> categories = new ObservableCollection<Category>();
        //var req = JsonSerializer.Serialize(id, options);
        var resp = await client.GetAsync($"Categories/GetMyCategory?id={id}");
        //?id={AuthorizedUser.GetInstance().AuthUser.Id}
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = $"не удалось получить категориии \t  {Error} "
            };
        }
        else
            categories = await resp.Content.ReadFromJsonAsync<ObservableCollection<Category>>(options);
        return categories;
    }

    
    internal async Task<ObservableCollection<Category>> GetProject()
    {
        //int id = AuthorizedUser.GetInstance().AuthUser.Id;
        int id = 1;
        ObservableCollection<Category> categories = new ObservableCollection<Category>();
        //var req = JsonSerializer.Serialize(id, options);
        var resp = await client.GetAsync($"Categories/GetMyProject?id={id}");
        //?id={AuthorizedUser.GetInstance().AuthUser.Id}
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = $"не удалось получить категориии \t  {Error} "
            };
        }
        else
            categories = await resp.Content.ReadFromJsonAsync<ObservableCollection<Category>>(options);
        return categories;
    }

    internal async Task<ObservableCollection<Category>> GetProjectsWithMissions(int id)
    {
        //int id = AuthorizedUser.GetInstance().AuthUser.Id;
        //int id = 1;
        ObservableCollection<Category> categories = new ObservableCollection<Category>();
        //var req = JsonSerializer.Serialize(id, options);
        var resp = await client.GetAsync($"Missions/GetMissionsProjects?id={id}");
        //?id={AuthorizedUser.GetInstance().AuthUser.Id}
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = $"не удалось получить категориии \t  {Error} "
            };
        }
        else
            categories = await resp.Content.ReadFromJsonAsync<ObservableCollection<Category>>(options);
        return categories;
    }

    public async Task<ObservableCollection<Category>> GetFiltersSubCategories(int id)
    {
        ObservableCollection<Category> categories = new ObservableCollection<Category>();
        var resp = await client.GetAsync($"Categories/GetSubCategory?id={AuthorizedUser.GetInstance().AuthUser.Id}&idCategory={id}");
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = $"не удалось получить категориии \t  {Error} "
            };
        }
        else
            categories = await resp.Content.ReadFromJsonAsync<ObservableCollection<Category>>(options);
        return categories;

    }
    #endregion

    #region Post

    //Создание категории
    internal async Task CreateCategory(Category category)
    {
        //mission.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        category.IdBigBoss = AuthorizedUser.GetInstance().AuthUser.Id;
        //mission.User = AuthorizedUser.GetInstance().AuthUser;
        var arg = JsonSerializer.Serialize(category, options);
        var res = await client.PostAsync($"Categories", new StringContent(arg, Encoding.UTF8, "application/json"));
        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await res.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog() { Content = $"не удалось создать категорию \t  {Error} " };
        }
    }

    //удаление всех задач в категории
    public async Task СlearСategory(Category category)
    {

        var arg = JsonSerializer.Serialize(category, options);
        var res = await client.PostAsync($"Categories/СlearСategory", new StringContent(arg, Encoding.UTF8, "application/json"));

        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await res.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }

    }
    #endregion

    #region Put

    public async Task EditCategory(Category category)
    {
        //mission.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        //mission.UserId = 1;
        //mission.User = AuthorizedUser.GetInstance().AuthUser;
        var arg = JsonSerializer.Serialize(category, options);
        var resp = await client.PutAsync($"Categories", new StringContent(arg, Encoding.UTF8, "application/json"));
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog() { Content = $"не удалось редактировать миссию \t  {Error} " };
        }
    }
    #endregion

    #region Delete

    // множественное удаление категорий
    public async Task DeleteCategories(int id, CategoryDeleteMode deleteMode)
    {
        //var arg = JsonSerializer.Serialize(category, options);
        var res = await client.DeleteAsync($"Categories/DeleteCategory/{id}?mode={deleteMode}");
        if (res.StatusCode != System.Net.HttpStatusCode.NoContent)
        {
            string Error = await res.Content.ReadAsStringAsync();
            Debug.WriteLine($"Ошибка удаления: {Error}");
        }

    }
    #endregion
    #endregion

    #region Timers

    public async Task<List<Focustimer>> GetMyFocusTimer()
    {
        //int id = AuthorizedUser.GetInstance().AuthUser.Id;
        List<Focustimer> focustimers = new List<Focustimer>();
        int id = AuthorizedUser.GetInstance().AuthUser.Id;
        var req = JsonSerializer.Serialize(id, options);
        var resp = await client.GetAsync($"Focustimers?id={id}");
        //?id={AuthorizedUser.GetInstance().AuthUser.Id}
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }
        else
            focustimers = await resp.Content.ReadFromJsonAsync<List<Focustimer>>(options);
        return focustimers;
    }
    internal async Task CreateFocustimer(Focustimer focustimer)
    {
        focustimer.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        //mission.User = AuthorizedUser.GetInstance().AuthUser;
        var arg = JsonSerializer.Serialize(focustimer, options);
        var res = await client.PostAsync($"Focustimers", new StringContent(arg, Encoding.UTF8, "application/json"));
        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await res.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }
    }

    public async Task EditFocustimer(Focustimer focustimer)
    {

        //mission.User = AuthorizedUser.GetInstance().AuthUser;
        var arg = JsonSerializer.Serialize(focustimer, options);
        var resp = await client.PutAsync($"Focustimers", new StringContent(arg, Encoding.UTF8, "application/json"));
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }
    }

    internal async Task DeleteTimer(int id)
    {
        //var arg = JsonSerializer.Serialize(id, options);
        var resp = await client.DeleteAsync($"Focustimers?id={id}");
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }
    }


    public async Task CreateMissionTimer(Missionstimer missionstimer)
    {
        //mission.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        missionstimer.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        //mission.User = AuthorizedUser.GetInstance().AuthUser;
        var arg = JsonSerializer.Serialize(missionstimer, options);
        var res = await client.PostAsync($"Missionstimers", new StringContent(arg, Encoding.UTF8, "application/json"));
        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await res.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }

    }


    public async Task<TimeAnalyticsSummaryDto> GetSummaryTimers()
    {
        //int id = AuthorizedUser.GetInstance().AuthUser.Id;
        //List<Focustimer> focustimers = new List<Focustimer>();
        int id = AuthorizedUser.GetInstance().AuthUser.Id;
        //var req = JsonSerializer.Serialize(id, options);
        var resp = await client.GetAsync($"Missionstimers/GetSummary/{id}");
        //?id={AuthorizedUser.GetInstance().AuthUser.Id}
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }
        else
        {
            var timeAnalyticsSummary = await resp.Content.ReadFromJsonAsync<TimeAnalyticsSummaryDto>(options);
            return timeAnalyticsSummary;
        }
        return new TimeAnalyticsSummaryDto();
    }

    public async Task<List<DailyWorkHoursDto>> GetDailyWorkHours(DateTime fromDate, DateTime toDate)
    {
        string from = fromDate.ToString("yyyy-MM-dd");
        string to = toDate.ToString("yyyy-MM-dd");
        //int id = AuthorizedUser.GetInstance().AuthUser.Id;
        //List<Focustimer> focustimers = new List<Focustimer>();
        int id = AuthorizedUser.GetInstance().AuthUser.Id;
        //var req = JsonSerializer.Serialize(id, options);
        var resp = await client.GetAsync($"Missionstimers/GetDailyWorkHours/{id}?fromDate={from}&toDate={to}");
        //?id={AuthorizedUser.GetInstance().AuthUser.Id}
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }
        else
        {
            var timeAnalyticsSummary = await resp.Content.ReadFromJsonAsync<List<DailyWorkHoursDto>>(options);
            return timeAnalyticsSummary;
        }
        return new  List<DailyWorkHoursDto>();
    }


    public async Task<List<DayTimelineDto>> GetTimelineData(DateTime fromDate, DateTime toDate)
    {
        string from = fromDate.ToString("yyyy-MM-dd");
        string to = toDate.ToString("yyyy-MM-dd");
        //int id = AuthorizedUser.GetInstance().AuthUser.Id;
        //List<Focustimer> focustimers = new List<Focustimer>();
        int id = AuthorizedUser.GetInstance().AuthUser.Id;
        //var req = JsonSerializer.Serialize(id, options);
        var resp = await client.GetAsync($"Missionstimers/GetWeeklyTimeline/{id}?fromDate={from}&toDate={to}");
        //?id={AuthorizedUser.GetInstance().AuthUser.Id}
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }
        else
        {
            var timeAnalyticsSummary = await resp.Content.ReadFromJsonAsync<List<DayTimelineDto>>(options);
            return timeAnalyticsSummary;
        }
        return new List<DayTimelineDto>();
    }


    public async Task<List<TaskDistributionDto>> GetTaskDistribution(DateTime fromDate, DateTime toDate)
    {
        string from = fromDate.ToString("yyyy-MM-dd");
        string to = toDate.ToString("yyyy-MM-dd");
        //int id = AuthorizedUser.GetInstance().AuthUser.Id;
        //List<Focustimer> focustimers = new List<Focustimer>();
        int id = AuthorizedUser.GetInstance().AuthUser.Id;
        //var req = JsonSerializer.Serialize(id, options);
        var resp = await client.GetAsync($"Missionstimers/GetTaskDistribution/{id}?fromDate={from}&toDate={to}");
        //?id={AuthorizedUser.GetInstance().AuthUser.Id}
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }
        else
        {
            var timeAnalyticsSummary = await resp.Content.ReadFromJsonAsync<List<TaskDistributionDto>>(options);
            return timeAnalyticsSummary;
        }
        return new List<TaskDistributionDto>();
    }


    public async Task<ObservableCollection<Missionstimer>> GetTitlesMissionsTimer()
    {

        var res = await client.GetAsync($"Missionstimers/GetTitlesMissionsTimer/{AuthorizedUser.GetInstance().AuthUser.Id}");
        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await res.Content.ReadAsStringAsync();

        }
        else
        {
            var missions = await res.Content.ReadFromJsonAsync<ObservableCollection<Missionstimer>>(options);
            return missions;
        }
        return new ObservableCollection<Missionstimer>();
    }


    public async Task<TaskDeepAnalysisDto> GetTaskDeepAnalysis(Missionstimer missionstimer, DateTime fromDate, DateTime toDate)
    {
        string from = fromDate.ToString("yyyy-MM-dd");
        string to = toDate.ToString("yyyy-MM-dd");
        int id = AuthorizedUser.GetInstance().AuthUser.Id;
        //var req = JsonSerializer.Serialize(id, options);
        var resp = await client.GetAsync($"Missionstimers/GetTaskDeepAnalysis/{id}?missionId={missionstimer.MissionId}&title={missionstimer.TitleMission}&fromDate={from}&toDate={to}");
        //?id={AuthorizedUser.GetInstance().AuthUser.Id}
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }
        else
        {
            var taskDeepAnalysisDto = await resp.Content.ReadFromJsonAsync<TaskDeepAnalysisDto>(options);
            return taskDeepAnalysisDto;
        }
        return new TaskDeepAnalysisDto();
    }




    public async Task<ObservableCollection<MissionSuggestionDto>> GetSuggestionsTimer(MissionSuggestionDto missionSuggestion)
    {
        missionSuggestion.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        var arg = JsonSerializer.Serialize(missionSuggestion, options);
        var resp = await client.PostAsync($"Missionstimers/SearchSuggestions", new StringContent(arg, Encoding.UTF8, "application/json"));

        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }
        else
        {
            var result = await resp.Content.ReadFromJsonAsync<ObservableCollection<MissionSuggestionDto>>(options);
            return result ?? new ObservableCollection<MissionSuggestionDto>();
        }

        return new ObservableCollection<MissionSuggestionDto>();
    }



    public async Task<ObservableCollection<Missionstimer>> GetSearchMissionTimer(MissionSuggestionDto missionSuggestion)
    {
        missionSuggestion.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
        var arg = JsonSerializer.Serialize(missionSuggestion, options);
        var res = await client.PostAsync($"Missionstimers/GetFilterMission", new StringContent(arg, Encoding.UTF8, "application/json"));

        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await res.Content.ReadAsStringAsync();
            //MessageBox.Show(Error);
        }
        else
        {
            var result = await res.Content.ReadFromJsonAsync<ObservableCollection<Missionstimer>>(options);
            return result ?? new ObservableCollection<Missionstimer>();
        }

        return new ObservableCollection<Missionstimer>();
    }
    #endregion

    #region Статусы
    
    public async Task<ObservableCollection<Progressstate>> GetStatusCategories()
    {

        var res = await client.GetAsync($"Categories/GetStatusCategories");
        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await res.Content.ReadAsStringAsync();

        }
        else
        {
            var missions = await res.Content.ReadFromJsonAsync<ObservableCollection<Progressstate>>(options);
            return missions;
        }
        return new ObservableCollection<Progressstate>();
    }
    #endregion

    #region Possibly Trash
    // пустой
    public async Task<List<Mission>> GetMyMissions()
    {
        int id = AuthorizedUser.GetInstance().AuthUser.Id;
        var req = JsonSerializer.Serialize(id, options);
        var resp = await client.GetAsync($"Missions/GetMyMissions?id={id}");
        //?id={AuthorizedUser.GetInstance().AuthUser.Id}
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string Error = await resp.Content.ReadAsStringAsync();
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = $"не удалось получить список заданий \t  {Error} "
            };
        }
        else
            Missions = await resp.Content.ReadFromJsonAsync<List<Mission>>(options);
        return Missions;
    }



  
    #endregion

    #region Sessions
    public async Task<List<TaskCompletionTime>> GetSessions()
    {
        var res = await client.GetAsync($"TaskCompletionTimes?id={AuthorizedUser.GetInstance().AuthUser.Id}");
        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = "невозможно получить данные сессий"
            };
            return null;
        }
        else
        {
            Sessions = await res.Content.ReadFromJsonAsync<List<TaskCompletionTime>>(options);
        }
        return Sessions;
    }

    #endregion



    //#region TaskCompletion

    //public async Task<List<TaskCompletionTime>> GetTaskCompletionTime()
    //{
    //    var res = await client.GetAsync($"TaskCompletionTimes?id={1}");
    //    if (res.StatusCode != System.Net.HttpStatusCode.OK)
    //    {
    //        string Error = await res.Content.ReadAsStringAsync();
    //        ContentDialog contentDialog = new ContentDialog()
    //        {
    //            Content = $"не удалось получить миссии \t  {Error} "
    //        };
    //        var dialog = new ContentDialog { Title = "Ошибка", Content = Error, CloseButtonText = "Закрыть" };
    //        await dialog.ShowAsync();
    //    }
    //    else
    //        Missions = await res.Content.ReadFromJsonAsync<List<Mission>>(options);
    //    return ;
    //}
    //#endregion



}
