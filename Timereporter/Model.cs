using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timereporter
{
    public class Model
    {
        public List<WorkdayModel> GetData()
        {
            // DataTable dataTable = new DataTable();
            // return dataTable;
            var startEndOfMonth = StartEndOfMonth();
            return EnumerateWorkdays(startEndOfMonth.Item1, startEndOfMonth.Item2).ToList();
        }

        private (DateTime, DateTime) StartEndOfMonth()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            var endOfMonth = new DateTime(now.Year, now.Month, daysInMonth);
            return (startOfMonth, endOfMonth);
        }

        private IEnumerable<WorkdayModel> EnumerateWorkdays(DateTime startOfMonth, DateTime endOfMonth)
        {
            for(int i = 0; startOfMonth.AddDays(i) < endOfMonth.AddDays(1); i++)
            {
                var date = startOfMonth.AddDays(i);
                yield return new WorkdayModel
                {
                    DayOfWeek = date.DayOfWeek.ToString(),
                    Date = date.ToString("yyyy-MM-dd"),
                };
            }
        }
    }

    public class WorkdayModel
    {
        public string DayOfWeek { get; set; }
        public string Date { get; set; }
        public string ArrivalHour { get; set; }
        public string ArrivalMinute { get; set; }
        public string DepatureHour { get; set; }
        public string DepartureMinute { get; set; }
        public string LuncheonBreakHour { get; set; }
        public string Color { get; set; }
    }
}
