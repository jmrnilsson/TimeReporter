using System;
using System.Data;
using System.Windows.Forms;

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
			//int columnIndex, rowIndex;
			//columnIndex = e.ColumnIndex;
			//rowIndex = e.RowIndex;
			DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
			string formattedValue = cell.FormattedValue as string;

			var d = Convert.ToDecimal(new DataTable().Compute(formattedValue, null));
			d = Math.Round(d, 1);
			//DataColumn taxColumn = new DataColumn();
			//taxColumn.DataType = System.Type.GetType("System.Decimal");
			//taxColumn.Expression = formattedValue;

			// public object Compute(string expression, string filter);
			// string key = dgv.Rows[e.RowIndex].Cells[0].Value as string;
			throw new System.NotImplementedException();
		}
	}
}
