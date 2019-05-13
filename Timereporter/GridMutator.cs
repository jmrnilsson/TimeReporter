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
using Optional;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace Timereporter
{
	public class GridMutator
	{
		private readonly DataGridView dgv;
		private BindingList<WorkdayDetailsBindingListItem> workdayDetailsBindingList;
		private BindingSource workdayBindingSource = new BindingSource();
		private long lastDataBindMillisecond;


		public GridMutator(DataGridView dgv)
		{
			// https://stackoverflow.com/questions/13395321/change-column-name-in-datagridview
			// https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/how-to-bind-data-to-the-windows-forms-datagridview-control
			// https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/raise-change-notifications--bindingsource
			// https://stackoverflow.com/questions/9758577/c-sharp-datagridview-not-updated-when-datasource-is-changed
			this.dgv = dgv;
			dgv.DataBindingComplete += Dgv_DataBindingComplete;

			this.workdayDetailsBindingList = new BindingList<WorkdayDetailsBindingListItem>();

			for(int i = 0; i < 31; i++)
			{
				this.workdayDetailsBindingList.Add(new WorkdayDetailsBindingListItem());
			}

			this.workdayBindingSource.DataSource = this.workdayDetailsBindingList;
			this.dgv.DataSource = workdayBindingSource;

			this.dgv.Columns[0].HeaderText = "Date";
			this.dgv.Columns[1].HeaderText = "Day of week";
			this.dgv.Columns[2].HeaderText = "Arrival";
			this.dgv.Columns[3].HeaderText = "Break";
			this.dgv.Columns[4].HeaderText = "Depature";
			this.dgv.Columns[5].HeaderText = "Total (H)";
		}

		private void Dgv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
		{
			lastDataBindMillisecond = SystemClock.Instance.GetCurrentInstant().ToUnixTimeMilliseconds();
		}

		public void Load(ComboBox comboBox1)
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

		public void DeferredLoad()
		{
			var loadStarted = SystemClock.Instance.GetCurrentInstant();
			while (SystemClock.Instance.GetCurrentInstant().ToUnixTimeMilliseconds() - lastDataBindMillisecond > 1000)
			{
				var now = SystemClock.Instance.GetCurrentInstant();
				if (now.ToUnixTimeSeconds() - loadStarted.ToUnixTimeSeconds() > 5)
				{
					break;
				}
				Thread.Sleep(100);
			}

			var instant = SystemClock.Instance.GetCurrentInstant();
			DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
			var monthRange = EnumerableExtensions.ReverseMonthRange(instant, tz, 1);

			Load(monthRange.First().Some());
		}

		public void Load(Option<LocalDate> yearMonthOption)
		{

			Color lightGray = Color.FromArgb(255, 240, 240, 240);
			Color lightRed = Color.FromArgb(255, 255, 244, 244);

			LocalDate localDate = yearMonthOption.ValueOr(delegate ()
			{
				Instant now = SystemClock.Instance.GetCurrentInstant();
				DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
				return now.InZone(tz).Date;
			});

			var workdays = GetData(localDate.Year, localDate.Month).ToList();

			// Collection already belongs to a DataGridView control. This operation is no longer valid.
			for(int i = 0; i < workdayDetailsBindingList.Count; i++)
			{
				var item = workdayDetailsBindingList[i];
				if (i < workdays.Count)
				{
					var wd = workdays[i];
					item.ArrivalHours = wd.ArrivalHours;
					item.BreakHours = wd.BreakHours;
					item.DepartureHours = wd.DepartureHours;
					item.DayOfWeek = wd.DayOfWeek;
					item.Total = wd.Total;
					item.WeekNumber = wd.WeekNumber;
					item.Date = wd.Date;

					ApplyCellFormatting(lightGray, lightRed, i, wd);
				}
				else
				{

					item.ArrivalHours = null;
					item.BreakHours = null;
					item.DepartureHours = null;
					item.DayOfWeek = null;
					item.Total = null;
					item.WeekNumber = null;
					item.Date = null;

					ResetCellFormatting(i);
				}
			}
			this.dgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
		}

		private void ResetCellFormatting(int i)
		{
			dgv.Rows[i].Cells[2].Style.BackColor = Color.Empty;
			dgv.Rows[i].Cells[4].Style.BackColor = Color.Empty;
			dgv.Rows[i].DefaultCellStyle.BackColor = Color.Empty;
		}

		private void ApplyCellFormatting(Color lightGray, Color lightRed, int i, WorkdayDetailsDto wd)
		{
			if (Enum.TryParse(wd.ArrivalConfidence, out TimeConfidence arrivalConfidence) && arrivalConfidence == TimeConfidence.Confident)
			{
				dgv.Rows[i].Cells[2].Style.BackColor = lightRed;
			}
			else if (dgv.Rows[i].Cells[2].Style.BackColor != Color.Empty)
			{
				dgv.Rows[i].Cells[2].Style.BackColor = Color.Empty;
			}

			if (Enum.TryParse(wd.DepartureConfidence, out TimeConfidence departureConfidence) && departureConfidence == TimeConfidence.Confident)
			{
				dgv.Rows[i].Cells[4].Style.BackColor = lightRed;
			}
			else if (dgv.Rows[i].Cells[4].Style.BackColor != Color.Empty)
			{
				dgv.Rows[i].Cells[4].Style.BackColor = Color.Empty;
			}

			if (wd.IsWeekend)
			{
				dgv.Rows[i].DefaultCellStyle.BackColor = lightGray;
			}
			else if (dgv.Rows[i].DefaultCellStyle.BackColor != Color.Empty)
			{
				dgv.Rows[i].DefaultCellStyle.BackColor = Color.Empty;
			}
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
