﻿using System;
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
using Optional;

namespace Timereporter
{
	public class GridMutator
	{
		private readonly DataGridView dgv;

		public GridMutator(DataGridView dgv)
		{
			this.dgv = dgv;
		}

		public void Load(ComboBox comboBox1)
		{
			{
				var instant = SystemClock.Instance.GetCurrentInstant();
				DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
				var monthRange = EnumerableExtensions.ReverseMonthRange(instant, tz, 9);

				var options =
					from ym in monthRange
					let readableMonth = new DateTime(ym.Year, ym.Month, 1).ToString("MMMM", CultureInfo.InvariantCulture)
					select new DateOption
					{
						YearMonth = $"{ym.Year}-{ym.Month}",
						Name = $"{ym.Year}, {readableMonth}",
						Date = ym
					};

				comboBox1.DataSource = options.ToList();
				comboBox1.DisplayMember = "Name";
				comboBox1.ValueMember = "YearMonth";
			}
		}

		public void Load(Option<LocalDate> yearMonthOption)
		{
			dgv.Rows.Clear();

			Color lgray = Color.FromArgb(255, 240, 240, 240);
			Color lred = Color.FromArgb(255, 255, 244, 244);

			LocalDate localDate = yearMonthOption.ValueOr(delegate ()
			{
				Instant now = SystemClock.Instance.GetCurrentInstant();
				DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
				return now.InZone(tz).Date;
			});

			var workdays = GetData(localDate.Year, localDate.Month).ToList();

			// Collection already belongs to a DataGridView control. This operation is no longer valid.
			for (int i = 0; i < workdays.Count; i++)
			{
				var wd = workdays[i];

				dgv.Rows.Add
				(
					wd.Date,
					wd.DayOfWeek,
					wd.ArrivalHours,
					wd.BreakHours,
					wd.DepartureHours,
					wd.Total
				);

				if (Enum.TryParse(wd.ArrivalConfidence, out TimeConfidence arrivalConfidence))
				{
					if (arrivalConfidence == TimeConfidence.Confident)
					{
						dgv.Rows[i].Cells[2].Style.BackColor = lred;
					}
				}

				if (Enum.TryParse(wd.DepartureConfidence, out TimeConfidence departureConfidence))
				{
					if (departureConfidence == TimeConfidence.Confident)
					{
						dgv.Rows[i].Cells[4].Style.BackColor = lred;
					}
				}


				if (wd.IsWeekend)
				{
					dgv.Rows[i].DefaultCellStyle.BackColor = lgray;
				}

				//dgv.Rows[i].Cells[0].Value = wd.Date;
				//dgv.Rows[i].Cells[1].Value = wd.DayOfWeek;
			}

			// Use pre-defined columns instead `dgv.DataSource = data.Workdays`;
		}

		public IEnumerable<WorkdayDetailsDto> GetData(int year, int month)
		{

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var dateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
				var dtz = dateTimeZone.Id.Replace("/", "_");
				var response = client.GetAsync($"http://localhost:53762/api/workday/{year}/{month}/{dtz}").Result;

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
