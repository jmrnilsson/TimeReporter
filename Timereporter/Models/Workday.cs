using System;
using System.Collections.Generic;
using System.Linq;

namespace Timereporter.Models
{

	public class Workday : IWorkday
	{
		private readonly DateTime dateTime;
		private int arrival;
		private int @break;
		private int departure;
		private IList<Workday> otherDays;

		public Workday(DateTime dateTime, int arrival, int @break, int departure)
		{
			this.dateTime = dateTime;
			this.arrival = arrival;
			this.@break = @break;
			this.departure = departure;
			this.otherDays = new List<Workday>();
		}

		public string DayOfWeekText => dateTime.DayOfWeek.ToString();

		public string DateText => dateTime.ToString("yyyy-MM-dd");

		public string ArrivalText => arrival.ToString();

		public string BreakText => @break.ToString();

		public string DepatureText => departure.ToString();

		public int Week => Total + otherDays.Sum(wd => wd.Total);

		public int Total => departure - @break - arrival;

		public void Update(int columnIndex, string value)
		{
			switch (columnIndex)
			{
				case 2: arrival = Int32.Parse(value); break;
				case 3: @break = Int32.Parse(value); break;
				case 4: departure = Int32.Parse(value); break;
				default: throw new ArgumentException($"Column index {columnIndex} is not mutable for a workday.");
			}
		}

		public bool Is(DayOfWeek dayOfWeek)
		{
			return dateTime.DayOfWeek == dayOfWeek;
		}

	}
}
