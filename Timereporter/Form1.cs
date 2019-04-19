using System;
using System.Drawing;
using System.Windows.Forms;

namespace Timereporter
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void ShowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Show();
			this.CenterToScreen();
		}

		private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void Form1_Move(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized)
			{
				this.Hide();
				notifyIcon1.ShowBalloonTip(1000, "Notice", "Something has happened", ToolTipIcon.Info);
			}
		}

		private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.Show();
			this.WindowState = FormWindowState.Normal;
		}

		private void LoadData()
		{
			Color lgray = Color.FromArgb(255, 240, 240, 240);
			var model = new GridMetadataFactory();
			var data = model.GetData();
			dataGridView1.DataSource = data.Workdays;

			foreach (var i in data.WeekendIndices)
			{
				dataGridView1.Rows[i].DefaultCellStyle.BackColor = lgray;
			}
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			LoadData();
		}
	}
}
