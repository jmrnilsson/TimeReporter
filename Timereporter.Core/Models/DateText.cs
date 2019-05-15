using NodaTime;
using System.Globalization;

namespace Timereporter.Core.Models
{
	public struct DateText
	{
		private string dateText;

		public DateText(LocalDate localDate)
		{
			dateText = localDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
		}

		public DateText(string dateText)
		{
			this.dateText = dateText;
		}

		public DateText(int dateInt32)
		{
			var dateText = dateInt32.ToString();
			this.dateText = $"{dateText.Substring(0, 4)}-{dateText.Substring(4, 2)}-{dateText.Substring(6, 2)}";
		}


		public override string ToString()
		{
			return dateText.ToString();
		}

		public int ToInt32()
		{
			return int.Parse(dateText.Replace("-", ""));
		}
	}
}
