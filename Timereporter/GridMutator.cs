using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Timereporter.Core.Models;
using Timereporter.Core.Collections;

namespace Timereporter
{
	public static class GridMutator
	{
		public static void Load(DataGridView dgv)
		{
			dgv.Rows.Clear();

			Color lgray = Color.FromArgb(255, 240, 240, 240);
			var now = DateTime.Now;
			var workdays = WorkdayHelper.Range(now.Year, now.Month);

			// Collection already belongs to a DataGridView control. This operation is no longer valid.
			for (int i = 0; i < workdays.Count; i++)
			{
				Workday wd = workdays[i];
				dgv.Rows.Add
				(
					wd.DateText,
					wd.DayOfWeekText
				);

				if (wd.IsWeekend())
				{
					dgv.Rows[i].DefaultCellStyle.BackColor = lgray;
				}
				//dgv.Rows[i].Cells[0].Value = wd.Date;
				//dgv.Rows[i].Cells[1].Value = wd.DayOfWeek;
			}

			// Use pre-defined columns instead `dgv.DataSource = data.Workdays`;
		}
	}
}
