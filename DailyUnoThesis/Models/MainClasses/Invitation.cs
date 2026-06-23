using System;
using System.Collections.Generic;
using System.Text;

namespace DailyUnoThesis.Models.MainClasses
{
    public partial class Invitation
    {
        public int Id { get; set; }

        public int IdFromUser { get; set; }

        public int IdToUser { get; set; }

        public int IdProject { get; set; }

        public virtual User IdFromUserNavigation { get; set; } = null!;

        public virtual Category IdProjectNavigation { get; set; } = null!;

        public virtual User IdToUserNavigation { get; set; } = null!;
    }

}
