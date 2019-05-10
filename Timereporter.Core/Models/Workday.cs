using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using Timereporter.Core.Models;

namespace Timereporter.Core.Models
{

	public class Workday : IWorkday
	{
		private readonly LocalDate date;
		private int arrival;
		private int @break;
		private int departure;
		private IList<Workday> otherDays;

		public Workday()
		{

		}

		public Workday(LocalDate date, int arrival, int @break, int departure)
		{
			this.date = date;
			this.arrival = arrival;
			this.@break = @break;
			this.departure = departure;
			otherDays = new List<Workday>();
		}

		public Workday(DateTime date, int arrival, int @break, int departure)
		{
			this.date = new LocalDate(date.Year, date.Month, date.Day);
			this.arrival = arrival;
			this.@break = @break;
			this.departure = departure;
			otherDays = new List<Workday>();
		}

		public string DayOfWeekText => date.DayOfWeek.ToString();

		public string DateText => new DateText(date).ToString();

		public string ArrivalText => arrival.ToString();

		public string BreakText => @break.ToString();

		public string DepatureText => departure.ToString();

		public int Week => Total + otherDays.Sum(wd => wd.Total);

		public int Total => departure - @break - arrival;

		public void Update(int columnIndex, string value)
		{
			switch (columnIndex)
			{
				case 2: arrival = int.Parse(value); break;
				case 3: @break = int.Parse(value); break;
				case 4: departure = int.Parse(value); break;
				default: throw new ArgumentException($"Column index {columnIndex} is not mutable for a workday.");
			}
		}

		public bool Is(IsoDayOfWeek dayOfWeek)
		{
			return date.DayOfWeek == dayOfWeek;
		}

		public bool IsWeekend()
		{
			return date.IsWeekend();
		}

	}
}
