using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Timereporter.Models;

namespace Timereporter
{
	public class GridMetadataFactory
    {
        public GridMetadata GetData()
        {
			//bool DayIsWeekend(Workday wd)
			//{
			//	return wd.DayOfWeek == DayOfWeek.Saturday.ToString()
			//		|| wd.DayOfWeek == DayOfWeek.Sunday.ToString();
			//}

            // DataTable dataTable = new DataTable();
            // return dataTable;
            var startEndOfMonth = StartEndOfMonth();
            var workdays = EnumerateWorkdays(startEndOfMonth.Item1, startEndOfMonth.Item2).ToList();

			return new GridMetadata
			{
				Workdays = workdays,
				WeekendIndices = new HashSet<int>(GetWeekendIndices(workdays)),
			};
        }

		private IEnumerable<int> GetWeekendIndices(List<Workday> workdays)
		{
			int i = 0;
			while(i < workdays.Count)
			{
				if (workdays[i].DayOfWeek == DayOfWeek.Sunday.ToString())
				{
					yield return i;
				}
				else if (workdays[i].DayOfWeek == DayOfWeek.Saturday.ToString())
				{
					yield return i;
				}
				else if (workdays[i].DayOfWeek == DayOfWeek.Monday.ToString())
				{
					i += 5;
					continue;
				}
				i++;
			}
		}

        private (DateTime, DateTime) StartEndOfMonth()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            var endOfMonth = new DateTime(now.Year, now.Month, daysInMonth);
            return (startOfMonth, endOfMonth);
        }

        private IEnumerable<Workday> EnumerateWorkdays(DateTime startOfMonth, DateTime endOfMonth)
        {
            for(int i = 0; startOfMonth.AddDays(i) < endOfMonth.AddDays(1); i++)
            {
                var date = startOfMonth.AddDays(i);
                yield return new Workday
                {
                    DayOfWeek = date.DayOfWeek.ToString(),
                    Date = date.ToString("yyyy-MM-dd"),
                };
            }
        }
    }
}
