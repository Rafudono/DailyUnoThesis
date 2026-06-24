using DailyUnoThesis.Models.MainClasses;
using Windows.ApplicationModel.DataTransfer;

namespace DailyUnoThesis.Presentation.View.Calendar;

internal static class DeadlineHelper
{
    public static void AddDeadlineProperties(DataPackagePropertySet properties, Mission? mission)
    {
        if (mission == null) return;

        DateTime? overallMin = null, overallMax = null;

        var current = mission;
        bool isTargetMission = true;
        while (current != null)
        {
            if (current.StartDate != null)
            {
                if (current.StartDate.Value.Date != current.EndDate?.Date)
                {
                    if (overallMin == null || current.StartDate.Value.Date > overallMin.Value)
                        overallMin = current.StartDate.Value.Date;
                    if (overallMax == null || current.EndDate.Value < overallMax.Value)
                        overallMax = current.EndDate.Value;
                }
                else if (!isTargetMission)
                {
                    if (overallMax == null || current.EndDate.Value < overallMax.Value)
                        overallMax = current.EndDate.Value;
                }
            }
            current = current.IdUpMissionNavigation;
            isTargetMission = false;
        }

        if (overallMax == null) return;

        properties.Add("DragMaxDate", overallMax.Value);
        if (overallMin != null)
            properties.Add("DragMinDate", overallMin.Value);
    }

    public static bool IsDateAllowedByDeadline(DateTime targetDate, DataPackagePropertySetView properties)
    {
        if (!properties.TryGetValue("DragMaxDate", out var maxObj) || maxObj is not DateTime max)
            return true;

        if (targetDate > max) return false;

        if (properties.TryGetValue("DragMinDate", out var minObj) && minObj is DateTime min)
            return targetDate >= min;

        return true;
    }

    public static bool IsAllowedByDeadlineTime(DateTime targetDateTime, DataPackagePropertySetView properties)
    {
        if (!properties.TryGetValue("DragMaxDate", out var maxObj) || maxObj is not DateTime max)
            return true;

        if (max.TimeOfDay.TotalSeconds <= 1)
            return true;

        return targetDateTime <= max;
    }
}
