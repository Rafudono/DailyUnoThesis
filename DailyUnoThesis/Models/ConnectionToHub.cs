
using DailyUnoThesis.Models.MainClasses;
using Microsoft.AspNetCore.SignalR.Client;

namespace DailyThesisAPI.SignalR
{
    public class ConnectionToHub
    {
        public HubConnection _connection;
        private static ConnectionToHub instance;
        public static ConnectionToHub Instance => instance ?? (instance = new ConnectionToHub());
        public ConnectionToHub()
        {

        }
        public async Task CreateConnection() // когда вхожу в акк
        {
            _connection = new HubConnectionBuilder().
                    WithUrl("http://localhost:5114")
                    .WithAutomaticReconnect()
                    .Build();

            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex) { }
        }

        //приглашение в проект
        public void SendInvitation(User selectedUser, Category category)
        {

        }
        public void RecieveInvitationResp(bool isAccepted)
        {

        }
    }
}
