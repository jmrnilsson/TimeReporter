using NodaTime;
using Optional;
using System;
using System.Data;
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
					case 3: return "USER_BREAK".Some();
					case 2: return "USER_MIN".Some();
					default: return Option.None<string>();
				}
			}

			GetEventName().MatchSome(delegate (eventName)
			{
				DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
				string formattedValue = cell.FormattedValue as string;
				Instant instant = ParseFromSystemLocalToInstant()
				ApiClient.PostEvent(new Event(eventName, instant));
			});
			//int columnIndex, rowIndex;
			//columnIndex = e.ColumnIndex;
			//rowIndex = e.RowIndex;

			

			var d = Convert.ToDecimal(new DataTable().Compute(formattedValue, null));
			d = Math.Round(d, 1);
			//DataColumn taxColumn = new DataColumn();
			//taxColumn.DataType = System.Type.GetType("System.Decimal");
			//taxColumn.Expression = formattedValue;

			// public object Compute(string expression, string filter);
			// string key = dgv.Rows[e.RowIndex].Cells[0].Value as string;

		}
	}
}
