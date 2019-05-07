using NodaTime;
using Optional;
using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Timereporter.Core;
using Timereporter.Core.Models;

namespace Timereporter
{
	public class GridActor
	{
		private readonly DataGridView dgv;

		public GridActor(DataGridView dgv)
		{
			this.dgv = dgv;
			this.dgv.CellValueChanged += Dgv_CellValueChanged;

		}

		public void Decouple()
		{
			this.dgv.CellValueChanged -= Dgv_CellValueChanged;
		}

		private void Dgv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			Option<string> GetEventName()
			{
				switch (e.ColumnIndex)
				{
					case 4: return "USER_MAX".Some();
					case 3: goto default; // throw new NotSupportedException("USER_BREAK not supported yet!");
					case 2: return "USER_MIN".Some();
					default: return Option.None<string>();
				}
			}

			void PostNew(string value_, string eventName, LocalDate localDate, DateTimeZone tdz)
			{
				long instant = value_.FromHourDecimalExpressionToUnixTimestampMilliseconds(localDate, tdz);
				ApiClient.PostEvent(new Event(eventName, instant));
			}

			void PostIgnore(string eventName, LocalDate localDate, DateTimeZone tdz)
			{
				long instant = "0.1".FromHourDecimalExpressionToUnixTimestampMilliseconds(localDate, tdz);
				ApiClient.PostEvent(new Event($"{eventName}_REMOVE", instant));
			}

			GetEventName().MatchSome(delegate (string eventName)
			{
				DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
				string formattedValue = cell.FormattedValue as string;
				string date = dgv.Rows[e.RowIndex].Cells[0].FormattedValue as string;
				DateTime localDateTime = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
				LocalDate localDate = new LocalDate(localDateTime.Year, localDateTime.Month, localDateTime.Day);
				DateTimeZone tdz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
				Option<string> value = formattedValue.SomeWhen(v => !string.IsNullOrEmpty(v));
				value.Match
				(
					some: v => PostNew(v, eventName, localDate, tdz),
					none: () => PostIgnore(eventName, localDate, tdz)
				);
			});
		

			//var d = Convert.ToDecimal(new DataTable().Compute("", null));
			//d = Math.Round(d, 1);
			//DataColumn taxColumn = new DataColumn();
			//taxColumn.DataType = System.Type.GetType("System.Decimal");
			//taxColumn.Expression = formattedValue;

			// public object Compute(string expression, string filter);
			// string key = dgv.Rows[e.RowIndex].Cells[0].Value as string;

		}
	}
}
