using System;
using System.Collections;
using System.Collections.Generic;
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
            var panel = d as TimelinePanel;
            panel?.InvalidateMeasure();  
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
            var layout = ComputeOverlapLayout(finalSize.Width);

            foreach (UIElement child in Children)
            {
                var element = child as FrameworkElement;
                var task = element?.DataContext as TaskCompletionTime;

                if (task == null || !task.StartExecution.HasValue || !task.EndExecution.HasValue)
                {
                    child.Arrange(new Rect(0, 0, finalSize.Width, HourHeight));
                    continue;
                }

                var hourSlot = GetHourSlot(task.StartExecution.Value.Hour);
                bool isNonWorking = hourSlot != null && !hourSlot.IsWorkingHour;

                if (isNonWorking && !hourSlot.IsExpanded && task.Id != 0)
                {
                    element.Visibility = Visibility.Collapsed;
                    child.Arrange(new Rect(0, 0, 0, 0));
                    continue;
                }

                element.Visibility = Visibility.Visible;

                int endHour = task.EndExecution.Value.Hour;
                if (endHour == 0 && task.EndExecution.Value.Date > task.StartExecution.Value.Date)
                    endHour = 24;

                double top = GetAccumulatedHeight(task.StartExecution.Value.Hour);
                top += (task.StartExecution.Value.Minute / 60.0) * HourHeight;

                double bottom = GetAccumulatedHeight(endHour);
                bottom += (task.EndExecution.Value.Minute / 60.0) * HourHeight;
                double height = bottom - top;

                double minHeight = HourHeight;
                if (height < minHeight) height = minHeight;

                if (layout.TryGetValue(child, out var info))
                {
                    Canvas.SetZIndex(child, info.zIndex);
                    child.Arrange(new Rect(info.left, top, info.width, height));
                }
                else
                {
                    Canvas.SetZIndex(child, 0);
                    child.Arrange(new Rect(0, top, finalSize.Width, height));
                }
            }

            return finalSize;
        }

        private const double OverlapIndent = 20.0;

        private Dictionary<UIElement, (double left, double width, int zIndex)> ComputeOverlapLayout(double panelWidth)
        {
            var tasks = new List<(UIElement element, double top, double bottom)>();

            foreach (UIElement child in Children)
            {
                var element = child as FrameworkElement;
                var task = element?.DataContext as TaskCompletionTime;
                if (task == null || !task.StartExecution.HasValue || !task.EndExecution.HasValue)
                    continue;

                var hourSlot = GetHourSlot(task.StartExecution.Value.Hour);
                bool isNonWorking = hourSlot != null && !hourSlot.IsWorkingHour;
                if (isNonWorking && !hourSlot.IsExpanded && task.Id != 0)
                    continue;

                int endHour = task.EndExecution.Value.Hour;
                if (endHour == 0 && task.EndExecution.Value.Date > task.StartExecution.Value.Date)
                    endHour = 24;

                double top = GetAccumulatedHeight(task.StartExecution.Value.Hour)
                             + (task.StartExecution.Value.Minute / 60.0) * HourHeight;
                double bottom = GetAccumulatedHeight(endHour)
                                + (task.EndExecution.Value.Minute / 60.0) * HourHeight;

                tasks.Add((child, top, bottom));
            }

            tasks.Sort((a, b) => a.top.CompareTo(b.top));
            if (tasks.Count == 0) return new();

            var result = new Dictionary<UIElement, (double left, double width, int zIndex)>();

            // Build overlap clusters (connected components)
            var clusters = new List<List<(UIElement element, double top, double bottom)>>();
            var currentCluster = new List<(UIElement, double, double)> { tasks[0] };
            double clusterEnd = tasks[0].bottom;

            for (int i = 1; i < tasks.Count; i++)
            {
                if (tasks[i].top < clusterEnd)
                {
                    currentCluster.Add(tasks[i]);
                    if (tasks[i].bottom > clusterEnd)
                        clusterEnd = tasks[i].bottom;
                }
                else
                {
                    clusters.Add(currentCluster);
                    currentCluster = new List<(UIElement, double, double)> { tasks[i] };
                    clusterEnd = tasks[i].bottom;
                }
            }
            clusters.Add(currentCluster);

            // Process each cluster
            int globalZ = 0;
            foreach (var cluster in clusters)
            {
                if (cluster.Count == 1)
                {
                    result[cluster[0].element] = (0, panelWidth, globalZ++);
                    continue;
                }

                // Group by exact start time
                var groups = new Dictionary<double, List<(UIElement element, double bottom)>>();
                foreach (var t in cluster)
                {
                    if (!groups.ContainsKey(t.top))
                        groups[t.top] = new();
                    groups[t.top].Add((t.element, t.bottom));
                }

                var sortedStarts = new List<double>(groups.Keys);
                sortedStarts.Sort();

                bool isFirst = true;
                foreach (var start in sortedStarts)
                {
                    var group = groups[start];
                    if (group.Count > 1)
                    {
                        double availableWidth = isFirst ? panelWidth : panelWidth - OverlapIndent;
                        double colWidth = availableWidth / group.Count;
                        double xOffset = isFirst ? 0 : OverlapIndent;
                        for (int c = 0; c < group.Count; c++)
                        {
                            result[group[c].element] = (xOffset + c * colWidth, colWidth, globalZ++);
                        }
                    }
                    else if (isFirst)
                    {
                        result[group[0].element] = (0, panelWidth, globalZ++);
                    }
                    else
                    {
                        result[group[0].element] = (OverlapIndent, panelWidth - OverlapIndent, globalZ++);
                    }
                    isFirst = false;
                }
            }

            return result;
        }

        private HourSlot GetHourSlot(int hour)
        {
            if (Hours == null) return null;
            foreach (HourSlot slot in Hours)
            {
                if (slot.Hour == hour) return slot;
            }
            return null;
        }

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
