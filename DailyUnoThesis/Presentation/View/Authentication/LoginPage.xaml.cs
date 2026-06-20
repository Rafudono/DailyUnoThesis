using DailyUnoThesis.Presentation.ViewModel.AuthenticationControle;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DailyUnoThesis.Presentation.View.Authentication;

public sealed partial class LoginPage : Page
{
    private LoginViewModel _viewModel;

    public LoginPage()
    {
        this.InitializeComponent();
        this.DataContextChanged += (s, args) =>
        {
            if (args.NewValue is LoginViewModel vm)
                _viewModel = vm;
        };
        this.Loaded += async (s, e) =>
        {
            await Task.Yield();
            if (_viewModel is not null)
                await _viewModel.TryAutoLogin();
        };
    }
}
