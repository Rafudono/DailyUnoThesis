using DailyUnoThesis.Models.MainClasses;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DailyUnoThesis.Models
{
    public class APIHost
    {
        public APIHost()
        {
            client.BaseAddress = new Uri("http://localhost:5114/api/");
            options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
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
        public async Task<bool> AuthUser(string password, string emailOrusername)
        {
            AuthUserData userData = new AuthUserData() { EmailOrLogin = emailOrusername, Password= password}; 
            var arg = JsonSerializer.Serialize(userData, options);
            var res = await client.PostAsync($"Users/AuthUser", new StringContent(arg, Encoding.UTF8, "application/json"));
            if (res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Content = "не удалось авторизоваться \t  {Error}"
                };
                return false;
            }
            else
            {
                var user = await res.Content.ReadFromJsonAsync<User>(options);
                AuthorizedUser.GetInstance().AuthUser = user;
                return true;
            }
        }
        public async Task<bool> RegUser(string password, string email, string username)
        {
            AuthUserData userData = new AuthUserData() { Email = email, Password = password, Login = username };
            var arg = JsonSerializer.Serialize(userData, options);
            var res = await client.PostAsync($"Users", new StringContent(arg, Encoding.UTF8, "application/json"));
            if (res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string Error = await res.Content.ReadAsStringAsync();
                ContentDialog contentDialog = new ContentDialog()
                {
                    Content = $"не удалось зарегистрироваться \t  {Error} "
                };
                return false;
            }
            else
            {
                var user = await res.Content.ReadFromJsonAsync<User>(options);
                AuthorizedUser.GetInstance().AuthUser = user;
                return true;
            }
        }
        public async Task<List<Mission>> GetMissions()
        {

            var res = await client.GetAsync($"Missions?id={1}");
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

        public async Task<List<Mission>> GetTodayList()
        {

            var res = await client.GetAsync($"Missions/GetToday");
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


        public async Task<List<Mission>> GetMCompleteList()
        {

            var res = await client.GetAsync($"Missions/GetComplete");
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


        public async Task<List<Mission>> GetMyMissions()
        {
            int id = AuthorizedUser.GetInstance().AuthUser.Id;
            var req=JsonSerializer.Serialize(id, options);
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


        public async Task CreateMission(Mission mission)
        {
            //mission.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
            mission.UserId = 1;
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

        public async Task EditMission(Mission mission)
        {
            //mission.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
            mission.UserId = 1;
            //mission.User = AuthorizedUser.GetInstance().AuthUser;
            var arg = JsonSerializer.Serialize(mission, options);
            var resp = await client.PutAsync($"Missions", new StringContent (arg, Encoding.UTF8, "application/json"));
            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string Error = await resp.Content.ReadAsStringAsync();
                ContentDialog contentDialog = new ContentDialog() { Content = $"не удалось редактировать миссию \t  {Error} " };
            }
        }

        internal async Task<List<Category>> GetCategories()
        {
            //int id = AuthorizedUser.GetInstance().AuthUser.Id;
            int id = 1;
            List<Category> categories = new List<Category>();
            var req = JsonSerializer.Serialize(id, options);
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
                categories = await resp.Content.ReadFromJsonAsync<List<Category>>(options);
            return categories;
        }

        internal async Task CreateCategory(Category category)
        {
            //mission.UserId = AuthorizedUser.GetInstance().AuthUser.Id;
            category.IdBigBoss = 1;
            //mission.User = AuthorizedUser.GetInstance().AuthUser;
            var arg = JsonSerializer.Serialize(category, options);
            var res = await client.PostAsync($"Categories", new StringContent(arg, Encoding.UTF8, "application/json"));
            if (res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string Error = await res.Content.ReadAsStringAsync();
                ContentDialog contentDialog = new ContentDialog() { Content = $"не удалось создать категорию \t  {Error} " };
            }
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

        internal async Task<List<Mission>> GetOverdue()
        {

            int id = 1;
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
    }
}
