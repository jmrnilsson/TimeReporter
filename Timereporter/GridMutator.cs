using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Timereporter.Core.Models;
using Timereporter.Core.Collections;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Timereporter.Core;
using NodaTime;
using System.Globalization;

namespace Timereporter
{
	public static class GridMutator
	{
		public static void Load(ComboBox comboBox1)
		{
			{
				var instant = SystemClock.Instance.GetCurrentInstant();
				DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
				var monthRange = Extensions.ReverseMonthRange(instant, tz, 4);

				var options = monthRange.Select(m => new MonthOption
				{
					YearMonth = $"{m.Year}-{m.Month}",
					Name = new DateTime(m.Year, m.Month, 1).ToString("MMMM", CultureInfo.InvariantCulture)
				}).ToList();

				comboBox1.DataSource = options;
				comboBox1.DisplayMember = "Name";
				comboBox1.ValueMember = "YearMonth";
			}
		}

		public static void Load(DataGridView dgv)
		{
			dgv.Rows.Clear();

			Color lgray = Color.FromArgb(255, 240, 240, 240);
			Color lred = Color.FromArgb(255, 255, 244, 244);

			DateTime chosenMonth;
			{
				var now = DateTime.Now;
				chosenMonth = now.AddDays(0 - now.Day - 1);
			}
			var workdays = WorkdayHelper.Range(chosenMonth.Year, chosenMonth.Month);
			var workdayKvp = GetData(chosenMonth.Year, chosenMonth.Month);
			var workKvp = workdayKvp.ToDictionary(wd => wd.Date);

			// Collection already belongs to a DataGridView control. This operation is no longer valid.
			for (int i = 0; i < workdays.Count; i++)
			{
				IWorkday wd = workdays[i];
				if (workKvp.ContainsKey(wd.DateText) && !wd.IsWeekend())
				{
					dgv.Rows.Add
					(
						wd.DateText,
						wd.DayOfWeekText,
						workKvp[wd.DateText].ArrivalHours.ToString("0.0"),
						workKvp[wd.DateText].BreakHours.ToString("0.0"),
						workKvp[wd.DateText].DepartureHours.ToString("0.0")
					);

					if (Enum.TryParse<TimeConfidence>(workKvp[wd.DateText].ArrivalConfidence, out TimeConfidence arrivalConfidence))
					{
						if (arrivalConfidence == TimeConfidence.Confident)
						{
							dgv.Rows[i].Cells[2].Style.BackColor = lred;
						}
					}

					if (Enum.TryParse<TimeConfidence>(workKvp[wd.DateText].DepartureConfidence, out TimeConfidence departureConfidence))
					{
						if (departureConfidence == TimeConfidence.Confident)
						{
							dgv.Rows[i].Cells[4].Style.BackColor = lred;
						}
					}

				}
				else
				{
					dgv.Rows.Add
					(
						wd.DateText,
						wd.DayOfWeekText
					);
				}

				if (wd.IsWeekend())
				{
					dgv.Rows[i].DefaultCellStyle.BackColor = lgray;
				}

				//dgv.Rows[i].Cells[0].Value = wd.Date;
				//dgv.Rows[i].Cells[1].Value = wd.DayOfWeek;
			}

			// Use pre-defined columns instead `dgv.DataSource = data.Workdays`;
		}

		public static IEnumerable<WorkdayDto> GetData(int year, int month)
		{

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var response = client.GetAsync($"http://localhost:53762/api/workday/{year}/{month}").Result;

				if (!response.IsSuccessStatusCode)
				{
					throw new ApplicationException("workday get rest");
				}
				var json = response.Content.ReadAsStringAsync().Result;
				var wd = JsonConvert.DeserializeObject<Core.Models.Workdays>(json);
				return wd.List;
			}
		}
	}
}
