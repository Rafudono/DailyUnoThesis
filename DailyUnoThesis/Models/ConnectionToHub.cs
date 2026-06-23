using DailyUnoThesis.Models.DobleClasses;
using DailyUnoThesis.Models.MainClasses;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.UI.Dispatching;

namespace DailyThesisAPI.SignalR
{
    public class ConnectionToHub
    {
        private HubConnection? _connection;
        private static ConnectionToHub? instance;
        public static ConnectionToHub Instance => instance ?? (instance = new ConnectionToHub());
        private readonly DispatcherQueue _dispatcher = DispatcherQueue.GetForCurrentThread();

        public ConnectionToHub()
        {
        }

        public async Task CreateConnection()
        {
            var user = AuthorizedUser.GetInstance().AuthUser;
            if (user == null) return;

            _connection = new HubConnectionBuilder()
                    .WithUrl($"http://localhost:5114/dailyhub?userId={user.Id}", options =>
                    {
                        options.AccessTokenProvider = () =>
                            Task.FromResult<string?>(AuthorizedUser.GetInstance().AccessToken);
                    })
                    .WithAutomaticReconnect()
                    .Build();

            _connection.On("ReceiveInvitation", (int fromUserId, int projectId) =>
            {
                _ = _dispatcher.TryEnqueue(async () =>
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Приглашение в проект",
                        Content = $"Пользователь #{fromUserId} приглашает вас в проект #{projectId}",
                        CloseButtonText = "Закрыть"
                    };
                    await dialog.ShowAsync();
                });
            });

            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SignalR connection error: {ex.Message}");
            }
        }

        public async Task Disconnect()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        public async Task<string?> SendInvitation(int toUserId, int projectId)
        {
            if (_connection?.State == HubConnectionState.Connected)
            {
                var fromUserId = AuthorizedUser.GetInstance().AuthUser.Id;
                return await _connection.InvokeAsync<string?>("SendInvitation", fromUserId, toUserId, projectId);
            }
            return "Нет подключения к серверу";
        }

        public async Task<bool> AcceptInvitation(int invitationId)
        {
            if (_connection?.State == HubConnectionState.Connected)
                return await _connection.InvokeAsync<bool>("AcceptInvitation", invitationId);
            return false;
        }

        public async Task<bool> DeclineInvitation(int invitationId)
        {
            if (_connection?.State == HubConnectionState.Connected)
                return await _connection.InvokeAsync<bool>("DeclineInvitation", invitationId);
            return false;
        }
    }
}
