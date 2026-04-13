using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Data;
using Microsoft.Xaml.Interactivity;
using System.Collections.Specialized;

namespace DailyUnoThesis.Presentation.View.Calendar;
public class CalendarWidthBehavior : Behavior<ItemsControl>
{
    private INotifyCollectionChanged _currentCollection;
    private readonly DispatcherQueue _dispatcherQueue;

    public CalendarWidthBehavior()
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += OnLoaded;
        AssociatedObject.SizeChanged += OnSizeChanged;

        // Просто регистрируем без сохранения токена
        AssociatedObject.RegisterPropertyChangedCallback(
            ItemsControl.ItemsSourceProperty,
            OnItemsSourceChanged);

        // Сразу подписываемся на коллекцию, если она есть
        UpdateCollectionSubscription();
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Loaded -= OnLoaded;
        AssociatedObject.SizeChanged -= OnSizeChanged;

        // Отписываемся от коллекции
        UnsubscribeFromCollection();

        // Для PropertyChangedCallback отписка не обязательна,
        // так как Behavior уничтожается вместе с AssociatedObject
    }

    private void OnItemsSourceChanged(DependencyObject sender, DependencyProperty dp)
    {
        UpdateCollectionSubscription();
        UpdateItemsWidth();
    }

    private void UpdateCollectionSubscription()
    {
        UnsubscribeFromCollection();

        if (AssociatedObject?.ItemsSource is INotifyCollectionChanged collection)
        {
            _currentCollection = collection;
            collection.CollectionChanged += OnCollectionChanged;
        }
    }

    private void UnsubscribeFromCollection()
    {
        if (_currentCollection != null)
        {
            _currentCollection.CollectionChanged -= OnCollectionChanged;
            _currentCollection = null;
        }
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        _dispatcherQueue.TryEnqueue(UpdateItemsWidth);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateItemsWidth();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateItemsWidth();
    }

    private void UpdateItemsWidth()
    {
        var itemsControl = AssociatedObject;
        if (itemsControl == null || itemsControl.ActualWidth <= 0) return;

        var itemWidth = itemsControl.ActualWidth / 7;

        for (int i = 0; i < itemsControl.Items.Count; i++)
        {
            if (itemsControl.ContainerFromIndex(i) is FrameworkElement container)
            {
                container.Width = itemWidth;
            }
        }
    }
}
