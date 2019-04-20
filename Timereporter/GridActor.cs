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
			// DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
			string key = dgv.Rows[e.RowIndex].Cells[0].Value as string;
			throw new System.NotImplementedException();
		}
	}
}
