using System;
using System.Drawing;
using System.Windows.Forms;
using Timereporter.Models;

namespace Timereporter
{
	public static class GridMutator
	{
		public static void Load(DataGridView dgv)
		{
			Color lgray = Color.FromArgb(255, 240, 240, 240);
			var now = DateTime.Now;
			var workdays = new Workdays(now.Year, now.Month);

			// Collection already belongs to a DataGridView control. This operation is no longer valid.
			for (int i = 0; i < workdays.Count; i++)
			{
				var wd = data.Workdays[i];
				dgv.Rows.Add
				(
					wd.DateText,
					wd.DayOfWeekText
				);
				//dgv.Rows[i].Cells[0].Value = wd.Date;
				//dgv.Rows[i].Cells[1].Value = wd.DayOfWeek;
			}

			// Use pre-defined columns instead `dgv.DataSource = data.Workdays`;

			foreach (var i in data.WeekendIndices)
			{
				dgv.Rows[i].DefaultCellStyle.BackColor = lgray;
			}
		}
	}
}
