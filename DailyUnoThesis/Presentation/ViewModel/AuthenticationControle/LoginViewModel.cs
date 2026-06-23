using System;
using System.Collections.Generic;
using System.Text;
using DailyThesisAPI.SignalR;
using Uno.Extensions.Navigation;
using Windows.Storage;

namespace DailyUnoThesis.Presentation.ViewModel.AuthenticationControle;
public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string _username;
    [ObservableProperty]
    private string _password;
    [ObservableProperty]
    private string _errorMessage;
    [ObservableProperty]
    private Visibility _errorVisibility = Visibility.Collapsed;
    private readonly INavigator _navigator;

    public LoginViewModel(INavigator navigator)
    {
        _navigator = navigator;
    }

    public async Task TryAutoLogin()
    {
        var savedRefreshToken = ApplicationData.Current.LocalSettings.Values["RefreshToken"] as string;
        if (string.IsNullOrEmpty(savedRefreshToken)) return;

        AuthorizedUser.GetInstance().RefreshToken = savedRefreshToken;
        var success = await APIHost.GetInstance().RefreshToken();
        if (success)
        {
            _ = ConnectionToHub.Instance.CreateConnection();
            await _navigator.NavigateRouteAsync(this, "Main");
        }
    }

    public ICommand Login => new RelayCommand(async () =>
    {
        ErrorMessage = "";
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "🛈 Заполните все поля";
            ErrorVisibility = Visibility.Visible;
            return;
        }
        var (isSucced, resp)=await APIHost.GetInstance().AuthUser(Password, Username);
        if (isSucced)
        {
            ApplicationData.Current.LocalSettings.Values["RefreshToken"] =
                AuthorizedUser.GetInstance().RefreshToken;
            var tkn = ApplicationData.Current.LocalSettings.Values["RefreshToken"];
            _ = ConnectionToHub.Instance.CreateConnection();
            await _navigator.NavigateRouteAsync(this, "Main");
        }
        else
        {
            ErrorMessage = $"🛈 {resp}";
            ErrorVisibility = Visibility.Visible;
        }
    });
    public ICommand OpenRegistrationPage => new RelayCommand(async () =>
    {
        await _navigator.NavigateRouteAsync(this, "Registration");
    });
}
