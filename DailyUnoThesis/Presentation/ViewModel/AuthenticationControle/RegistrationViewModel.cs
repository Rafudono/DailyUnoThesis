using System;
using System.Collections.Generic;
using System.Text;

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

        if (!await APIHost.GetInstance().UsernameExist(Username))
        {
            APIHost.GetInstance().RegUser(Password, Email, Username);
            await _navigator.NavigateRouteAsync(this, "Main");
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
