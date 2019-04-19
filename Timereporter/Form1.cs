using System;
using System.Windows.Forms;

namespace Timereporter
{
	public partial class Form1 : Form
	{
		private GridActor _gridActor;

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
			GridMutator.Load(dataGridView1);
			_gridActor = new GridActor(dataGridView1);
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			LoadData();
		}
	}
}
