using System;
using System.Collections.Generic;
using System.Text;
using DailyUnoThesis.Presentation.ViewModel.NavigationClasses;

namespace DailyUnoThesis.Presentation.ViewModel.HelperClasses;
public partial class BindingProxy 
{

    private static BindingProxy _instance;
    //public static BindingProxy Instance => _instance ??= new BindingProxy();
    public static BindingProxy GetInstance()
    {
        if (_instance == null)
        {
            _instance = new BindingProxy();
        }
        return _instance;
    }

    public BindingProxy()
    {
        _instance = this;
    }

    public TaskPageControle TaskPageControle { get; set; }

    //public static readonly DependencyProperty DataProperty =
    //         DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));

    //public object Data
    //{
    //    get => GetValue(DataProperty);
    //    set => SetValue(DataProperty, value);
    //}
}
