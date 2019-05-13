using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Timereporter.Core.Models
{
	// This could most likely be replaced with something that already implements
	// INotifyPropertyChanged by default. Maybe ExpandoObject does this already?
	public class WorkdayDetailsBindingListItem : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private string date;
		private string dayOfWeek;
		private string arrivalHours;
		private string breakHours;
		private string departureHours;
		private string total;
		private int? weekNumber;

		public string Date
		{
			get
			{
				return this.date;
			}

			set
			{
				if (value != this.date)
				{
					this.date = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string DayOfWeek
		{
			get
			{
				return this.dayOfWeek;
			}

			set
			{
				if (value != this.dayOfWeek)
				{
					this.dayOfWeek = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string ArrivalHours
		{
			get
			{
				return this.arrivalHours;
			}

			set
			{
				if (value != this.arrivalHours)
				{
					this.arrivalHours = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string BreakHours
		{
			get
			{
				return this.breakHours;
			}

			set
			{
				if (value != this.breakHours)
				{
					this.breakHours = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string DepartureHours
		{
			get
			{
				return this.departureHours;
			}

			set
			{
				if (value != this.departureHours)
				{
					this.departureHours = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string Total
		{
			get
			{
				return this.total;
			}

			set
			{
				if (value != this.total)
				{
					this.total = value;
					NotifyPropertyChanged();
				}
			}
		}

		public int? WeekNumber
		{
			get
			{
				return this.weekNumber;
			}

			set
			{
				if (value != this.weekNumber)
				{
					this.weekNumber = value;
					NotifyPropertyChanged();
				}
			}
		}
	}
}
