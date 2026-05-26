using System;
using System.Collections.Generic;
using System.Text;
using Uno.Extensions.Navigation;

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
    public ICommand Login => new RelayCommand(async () =>
    {
        ErrorMessage = "";
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "🛈 Заполните все поля";
            ErrorVisibility = Visibility.Visible;
            return;
        }
        if (await APIHost.GetInstance().AuthUser(Username, Password))
        {
            await _navigator.NavigateRouteAsync(this, "Main");
        }
        else
        {
            ErrorMessage = "🛈 Неверный логин или пароль";
            ErrorVisibility = Visibility.Visible;
        }
    });
    public ICommand OpenRegistrationPage => new RelayCommand(async () =>
    {
        await _navigator.NavigateRouteAsync(this, "Registration");
    });
}
