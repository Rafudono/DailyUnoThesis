using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyUnoThesis.Models;
public class NavItem
{
    public string Label { get; set; }
    public Symbol Icon { get; set; }
    public Type Page { get; set; }
}
