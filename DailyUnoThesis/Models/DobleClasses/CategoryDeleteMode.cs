using System;
using System.Collections.Generic;
using System.Text;

namespace DailyUnoThesis.Models.DobleClasses;
public enum CategoryDeleteMode
{
    MoveToParent,           // Передать "наследство" выше
    Cascade,               // Сжечь всё дотла
    OrphanTasks,          // Оставить задачи "сиротками" (без категорий)
    EmptyOnlyDelete,     // Удаление пустой категории
    DeleteWithMissions  // Удаление категории с её задачами
}
