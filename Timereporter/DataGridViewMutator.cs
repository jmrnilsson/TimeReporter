using System.Drawing;
using System.Windows.Forms;

namespace Timereporter
{
	public static class GridMutator
	{
		public static void Load(DataGridView dgv)
		{
			Color lgray = Color.FromArgb(255, 240, 240, 240);
			var model = new GridMetadataFactory();
			var data = model.GetData();

			// Collection already belongs to a DataGridView control. This operation is no longer valid.
			for (int i = 0; i < data.Workdays.Count; i++)
			{
				var wd = data.Workdays[i];
				dgv.Rows.Add
				(
					wd.Date,
					wd.DayOfWeek
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
			//int columnIndex, rowIndex;
			//columnIndex = e.ColumnIndex;
			//rowIndex = e.RowIndex;
			DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
			throw new System.NotImplementedException();
		}
	}
}
