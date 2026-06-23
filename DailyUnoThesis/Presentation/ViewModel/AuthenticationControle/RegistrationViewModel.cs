using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DailyThesisAPI.SignalR;
using Windows.Storage;

namespace DailyUnoThesis.Presentation.ViewModel.AuthenticationControle;
public partial class RegistrationViewModel:ObservableObject
{
    [ObservableProperty]
    private string _username;
    [ObservableProperty]
    private string _email;
    [ObservableProperty]
    private string _password;
    [ObservableProperty]
    private string _confirmPassword;
    [ObservableProperty]
    private string _confirmEmail;
    [ObservableProperty]
    private string _errorMessage;
    [ObservableProperty]
    private Visibility _errorVisibility = Visibility.Collapsed;
    private readonly INavigator _navigator;

    public RegistrationViewModel(INavigator navigator)
    {
        _navigator = navigator;
    }
    public ICommand RegistrationStart => new RelayCommand(async ()=>
    {
        ErrorMessage = "";
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ErrorMessage = "🛈 Заполните все поля";
            ErrorVisibility = Visibility.Visible;
            return;
        }
        try
        {
            var addr = new System.Net.Mail.MailAddress(Email);
            if (addr.Address != Email) throw new Exception();
        }
        catch
        {
            ErrorMessage = "🛈 Некорректный email";
            ErrorVisibility = Visibility.Visible;
            return;
        }
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "🛈 Пароли не совпадают";
            ErrorVisibility = Visibility.Visible;
            return;
        }
        if (Password.Length < 6)
        {
            ErrorMessage = "🛈 Пароль должен быть не менее 6 символов";
            ErrorVisibility = Visibility.Visible;
            return;
        }
        if (!Regex.IsMatch(Password, @"[A-Z]"))
        {
            ErrorMessage = "🛈 Пароль должен содержать заглавную букву";
            ErrorVisibility = Visibility.Visible;
            return;
        }
        if (!Regex.IsMatch(Password, @"[a-z]"))
        {
            ErrorMessage = "🛈 Пароль должен содержать строчную букву";
            ErrorVisibility = Visibility.Visible;
            return;
        }
        if (!Regex.IsMatch(Password, @"\d"))
        {
            ErrorMessage = "🛈 Пароль должен содержать цифру";
            ErrorVisibility = Visibility.Visible;
            return;
        }
        if (!Regex.IsMatch(Password, @"[!@#$%^&*(),.?"":{}|<>]"))
        {
            ErrorMessage = "🛈 Пароль должен содержать спецсимвол";
            ErrorVisibility = Visibility.Visible;
            return;
        }

        if (!await APIHost.GetInstance().UsernameExist(Username))
        {
            var (isSucced, resp) =await APIHost.GetInstance().RegUser(Password, Email, Username);
            if (isSucced)
            {
                ApplicationData.Current.LocalSettings.Values["RefreshToken"] =
                    AuthorizedUser.GetInstance().RefreshToken;
                _ = ConnectionToHub.Instance.CreateConnection();
                await _navigator.NavigateRouteAsync(this, "Main");
            }
            else
                ErrorMessage = $"🛈 {resp}";
        }
        else
        {
            ErrorMessage = "🛈 Имя пользователя занято";
            ErrorVisibility = Visibility.Visible;
            return;
        }
    });
    public ICommand BackToLoginPage => new RelayCommand(async () =>
    {
        await _navigator.GoBack(this);
    });
}
