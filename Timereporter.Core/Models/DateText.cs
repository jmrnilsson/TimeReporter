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

		public override string ToString()
		{
			return dateText.ToString();
		}
	}
}
