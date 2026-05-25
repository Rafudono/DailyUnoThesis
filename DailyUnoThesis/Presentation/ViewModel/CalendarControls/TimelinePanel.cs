using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using DailyUnoThesis.Models.MainClasses;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace DailyUnoThesis.Presentation.ViewModel.CalendarControls
{
    public class TimelinePanel : Panel
    {
        private INotifyCollectionChanged _notifyCollection;

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(TimelinePanel),
                new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(TimelinePanel),
                new PropertyMetadata(null, OnItemTemplateChanged));

        public static readonly DependencyProperty StartHourProperty =
            DependencyProperty.Register(nameof(StartHour), typeof(int), typeof(TimelinePanel),
                new PropertyMetadata(9, OnLayoutPropertyChanged));

        public static readonly DependencyProperty EndHourProperty =
            DependencyProperty.Register(nameof(EndHour), typeof(int), typeof(TimelinePanel),
                new PropertyMetadata(22, OnLayoutPropertyChanged));

        public static readonly DependencyProperty HourHeightProperty =
            DependencyProperty.Register(nameof(HourHeight), typeof(double), typeof(TimelinePanel),
                new PropertyMetadata(60.0, OnLayoutPropertyChanged));


        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public int StartHour
        {
            get => (int)GetValue(StartHourProperty);
            set => SetValue(StartHourProperty, value);
        }

        public int EndHour
        {
            get => (int)GetValue(EndHourProperty);
            set => SetValue(EndHourProperty, value);
        }



        public double HourHeight
        {
            get => (double)GetValue(HourHeightProperty);
            set => SetValue(HourHeightProperty, value);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as TimelinePanel;

            // Отписываемся от старой коллекции
            if (panel?._notifyCollection != null)
            {
                panel._notifyCollection.CollectionChanged -= panel.OnCollectionChanged;
                panel._notifyCollection = null;
            }

            // Подписываемся на новую коллекцию
            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                panel._notifyCollection = newCollection;
                panel._notifyCollection.CollectionChanged += panel.OnCollectionChanged;
            }

            panel?.RebuildChildren();
        }

        private static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TimelinePanel)?.RebuildChildren();
        }

        private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //  (d as TimelinePanel)?.InvalidateArrange();
            var panel = d as TimelinePanel;
            panel?.InvalidateMeasure();  // Добавьте эту строку
            panel?.InvalidateArrange();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RebuildChildren();
        }

        private void RebuildChildren()
        {
            Children.Clear();

            if (ItemsSource == null || ItemTemplate == null)
                return;

            foreach (var item in ItemsSource)
            {
                var content = ItemTemplate.LoadContent() as FrameworkElement;
                if (content != null)
                {
                    content.DataContext = item;
                    Children.Add(content);
                }
            }

            InvalidateArrange();
            InvalidateMeasure();
        }

        //private double PixelsPerMinute => HourHeight / 60.0;
        //private double TotalMinutes => (EndHour - StartHour) * 60;

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
            {
                child.Measure(new Size(availableSize.Width, double.PositiveInfinity));
            }

            // Вычисляем общую высоту через Hours, а не через StartHour/EndHour
            double totalHeight = GetTotalHeight();
            return new Size(availableSize.Width, totalHeight);
        }

        private double GetTotalHeight()
        {
            if (Hours == null) return (EndHour - StartHour) * HourHeight;

            double total = 0;
            foreach (HourSlot slot in Hours)
            {
                total += slot.Height;
            }
            return total;
        }
        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    foreach (UIElement child in Children)
        //    {
        //        child.Measure(new Size(availableSize.Width, double.PositiveInfinity));
        //    }

        //    double totalHeight = (EndHour - StartHour) * HourHeight;
        //    return new Size(availableSize.Width, totalHeight);
        //}

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in Children)
            {
                var task = (child as FrameworkElement)?.DataContext as TaskCompletionTime;

                if (task == null || !task.StartExecution.HasValue || !task.EndExecution.HasValue)
                {
                    child.Arrange(new Rect(0, 0, finalSize.Width, HourHeight));
                    continue;
                }

                // Вычисляем top с учетом свернутых часов
                double top = GetAccumulatedHeight(task.StartExecution.Value.Hour);

                // Добавляем минуты
                top += (task.StartExecution.Value.Minute / 60.0) * HourHeight;

                // Вычисляем высоту (с учетом свернутых часов в конце)
                double bottom = GetAccumulatedHeight(task.EndExecution.Value.Hour);
                bottom += (task.EndExecution.Value.Minute / 60.0) * HourHeight;
                double height = bottom - top;

                if (height < 4) height = 4;

                child.Arrange(new Rect(0, top, finalSize.Width, height));
            }

            return finalSize;
        }

        //public static readonly DependencyProperty HoursProperty =
        //DependencyProperty.Register(nameof(Hours), typeof(IEnumerable), typeof(TimelinePanel),
        //    new PropertyMetadata(null, OnLayoutPropertyChanged));
        public static readonly DependencyProperty HoursProperty =
        DependencyProperty.Register(nameof(Hours), typeof(IEnumerable), typeof(TimelinePanel),
        new PropertyMetadata(null, OnHoursChanged));

        public IEnumerable Hours
        {
            get => (IEnumerable)GetValue(HoursProperty);
            set => SetValue(HoursProperty, value);
        }

        private double GetAccumulatedHeight(int targetHour)
        {
            if (Hours == null) return targetHour * HourHeight;

            double total = 0;
            int currentHour = 0;
            foreach (HourSlot slot in Hours)
            {
                if (currentHour >= targetHour) break;
                total += slot.Height;
                currentHour++;
            }
            return total;
        }

        public void RefreshLayout()
        {
            InvalidateMeasure();
            InvalidateArrange();
        }


        private static void OnHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as TimelinePanel;

            // Отписываемся от старых элементов
            if (e.OldValue is IEnumerable oldEnumerable)
            {
                foreach (var item in oldEnumerable)
                {
                    if (item is INotifyPropertyChanged notify)
                        notify.PropertyChanged -= panel.OnHourSlotPropertyChanged;
                }
            }

            // Подписываемся на новые элементы
            if (e.NewValue is IEnumerable newEnumerable)
            {
                foreach (var item in newEnumerable)
                {
                    if (item is INotifyPropertyChanged notify)
                        notify.PropertyChanged += panel.OnHourSlotPropertyChanged;
                }
            }

            panel?.InvalidateMeasure();
            panel?.InvalidateArrange();
        }

        private void OnHourSlotPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HourSlot.Height) || e.PropertyName == nameof(HourSlot.IsExpanded))
            {
                InvalidateMeasure();
                InvalidateArrange();
            }
        }

        public double GetTopPosition(DateTime time)
        {
            double top = GetAccumulatedHeight(time.Hour);
            top += (time.Minute / 60.0) * HourHeight;
            return top;
        }
    }
}
